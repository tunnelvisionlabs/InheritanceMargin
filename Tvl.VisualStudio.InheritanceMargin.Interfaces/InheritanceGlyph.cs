// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Tvl.VisualStudio.InheritanceMargin
{
    /// <summary>
    /// Identifies the glyph shown in the inheritance margin for a particular inheritance relation.
    /// </summary>
    public enum InheritanceGlyph
    {
        /// <summary>
        /// No inheritance relation is present, or the inheritance relation does not fit into one of the other glyphs.
        /// </summary>
        None,

        /// <summary>
        /// The type or member is implemented elsewhere.
        /// </summary>
        HasImplementations,

        /// <summary>
        /// The member implements an interface member which is defined elsewhere.
        /// </summary>
        Implements,

        /// <summary>
        /// The member implements an interface member which is defined elsewhere, and is also itself implemented
        /// elsewhere.
        /// </summary>
        ImplementsAndHasImplementations,

        /// <summary>
        /// The member implements an interface member, and is also itself overridden elsewhere.
        /// </summary>
        ImplementsAndOverridden,

        /// <summary>
        /// The member is overridden elsewhere.
        /// </summary>
        Overridden,

        /// <summary>
        /// The member overrides a member of a base type.
        /// </summary>
        Overrides,

        /// <summary>
        /// The member overrides a member of a base type, and is also itself overridden elsewhere.
        /// </summary>
        OverridesAndOverridden,
    }
}
