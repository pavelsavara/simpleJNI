using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;

// from dotnet/runtime \src\libraries\System.Runtime.InteropServices\gen\Microsoft.Interop.SourceGeneration
namespace Microsoft.Interop;

public static class SyntaxExtensions
{
    public static SyntaxTokenList StripTriviaFromTokens(this SyntaxTokenList tokenList)
    {
        SyntaxToken[] strippedTokens = new SyntaxToken[tokenList.Count];
        for (int i = 0; i < tokenList.Count; i++)
        {
            strippedTokens[i] = tokenList[i].WithoutTrivia();
        }
        return new SyntaxTokenList(strippedTokens);
    }

    public static SyntaxTokenList AddToModifiers(this SyntaxTokenList modifiers, SyntaxKind modifierToAdd)
    {
        if (modifiers.IndexOf(modifierToAdd) >= 0)
            return modifiers;

        int idx = modifiers.IndexOf(SyntaxKind.PartialKeyword);
        return idx >= 0
            ? modifiers.Insert(idx, SyntaxFactory.Token(modifierToAdd))
            : modifiers.Add(SyntaxFactory.Token(modifierToAdd));
    }
}

internal static class Comparers
{
    /// <summary>
    /// Comparer for an individual generated stub source as a syntax tree and the generated diagnostics for the stub.
    /// </summary>
    public static readonly IEqualityComparer<(MemberDeclarationSyntax Syntax, ImmutableArray<Diagnostic> Diagnostics)> GeneratedSyntax = new CustomValueTupleElementComparer<MemberDeclarationSyntax, ImmutableArray<Diagnostic>>(SyntaxEquivalentComparer.Instance, new ImmutableArraySequenceEqualComparer<Diagnostic>(EqualityComparer<Diagnostic>.Default));
}

internal sealed class SyntaxEquivalentComparer : IEqualityComparer<SyntaxNode>, IEqualityComparer<SyntaxToken>
{
    public static readonly SyntaxEquivalentComparer Instance = new();

    public bool Equals(SyntaxNode x, SyntaxNode y)
    {
        if ((x is null) != (y is null))
        {
            return false;
        }
        // Implies that y is also null.
        if (x is null)
        {
            return true;
        }
        return x.IsEquivalentTo(y);
    }

    public bool Equals(SyntaxToken x, SyntaxToken y)
    {
        return x.IsEquivalentTo(y);
    }

    public int GetHashCode(SyntaxNode obj)
    {
        throw new InvalidProgramException();
    }

    public int GetHashCode(SyntaxToken obj)
    {
        throw new InvalidProgramException();
    }
}

internal sealed class CustomValueTupleElementComparer<T, U> : IEqualityComparer<(T, U)>
{
    private readonly IEqualityComparer<T> _item1Comparer;
    private readonly IEqualityComparer<U> _item2Comparer;

    public CustomValueTupleElementComparer(IEqualityComparer<T> item1Comparer, IEqualityComparer<U> item2Comparer)
    {
        _item1Comparer = item1Comparer;
        _item2Comparer = item2Comparer;
    }

    public bool Equals((T, U) x, (T, U) y)
    {
        return _item1Comparer.Equals(x.Item1, y.Item1) && _item2Comparer.Equals(x.Item2, y.Item2);
    }

    public int GetHashCode((T, U) obj)
    {
        throw new InvalidProgramException();
    }
}

/// <summary>
/// Generic comparer to compare two <see cref="ImmutableArray{T}"/> instances element by element.
/// </summary>
/// <typeparam name="T">The type of immutable array element.</typeparam>
internal sealed class ImmutableArraySequenceEqualComparer<T> : IEqualityComparer<ImmutableArray<T>>
{
    private readonly IEqualityComparer<T> _elementComparer;

    /// <summary>
    /// Creates an <see cref="ImmutableArraySequenceEqualComparer{T}"/> with a custom comparer for the elements of the collection.
    /// </summary>
    /// <param name="elementComparer">The comparer instance for the collection elements.</param>
    public ImmutableArraySequenceEqualComparer(IEqualityComparer<T> elementComparer)
    {
        _elementComparer = elementComparer;
    }

    public bool Equals(ImmutableArray<T> x, ImmutableArray<T> y)
    {
        return x.SequenceEqual(y, _elementComparer);
    }

    public int GetHashCode(ImmutableArray<T> obj)
    {
        throw new InvalidProgramException();
    }
}


