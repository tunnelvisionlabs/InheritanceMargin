// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Tvl.VisualStudio.InheritanceMargin.CSharp
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;

    internal abstract class AbstractTarget : IInheritanceTarget
    {
        private readonly SourceTextContainer _textContainer;
        private readonly ProjectId _projectId;
        private readonly string _displayName;
        private readonly KeyValuePair<SymbolKind, string>[] _symbolPath;

        protected AbstractTarget(SourceTextContainer textContainer, Project currentProject, Solution solution, ISymbol symbol)
        {
            Project project = solution.GetProject(symbol.ContainingAssembly) ?? currentProject;

            _textContainer = textContainer;
            _projectId = project.Id;
            _displayName = symbol.ToString();

            List<KeyValuePair<SymbolKind, string>> symbolPath = new List<KeyValuePair<SymbolKind, string>>();
            for (ISymbol currentSymbol = symbol.OriginalDefinition; currentSymbol != null; currentSymbol = currentSymbol.ContainingSymbol)
            {
                ImmutableArray<IParameterSymbol> parameters = default(ImmutableArray<IParameterSymbol>);
                ImmutableArray<ITypeParameterSymbol> typeParameters = default(ImmutableArray<ITypeParameterSymbol>);
                switch (currentSymbol.Kind)
                {
                case SymbolKind.Method:
                    parameters = ((IMethodSymbol)currentSymbol).Parameters;
                    typeParameters = ((IMethodSymbol)currentSymbol).TypeParameters;
                    break;

                case SymbolKind.Property:
                    parameters = ((IPropertySymbol)currentSymbol).Parameters;
                    break;

                default:
                    break;
                }

                if (!parameters.IsDefaultOrEmpty)
                {
                    for (int i = parameters.Length - 1; i >= 0; i--)
                    {
                        symbolPath.Add(new KeyValuePair<SymbolKind, string>(SymbolKind.Parameter, parameters[i].ToString()));
                    }
                }

                if (currentSymbol.Kind == SymbolKind.Namespace && ((INamespaceSymbol)currentSymbol).IsGlobalNamespace)
                    break;

                if (currentSymbol.Kind == SymbolKind.NetModule || currentSymbol.Kind == SymbolKind.Assembly)
                    break;

                string metadataName = currentSymbol.MetadataName;
                if (currentSymbol.Kind == SymbolKind.Method && !typeParameters.IsDefaultOrEmpty)
                    metadataName = metadataName + '`' + typeParameters.Length;

                symbolPath.Add(new KeyValuePair<SymbolKind, string>(currentSymbol.Kind, metadataName));
            }

            symbolPath.Reverse();
            _symbolPath = symbolPath.ToArray();
        }

        /// <inheritdoc/>
        public string DisplayName
        {
            get
            {
                return _displayName;
            }
        }

        /// <inheritdoc/>
        public void NavigateTo()
        {
            CSharpInheritanceAnalyzer.NavigateToSymbol(_textContainer, _projectId, _symbolPath);
        }
    }
}
