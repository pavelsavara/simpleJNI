﻿using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Text;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

// from dotnet/runtime \src\libraries\System.Runtime.InteropServices\gen\Microsoft.Interop.SourceGeneration
namespace Microsoft.Interop;

public readonly record struct ContainingSyntax(SyntaxTokenList Modifiers, SyntaxKind TypeKind, SyntaxToken Identifier, TypeParameterListSyntax? TypeParameters)
{
    public bool Equals(ContainingSyntax other)
    {
        return Modifiers.SequenceEqual(other.Modifiers, SyntaxEquivalentComparer.Instance)
            && TypeKind == other.TypeKind
            && Identifier.IsEquivalentTo(other.Identifier)
            && SyntaxEquivalentComparer.Instance.Equals(TypeParameters, other.TypeParameters);
    }

    public override int GetHashCode() => throw new InvalidProgramException();
}

public sealed record ContainingSyntaxContext(ImmutableArray<ContainingSyntax> ContainingSyntax, string? ContainingNamespace)
{
    public ContainingSyntaxContext(MemberDeclarationSyntax memberDeclaration)
        : this(GetContainingTypes(memberDeclaration), GetContainingNamespace(memberDeclaration))
    {
    }

    public ContainingSyntaxContext AddContainingSyntax(ContainingSyntax nestedType)
    {
        return this with { ContainingSyntax = ContainingSyntax.Insert(0, nestedType) };
    }

    private static ImmutableArray<ContainingSyntax> GetContainingTypes(MemberDeclarationSyntax memberDeclaration)
    {
        ImmutableArray<ContainingSyntax>.Builder containingTypeInfoBuilder = ImmutableArray.CreateBuilder<ContainingSyntax>();
        for (SyntaxNode? parent = memberDeclaration.Parent; parent is TypeDeclarationSyntax typeDeclaration; parent = parent.Parent)
        {
            containingTypeInfoBuilder.Add(new ContainingSyntax(typeDeclaration.Modifiers.StripTriviaFromTokens(), typeDeclaration.Kind(), typeDeclaration.Identifier.WithoutTrivia(),
                typeDeclaration.TypeParameterList));
        }

        return containingTypeInfoBuilder.ToImmutable();
    }

    private static string GetContainingNamespace(MemberDeclarationSyntax memberDeclaration)
    {
        StringBuilder? containingNamespace = null;
        for (SyntaxNode? parent = memberDeclaration.FirstAncestorOrSelf<BaseNamespaceDeclarationSyntax>(); parent is BaseNamespaceDeclarationSyntax ns; parent = parent.Parent)
        {
            if (containingNamespace is null)
            {
                containingNamespace = new StringBuilder(ns.Name.ToString());
            }
            else
            {
                string namespaceName = ns.Name.ToString();
                containingNamespace.Insert(0, namespaceName + ".");
            }
        }

        return containingNamespace?.ToString();
    }

    public bool Equals(ContainingSyntaxContext other)
    {
        return ContainingSyntax.SequenceEqual(other.ContainingSyntax)
            && ContainingNamespace == other.ContainingNamespace;
    }

    public override int GetHashCode()
    {
        int code = ContainingNamespace?.GetHashCode() ?? 0;
        foreach (ContainingSyntax containingSyntax in ContainingSyntax)
        {
            code ^= containingSyntax.Identifier.Value.GetHashCode();
        }
        return code;
    }

    public MemberDeclarationSyntax WrapMemberInContainingSyntaxWithUnsafeModifier(MemberDeclarationSyntax member)
    {
        bool addedUnsafe = false;
        MemberDeclarationSyntax wrappedMember = member;
        foreach (var containingType in ContainingSyntax)
        {
            TypeDeclarationSyntax type = TypeDeclaration(containingType.TypeKind, containingType.Identifier)
                .WithModifiers(containingType.Modifiers)
                .AddMembers(wrappedMember);
            if (!addedUnsafe)
            {
                type = type.WithModifiers(type.Modifiers.AddToModifiers(SyntaxKind.UnsafeKeyword));
            }
            if (containingType.TypeParameters is not null)
            {
                type = type.AddTypeParameterListParameters(containingType.TypeParameters.Parameters.ToArray());
            }
            wrappedMember = type;
        }
        if (ContainingNamespace is not null)
        {
            wrappedMember = NamespaceDeclaration(ParseName(ContainingNamespace)).AddMembers(wrappedMember);
        }
        return wrappedMember;
    }

    public MemberDeclarationSyntax WrapMembersInContainingSyntaxWithUnsafeModifier(params MemberDeclarationSyntax[] members)
    {
        bool addedUnsafe = false;
        MemberDeclarationSyntax? wrappedMember = null;
        foreach (var containingType in ContainingSyntax)
        {
            TypeDeclarationSyntax type = TypeDeclaration(containingType.TypeKind, containingType.Identifier)
                .WithModifiers(containingType.Modifiers)
                .AddMembers(wrappedMember is not null ? new[] { wrappedMember } : members);
            if (!addedUnsafe)
            {
                type = type.WithModifiers(type.Modifiers.AddToModifiers(SyntaxKind.UnsafeKeyword));
            }
            if (containingType.TypeParameters is not null)
            {
                type = type.AddTypeParameterListParameters(containingType.TypeParameters.Parameters.ToArray());
            }
            wrappedMember = type;
        }
        if (ContainingNamespace is not null)
        {
            wrappedMember = NamespaceDeclaration(ParseName(ContainingNamespace)).AddMembers(wrappedMember);
        }
        return wrappedMember;
    }
}
