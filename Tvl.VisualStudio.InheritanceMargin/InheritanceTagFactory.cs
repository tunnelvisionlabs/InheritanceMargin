// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Microsoft Reciprocal License (MS-RL). See LICENSE in the project root for license information.

namespace Tvl.VisualStudio.InheritanceMargin
{
    using System.Collections.Generic;
    using System.Linq;

    internal class InheritanceTagFactory : IInheritanceTagFactory
    {
        /// <inheritdoc/>
        public IInheritanceTag CreateTag(InheritanceGlyph glyph, string tooltip, IEnumerable<IInheritanceTarget> targets)
        {
            return new InheritanceTag(glyph, tooltip, targets.ToList());
        }
    }
}
