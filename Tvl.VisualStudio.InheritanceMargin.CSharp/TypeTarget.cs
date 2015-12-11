// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Microsoft Reciprocal License (MS-RL). See LICENSE in the project root for license information.

namespace Tvl.VisualStudio.InheritanceMargin.CSharp
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;

    internal sealed class TypeTarget : IInheritanceTarget
    {
        private readonly SourceTextContainer _textContainer;
        private readonly ISymbol _typeIdentifier;
        private readonly Solution _solution;

        public TypeTarget(SourceTextContainer textContainer, ISymbol typeIdentifier, Solution solution)
        {
            _textContainer = textContainer;
            _typeIdentifier = typeIdentifier;
            _solution = solution;
        }

        /// <inheritdoc/>
        public string DisplayName
        {
            get
            {
                return _typeIdentifier.ToString();
            }
        }

        /// <inheritdoc/>
        public void NavigateTo()
        {
            CSharpInheritanceAnalyzer.NavigateToSymbol(_textContainer, _typeIdentifier, _solution.GetProject(_typeIdentifier.ContainingAssembly));
        }
    }
}
