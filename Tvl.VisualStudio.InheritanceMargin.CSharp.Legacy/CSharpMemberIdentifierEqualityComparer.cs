// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Tvl.VisualStudio.InheritanceMargin.CSharp
{
    using System.Collections.Generic;
    using Microsoft.RestrictedUsage.CSharp.Semantics;

    internal class CSharpMemberIdentifierEqualityComparer : IEqualityComparer<CSharpMemberIdentifier>
    {
        /// <inheritdoc/>
        public bool Equals(CSharpMemberIdentifier left, CSharpMemberIdentifier right)
        {
            if (object.ReferenceEquals(left, right))
                return true;

            if (object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null))
                return false;

            return left.Equals(right);
        }

        /// <inheritdoc/>
        public int GetHashCode(CSharpMemberIdentifier identifier)
        {
            return identifier.GetHashCode();
        }
    }
}
