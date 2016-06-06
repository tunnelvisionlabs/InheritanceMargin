// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Tvl.VisualStudio.InheritanceMargin.CSharp
{
    using Microsoft.RestrictedUsage.CSharp.Semantics;

    internal sealed class TypeTarget : IInheritanceTarget
    {
        private readonly string _displayName;
        private readonly CSharpTypeIdentifier _typeIdentifier;

        public TypeTarget(string displayName, CSharpTypeIdentifier typeIdentifier)
        {
            _displayName = displayName;
            _typeIdentifier = typeIdentifier;
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
            CSharpInheritanceAnalyzer.NavigateToType(_typeIdentifier);
        }
    }
}
