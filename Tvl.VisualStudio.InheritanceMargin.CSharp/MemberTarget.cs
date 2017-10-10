// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Tvl.VisualStudio.InheritanceMargin.CSharp
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Text;

    internal sealed class MemberTarget : AbstractTarget
    {
        public MemberTarget(SourceTextContainer textContainer, Project currentProject, Solution solution, ISymbol symbol)
            : base(textContainer, currentProject, solution, symbol)
        {
        }
    }
}
