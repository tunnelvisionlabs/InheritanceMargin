// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Microsoft Reciprocal License (MS-RL). See LICENSE in the project root for license information.

namespace Tvl.VisualStudio.InheritanceMargin
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a factory that can create inheritance tags for particular inheritance relations.
    /// </summary>
    public interface IInheritanceTagFactory
    {
        /// <summary>
        /// Create an inheritance tag representing a particular inheritance relation.
        /// </summary>
        /// <param name="glyph">The glyph for this inheritance relation.</param>
        /// <param name="tooltip">The text to display in the tool tip for the glyph.</param>
        /// <param name="targets">A collection of targets of this inheritance relation.</param>
        /// <returns>An <see cref="IInheritanceTag"/> representing the inheritance relation.</returns>
        IInheritanceTag CreateTag(InheritanceGlyph glyph, string tooltip, IEnumerable<IInheritanceTarget> targets);
    }
}
