namespace Tvl.VisualStudio.InheritanceMargin
{
    using System;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using ErrorHandler = Microsoft.VisualStudio.ErrorHandler;
    using IVsPackage = Microsoft.VisualStudio.Shell.Interop.IVsPackage;
    using IVsShell = Microsoft.VisualStudio.Shell.Interop.IVsShell;
    using SVsServiceProvider = Microsoft.VisualStudio.Shell.SVsServiceProvider;
    using SVsShell = Microsoft.VisualStudio.Shell.Interop.SVsShell;

    [Name("InheritanceGlyphFactory")]
    [Export(typeof(IGlyphFactoryProvider))]
    [ContentType("text")]
    [TagType(typeof(InheritanceTag))]
    [Order]
    public class InheritanceGlyphFactoryProvider : IGlyphFactoryProvider
    {
        private static bool _packageLoaded;

        [Import]
        private SVsServiceProvider ServiceProvider
        {
            get;
            set;
        }

        #region IGlyphFactoryProvider Members

        public IGlyphFactory GetGlyphFactory(IWpfTextView view, IWpfTextViewMargin margin)
        {
            if (view == null || margin == null)
                return null;

            if (!_packageLoaded)
            {
                IVsShell shell = (IVsShell)ServiceProvider.GetService(typeof(SVsShell));
                Guid guid = typeof(InheritanceMarginPackage).GUID;
                IVsPackage package;
                ErrorHandler.ThrowOnFailure(shell.LoadPackage(ref guid, out package));
                _packageLoaded = true;
            }

            return new InheritanceGlyphFactory(this, view, margin);
        }

        #endregion
    }
}
