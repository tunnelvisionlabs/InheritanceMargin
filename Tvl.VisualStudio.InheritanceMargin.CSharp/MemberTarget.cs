// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Microsoft Reciprocal License (MS-RL). See LICENSE in the project root for license information.

namespace Tvl.VisualStudio.InheritanceMargin.CSharp
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;

    internal sealed class MemberTarget : IInheritanceTarget
    {
        private readonly SourceTextContainer _textContainer;
        private readonly ISymbol _memberIdentifier;
        private readonly Project _project;
        private readonly Solution _solution;

        public MemberTarget(SourceTextContainer textContainer, ISymbol memberIdentifier, Project project, Solution solution)
        {
            _textContainer = textContainer;
            _memberIdentifier = memberIdentifier;
            _project = project;
            _solution = solution;
        }

        /// <inheritdoc/>
        public string DisplayName
        {
            get
            {
                return _memberIdentifier.ToString();
            }
        }

        /// <inheritdoc/>
        public void NavigateTo()
        {
            CSharpInheritanceAnalyzer.NavigateToSymbol(_textContainer, _memberIdentifier, _project);
        }
    }
}
