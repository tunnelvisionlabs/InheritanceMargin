// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Tvl.VisualStudio.InheritanceMargin.CSharp
{
    using Microsoft.RestrictedUsage.CSharp.Semantics;

    internal sealed class MemberTarget : IInheritanceTarget
    {
        private readonly CSharpMemberIdentifier _memberIdentifier;

        public MemberTarget(CSharpMemberIdentifier memberIdentifier)
        {
            _memberIdentifier = memberIdentifier;
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
            CSharpInheritanceAnalyzer.NavigateToMember(_memberIdentifier);
        }
    }
}
