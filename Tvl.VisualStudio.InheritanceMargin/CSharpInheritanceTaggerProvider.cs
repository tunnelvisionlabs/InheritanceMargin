// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Tvl.VisualStudio.InheritanceMargin
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using IOutputWindowService = Tvl.VisualStudio.OutputWindow.Interfaces.IOutputWindowService;
    using TaskScheduler = System.Threading.Tasks.TaskScheduler;

    [Name("CSharp Inheritance Tagger Provider")]
    [TagType(typeof(IInheritanceTag))]
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("CSharp")]
    internal class CSharpInheritanceTaggerProvider : IViewTaggerProvider
    {
        public CSharpInheritanceTaggerProvider()
        {
            TaskScheduler = TaskScheduler.Default;
        }

        ////[Import(PredefinedTaskSchedulers.BackgroundIntelliSense)]
        public TaskScheduler TaskScheduler
        {
            get;
            private set;
        }

        [Import]
        public ITextDocumentFactoryService TextDocumentFactoryService
        {
            get;
            private set;
        }

        [Import]
        public IOutputWindowService OutputWindowService
        {
            get;
            private set;
        }

        [Import]
        public SVsServiceProvider GlobalServiceProvider
        {
            get;
            private set;
        }

        /// <inheritdoc/>
        public ITagger<T> CreateTagger<T>(ITextView view, ITextBuffer buffer)
            where T : ITag
        {
            if (buffer != null)
                return CSharpInheritanceTagger.CreateInstance(this, view, buffer) as ITagger<T>;

            return null;
        }
    }
}
