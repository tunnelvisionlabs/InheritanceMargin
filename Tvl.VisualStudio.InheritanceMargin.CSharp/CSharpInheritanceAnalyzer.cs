namespace Tvl.VisualStudio.InheritanceMargin.CSharp
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.FindSymbols;
    using Microsoft.CodeAnalysis.Text;
    using Microsoft.VisualStudio.LanguageServices;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Tvl.VisualStudio.OutputWindow.Interfaces;
    using Stopwatch = System.Diagnostics.Stopwatch;
    using StringBuilder = System.Text.StringBuilder;

    internal class CSharpInheritanceAnalyzer : BackgroundParser
    {
        private readonly SVsServiceProvider _serviceProvider;
        private readonly IInheritanceTagFactory _tagFactory;

        public CSharpInheritanceAnalyzer(ITextBuffer textBuffer, TaskScheduler taskScheduler, ITextDocumentFactoryService textDocumentFactoryService, IOutputWindowService outputWindowService, SVsServiceProvider serviceProvider, IInheritanceTagFactory tagFactory)
            : base(textBuffer, taskScheduler, textDocumentFactoryService, outputWindowService)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException("serviceProvider");

            _serviceProvider = serviceProvider;
            _tagFactory = tagFactory;
        }

        public override string Name
        {
            get
            {
                return "C# Inheritance Analyzer";
            }
        }

        private static readonly Lazy<Type> DependentTypeFinder = new Lazy<Type>(() => typeof(SymbolFinder).Assembly.GetType("Microsoft.CodeAnalysis.FindSymbols.DependentTypeFinder"));
        private static readonly Lazy<Func<INamedTypeSymbol, Solution, IImmutableSet<Project>, CancellationToken, Task<IEnumerable<INamedTypeSymbol>>>> FindDerivedClassesAsync
            = new Lazy<Func<INamedTypeSymbol, Solution, IImmutableSet<Project>, CancellationToken, Task<IEnumerable<INamedTypeSymbol>>>>(() => (Func<INamedTypeSymbol, Solution, IImmutableSet<Project>, CancellationToken, Task<IEnumerable<INamedTypeSymbol>>>)Delegate.CreateDelegate(typeof(Func<INamedTypeSymbol, Solution, IImmutableSet<Project>, CancellationToken, Task<IEnumerable<INamedTypeSymbol>>>), DependentTypeFinder.Value.GetMethod("FindDerivedClassesAsync", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)));
        private static readonly Lazy<Func<INamedTypeSymbol, Solution, IImmutableSet<Project>, CancellationToken, Task<IEnumerable<INamedTypeSymbol>>>> FindDerivedInterfacesAsync
            = new Lazy<Func<INamedTypeSymbol, Solution, IImmutableSet<Project>, CancellationToken, Task<IEnumerable<INamedTypeSymbol>>>>(() => (Func<INamedTypeSymbol, Solution, IImmutableSet<Project>, CancellationToken, Task<IEnumerable<INamedTypeSymbol>>>)Delegate.CreateDelegate(typeof(Func<INamedTypeSymbol, Solution, IImmutableSet<Project>, CancellationToken, Task<IEnumerable<INamedTypeSymbol>>>), DependentTypeFinder.Value.GetMethod("FindDerivedInterfacesAsync", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)));
        private static readonly Lazy<Func<INamedTypeSymbol, Solution, IImmutableSet<Project>, CancellationToken, Task<IEnumerable<INamedTypeSymbol>>>> FindImplementingTypesAsync
            = new Lazy<Func<INamedTypeSymbol, Solution, IImmutableSet<Project>, CancellationToken, Task<IEnumerable<INamedTypeSymbol>>>>(() => (Func<INamedTypeSymbol, Solution, IImmutableSet<Project>, CancellationToken, Task<IEnumerable<INamedTypeSymbol>>>)Delegate.CreateDelegate(typeof(Func<INamedTypeSymbol, Solution, IImmutableSet<Project>, CancellationToken, Task<IEnumerable<INamedTypeSymbol>>>), DependentTypeFinder.Value.GetMethod("FindImplementingTypesAsync", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)));

        public static void NavigateToSymbol(SourceTextContainer textContainer, ISymbol symbol, Project project)
        {
            Workspace workspace;
            if (!Workspace.TryGetWorkspace(textContainer, out workspace))
                return;

            VisualStudioWorkspace visualStudioWorkspace = workspace as VisualStudioWorkspace;
            if (visualStudioWorkspace == null)
                return;

            visualStudioWorkspace.TryGoToDefinition(symbol, project, CancellationToken.None);
        }

        protected override void ReParseImpl()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            ITextSnapshot snapshot = TextBuffer.CurrentSnapshot;

            try
            {
                ITextDocument textDocument = TextDocument;
                string fileName = textDocument != null ? textDocument.FilePath : null;
                Document document = snapshot.GetOpenDocumentInCurrentContextWithChanges();
                SourceTextContainer textContainer = document != null ? document.GetTextAsync().Result.Container : null;
                Project project = document != null ? document.Project : null;
                Solution solution = project.Solution;

                List<ITagSpan<IInheritanceTag>> tags = new List<ITagSpan<IInheritanceTag>>();

                if (document != null && !string.IsNullOrEmpty(fileName))
                {
                    SyntaxTree syntaxTree = document.GetSyntaxTreeAsync().Result;
                    SyntaxNode syntaxRoot = syntaxTree.GetRoot();
                    SemanticModel semanticModel = document.GetSemanticModelAsync().Result;
                    Compilation compilation = semanticModel.Compilation;

                    IDictionary<ISymbol, ISet<ISymbol>> interfaceImplementations = new Dictionary<ISymbol, ISet<ISymbol>>();

                    List<CSharpSyntaxNode> allMembers = new List<CSharpSyntaxNode>();
                    IEnumerable<BaseTypeDeclarationSyntax> typeNodes = syntaxRoot.DescendantNodes().OfType<BaseTypeDeclarationSyntax>();
                    foreach (var typeNode in typeNodes)
                    {
                        ISymbol symbol = semanticModel.GetDeclaredSymbol(typeNode);
                        if (symbol == null)
                        {
                            MarkDirty(true);
                            return;
                        }

                        INamedTypeSymbol typeSymbol = symbol as INamedTypeSymbol;
                        if (typeSymbol == null)
                            continue;

                        // get implemented interface symbols
                        foreach (INamedTypeSymbol namedTypeSymbol in typeSymbol.AllInterfaces)
                        {
                            foreach (ISymbol member in namedTypeSymbol.GetMembers())
                            {
                                ISymbol implementation = typeSymbol.FindImplementationForInterfaceMember(member);
                                if (implementation == null || !(implementation.ContainingSymbol.Equals(typeSymbol)))
                                    continue;

                                ISet<ISymbol> symbols;
                                if (!interfaceImplementations.TryGetValue(implementation, out symbols))
                                {
                                    symbols = new HashSet<ISymbol>();
                                    interfaceImplementations[implementation] = symbols;
                                }

                                symbols.Add(member);
                            }
                        }

                        TypeDeclarationSyntax typeDeclarationSyntax = typeNode as TypeDeclarationSyntax;
                        if (typeDeclarationSyntax != null)
                            allMembers.AddRange(typeDeclarationSyntax.Members);

                        if (typeSymbol.IsSealed)
                            continue;

                        // types which implement or derive from this type
                        ISet<ITypeSymbol> derivedTypes = new HashSet<ITypeSymbol>();
                        derivedTypes.UnionWith(FindDerivedClassesAsync.Value(typeSymbol, solution, null, CancellationToken.None).Result);
                        derivedTypes.UnionWith(FindDerivedInterfacesAsync.Value(typeSymbol, solution, null, CancellationToken.None).Result);
                        derivedTypes.UnionWith(FindImplementingTypesAsync.Value(typeSymbol, solution, null, CancellationToken.None).Result);

                        if (derivedTypes.Count == 0)
                            continue;

                        StringBuilder builder = new StringBuilder();
                        string elementKindDisplayName =
                            "types";

                        builder.AppendLine("Derived " + elementKindDisplayName + ":");
                        foreach (var derived in derivedTypes)
                            builder.AppendLine("    " + derived.ToString());

                        SyntaxToken identifier = typeNode.Accept(IdentifierSyntaxVisitor.Instance);
                        SnapshotSpan span = new SnapshotSpan(snapshot, new Span(identifier.SpanStart, identifier.Span.Length));

                        InheritanceGlyph tag = typeSymbol.TypeKind == TypeKind.Interface ? InheritanceGlyph.HasImplementations : InheritanceGlyph.Overridden;

                        var targets = derivedTypes.Select(i => new TypeTarget(textContainer, i, solution));
                        tags.Add(new TagSpan<IInheritanceTag>(span, _tagFactory.CreateTag(tag, builder.ToString().TrimEnd(), targets)));
                    }

                    foreach (var eventFieldDeclarationSyntax in allMembers.OfType<EventFieldDeclarationSyntax>().ToArray())
                        allMembers.AddRange(eventFieldDeclarationSyntax.Declaration.Variables);

                    foreach (CSharpSyntaxNode memberNode in allMembers)
                    {
                        if (!(memberNode is MethodDeclarationSyntax)
                            && !(memberNode is PropertyDeclarationSyntax)
                            && !(memberNode is EventDeclarationSyntax)
                            && !(memberNode is VariableDeclaratorSyntax))
                        {
                            continue;
                        }

                        ISymbol symbol = semanticModel.GetDeclaredSymbol(memberNode);
                        if (symbol == null)
                        {
                            MarkDirty(true);
                            return;
                        }

                        // members which this member implements
                        ISet<ISymbol> implementedMethods = new HashSet<ISymbol>();
                        if (!interfaceImplementations.TryGetValue(symbol, out implementedMethods))
                            implementedMethods = new HashSet<ISymbol>();

                        ISet<ISymbol> overriddenMethods = new HashSet<ISymbol>();

                        IMethodSymbol methodSymbol = symbol as IMethodSymbol;
                        if (methodSymbol != null)
                        {

                            // methods which this method overrides
                            for (IMethodSymbol current = methodSymbol.OverriddenMethod;
                            current != null;
                            current = current.OverriddenMethod)
                            {
                                overriddenMethods.Add(current);
                            }
                        }
                        else
                        {
                            IPropertySymbol propertySymbol = symbol as IPropertySymbol;
                            if (propertySymbol != null)
                            {
                                // properties which this property overrides
                                for (IPropertySymbol current = propertySymbol.OverriddenProperty;
                                current != null;
                                current = current.OverriddenProperty)
                                {
                                    overriddenMethods.Add(current);
                                }
                            }
                            else
                            {
                                IEventSymbol eventSymbol = symbol as IEventSymbol;
                                if (eventSymbol != null)
                                {
                                    // events which this event overrides
                                    for (IEventSymbol current = eventSymbol.OverriddenEvent;
                                    current != null;
                                    current = current.OverriddenEvent)
                                    {
                                        overriddenMethods.Add(current);
                                    }
                                }
                            }
                        }

                        ISet<ISymbol> implementingMethods = new HashSet<ISymbol>(SymbolFinder.FindImplementationsAsync(symbol, solution).Result);
                        
                        ISet<ISymbol> overridingMethods = new HashSet<ISymbol>(SymbolFinder.FindOverridesAsync(symbol, solution).Result);

                        if (implementingMethods.Count == 0 && implementedMethods.Count == 0 && overriddenMethods.Count == 0 && overridingMethods.Count == 0)
                            continue;

                        StringBuilder builder = new StringBuilder();
                        string elementKindDisplayName =
                            symbol is IPropertySymbol ? "properties" :
                            symbol is IEventSymbol ? "events" :
                            "methods";

                        if (implementedMethods.Count > 0)
                        {
                            builder.AppendLine("Implemented " + elementKindDisplayName + ":");
                            foreach (var methodId in implementedMethods)
                                builder.AppendLine("    " + methodId.ToString());
                        }

                        if (overriddenMethods.Count > 0)
                        {
                            builder.AppendLine("Overridden " + elementKindDisplayName + ":");
                            foreach (var methodId in overriddenMethods)
                                builder.AppendLine("    " + methodId.ToString());
                        }

                        if (implementingMethods.Count > 0)
                        {
                            builder.AppendLine("Implementing " + elementKindDisplayName + " in derived types:");
                            foreach (var methodId in implementingMethods)
                                builder.AppendLine("    " + methodId.ToString());
                        }

                        if (overridingMethods.Count > 0)
                        {
                            builder.AppendLine("Overriding " + elementKindDisplayName + " in derived types:");
                            foreach (var methodId in overridingMethods)
                                builder.AppendLine("    " + methodId.ToString());
                        }

                        SyntaxToken identifier = memberNode.Accept(IdentifierSyntaxVisitor.Instance);
                        SnapshotSpan span = new SnapshotSpan(snapshot, new Span(identifier.SpanStart, identifier.Span.Length));

                        InheritanceGlyph tag;
                        if (implementedMethods.Count > 0)
                        {
                            if (overridingMethods.Count > 0)
                                tag = InheritanceGlyph.ImplementsAndOverridden;
                            else if (implementingMethods.Count > 0)
                                tag = InheritanceGlyph.ImplementsAndHasImplementations;
                            else
                                tag = InheritanceGlyph.Implements;
                        }
                        else if (implementingMethods.Count > 0)
                        {
                            tag = InheritanceGlyph.HasImplementations;
                        }
                        else if (overriddenMethods.Count > 0)
                        {
                            if (overridingMethods.Count > 0)
                                tag = InheritanceGlyph.OverridesAndOverridden;
                            else
                                tag = InheritanceGlyph.Overrides;
                        }
                        else
                        {
                            tag = InheritanceGlyph.Overridden;
                        }

                        List<ISymbol> members = new List<ISymbol>();
                        members.AddRange(implementedMethods);
                        members.AddRange(overriddenMethods);
                        members.AddRange(implementingMethods);
                        members.AddRange(overridingMethods);

                        var targets = members.Select(i => new MemberTarget(textContainer, i, project, solution));
                        tags.Add(new TagSpan<IInheritanceTag>(span, _tagFactory.CreateTag(tag, builder.ToString().TrimEnd(), targets)));
                    }
                }

                InheritanceParseResultEventArgs result = new InheritanceParseResultEventArgs(snapshot, stopwatch.Elapsed, tags);
                OnParseComplete(result);
            }
            catch (InvalidOperationException)
            {
                base.MarkDirty(true);
                throw;
            }
        }

        private class IdentifierSyntaxVisitor : CSharpSyntaxVisitor<SyntaxToken>
        {
            public static readonly IdentifierSyntaxVisitor Instance = new IdentifierSyntaxVisitor();

            private IdentifierSyntaxVisitor()
            {
            }

            public override SyntaxToken VisitClassDeclaration(ClassDeclarationSyntax node)
            {
                return node.Identifier;
            }

            public override SyntaxToken VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
            {
                return node.Identifier;
            }

            public override SyntaxToken VisitDelegateDeclaration(DelegateDeclarationSyntax node)
            {
                return node.Identifier;
            }

            public override SyntaxToken VisitDestructorDeclaration(DestructorDeclarationSyntax node)
            {
                return node.Identifier;
            }

            public override SyntaxToken VisitEnumDeclaration(EnumDeclarationSyntax node)
            {
                return node.Identifier;
            }

            public override SyntaxToken VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
            {
                return node.Identifier;
            }

            public override SyntaxToken VisitEventDeclaration(EventDeclarationSyntax node)
            {
                return node.Identifier;
            }

            public override SyntaxToken VisitIndexerDeclaration(IndexerDeclarationSyntax node)
            {
                return node.ThisKeyword;
            }

            public override SyntaxToken VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
            {
                return node.Identifier;
            }

            public override SyntaxToken VisitMethodDeclaration(MethodDeclarationSyntax node)
            {
                return node.Identifier;
            }

            public override SyntaxToken VisitOperatorDeclaration(OperatorDeclarationSyntax node)
            {
                return node.OperatorToken;
            }

            public override SyntaxToken VisitPropertyDeclaration(PropertyDeclarationSyntax node)
            {
                return node.Identifier;
            }

            public override SyntaxToken VisitStructDeclaration(StructDeclarationSyntax node)
            {
                return node.Identifier;
            }

            public override SyntaxToken VisitVariableDeclarator(VariableDeclaratorSyntax node)
            {
                return node.Identifier;
            }
        }
    }
}
