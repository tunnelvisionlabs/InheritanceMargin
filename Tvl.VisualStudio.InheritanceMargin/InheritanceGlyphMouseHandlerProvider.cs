// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Microsoft Reciprocal License (MS-RL). See LICENSE in the project root for license information.

namespace Tvl.VisualStudio.InheritanceMargin
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;

    [ContentType("text")]
    [Name("InheritanceMarkerMouseHandler")]
    [Export(typeof(IGlyphMouseProcessorProvider))]
    [Order]
    internal class InheritanceGlyphMouseHandlerProvider : IGlyphMouseProcessorProvider
    {
        [Import]
        public IViewTagAggregatorFactoryService ViewTagAggregatorFactoryService
        {
            get;
            private set;
        }

        public IMouseProcessor GetAssociatedMouseProcessor(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin margin)
        {
            return new InheritanceGlyphMouseHandler(this, wpfTextViewHost, margin);
        }
    }
}
