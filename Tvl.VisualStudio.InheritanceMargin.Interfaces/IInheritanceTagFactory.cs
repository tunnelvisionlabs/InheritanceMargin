// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Microsoft Reciprocal License (MS-RL). See LICENSE in the project root for license information.

namespace Tvl.VisualStudio.InheritanceMargin
{
    using System.Collections.Generic;

    public interface IInheritanceTagFactory
    {
        IInheritanceTag CreateTag(InheritanceGlyph glyph, string displayName, IEnumerable<IInheritanceTarget> targets);
    }
}
