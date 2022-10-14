#pragma warning disable CS8604 // Possible null reference argument.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using Microsoft.Interop;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Simple.CodeGen
{
    [Generator]
    public class JavaImportGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // TODO parse .jar files for changes type information https://github.com/xamarin/java.interop/tree/main/src/Xamarin.Android.Tools.Bytecode
            // TODO watch .jar files for changes via FileSystemWatcher

            var attributedMethods = context.SyntaxProvider
                .CreateSyntaxProvider(
                    static (node, ct) => ShouldVisitNode(node),
                    static (context, ct) =>
                    {
                        MethodDeclarationSyntax syntax = (MethodDeclarationSyntax)context.Node;
                        if (context.SemanticModel.GetDeclaredSymbol(syntax, ct) is IMethodSymbol methodSymbol
                            && methodSymbol.GetAttributes().Any(static attribute => attribute.AttributeClass?.ToDisplayString() == Constants.JavaImportAttribute))
                        {
                            return new { Syntax = syntax, Symbol = methodSymbol };
                        }

                        return null;
                    })
                .Where(
                    static modelData => modelData is not null);

            // Validate if attributed methods can have source generated
            IncrementalValueProvider<Compilation> compilationProvider = context.CompilationProvider.Select(static (comp, ct) => comp);
            var methodsWithDiagnostics = attributedMethods.Combine(compilationProvider).Select(static (data, ct) =>
            {
                Diagnostic? diagnostic = GetDiagnosticIfInvalidMethodForGeneration(data.Left.Syntax, data.Left.Symbol, data.Right);
                return new { Syntax = data.Left.Syntax, Symbol = data.Left.Symbol, Diagnostic = diagnostic, Compilation = data.Right };
            });

            var methodsToGenerate = methodsWithDiagnostics.Where(static data => data.Diagnostic is null);
            var invalidMethodDiagnostics = methodsWithDiagnostics.Where(static data => data.Diagnostic is not null);

            // Report diagnostics for invalid methods
            context.RegisterSourceOutput(invalidMethodDiagnostics, static (context, invalidMethod) =>
            {
                context.ReportDiagnostic(invalidMethod.Diagnostic);
            });

            IncrementalValuesProvider<(MemberDeclarationSyntax, ImmutableArray<Diagnostic>)> generateSingleStub = methodsToGenerate
                .Select(static (data, ct) =>
                {
                    var containing = new ContainingSyntaxContext(data.Syntax);
                    return new StubGenerationContext(data.Syntax, data.Symbol, data.Compilation, containing);
                })
                .WithTrackingName("diag")
                .Select(static (data, ct) => GenerateSource(data))
                .WithComparer(Comparers.GeneratedSyntax)
                .WithTrackingName("single");

            context.RegisterDiagnostics(generateSingleStub.SelectMany((stubInfo, ct) => stubInfo.Item2));

            context.RegisterConcatenatedSyntaxOutputs(generateSingleStub.Select((data, ct) => data.Item1), "JavaImports.g.cs");
        }


        record StubGenerationContext(MemberDeclarationSyntax Syntax, IMethodSymbol Symbol, Compilation Compilation, ContainingSyntaxContext Containing);

        private static (MemberDeclarationSyntax, ImmutableArray<Diagnostic>) GenerateSource(StubGenerationContext ctx)
        {
            ImmutableArray<Diagnostic> todoDiag = ImmutableArray<Diagnostic>.Empty;
            var returnType = ctx.Symbol.ReturnType.ToString();
            var method = MethodDeclaration(IdentifierName(returnType), ctx.Symbol.Name)
                                    .WithModifiers(StripTriviaFromModifiers(ctx.Syntax.Modifiers))
                                    .WithBody(Block(SingletonList<StatementSyntax>(
                                    ReturnStatement(DefaultExpression(PredefinedType(Token(SyntaxKind.LongKeyword)))))));

            var toPrint = ctx.Containing.WrapMemberInContainingSyntaxWithUnsafeModifier(method);

            return (toPrint, todoDiag);
        }

        private static SyntaxTokenList StripTriviaFromModifiers(SyntaxTokenList tokenList)
        {
            SyntaxToken[] strippedTokens = new SyntaxToken[tokenList.Count];
            for (int i = 0; i < tokenList.Count; i++)
            {
                strippedTokens[i] = tokenList[i].WithoutTrivia();
            }
            return new SyntaxTokenList(strippedTokens);
        }

        private static bool ShouldVisitNode(SyntaxNode syntaxNode)
        {
            // We only support C# method declarations.
            if (syntaxNode.Language != LanguageNames.CSharp
                || !syntaxNode.IsKind(SyntaxKind.MethodDeclaration))
            {
                return false;
            }

            // Filter out methods with no attributes early.
            return ((MethodDeclarationSyntax)syntaxNode).AttributeLists.Count > 0;
        }

        private static Diagnostic? GetDiagnosticIfInvalidMethodForGeneration(MethodDeclarationSyntax methodSyntax, IMethodSymbol method, Compilation compilation)
        {
            if (!(compilation.Options is CSharpCompilationOptions { AllowUnsafe: true }))
            {
                return Diagnostic.Create(GeneratorDiagnostics.JSImportRequiresAllowUnsafeBlocks, null);
            }

            // Verify the method has no generic types or defined implementation
            // and is marked static and partial.
            if (methodSyntax.TypeParameterList is not null
                || methodSyntax.Body is not null
                || !methodSyntax.Modifiers.Any(SyntaxKind.StaticKeyword)
                || !methodSyntax.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                return Diagnostic.Create(GeneratorDiagnostics.InvalidImportAttributedMethodSignature, methodSyntax.Identifier.GetLocation(), method.Name);
            }

            // Verify that the types the method is declared in are marked partial.
            for (SyntaxNode? parentNode = methodSyntax.Parent; parentNode is TypeDeclarationSyntax typeDecl; parentNode = parentNode.Parent)
            {
                if (!typeDecl.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    return Diagnostic.Create(GeneratorDiagnostics.InvalidImportAttributedMethodContainingTypeMissingModifiers, methodSyntax.Identifier.GetLocation(), method.Name, typeDecl.Identifier);
                }
            }

            // Verify the method does not have a ref return
            if (method.ReturnsByRef || method.ReturnsByRefReadonly)
            {
                return Diagnostic.Create(GeneratorDiagnostics.ReturnConfigurationNotSupported, methodSyntax.Identifier.GetLocation(), "ref return", method.ToDisplayString());
            }

            return null;
        }
    }
}

