﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Tvl.VisualStudio.InheritanceMargin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using _DTE = EnvDTE._DTE;
    using DTE = EnvDTE.DTE;

    internal class CSharpInheritanceTagger : ITagger<IInheritanceTag>
    {
        internal static readonly ITagSpan<IInheritanceTag>[] NoTags = new ITagSpan<IInheritanceTag>[0];
        private static Type _analyzerType;

        private readonly CSharpInheritanceTaggerProvider _provider;
        private readonly ITextBuffer _buffer;
        private readonly IInheritanceParser _analyzer;

        private ITagSpan<IInheritanceTag>[] _tags = NoTags;

        public CSharpInheritanceTagger(CSharpInheritanceTaggerProvider provider, ITextView view, ITextBuffer buffer)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            _provider = provider;
            _buffer = buffer;

            if (_analyzerType == null)
                _analyzerType = LoadAnalyzerType(provider.GlobalServiceProvider);

            _analyzer = (IInheritanceParser)Activator.CreateInstance(_analyzerType, view, buffer, provider.TaskScheduler, provider.TextDocumentFactoryService, provider.OutputWindowService, provider.GlobalServiceProvider, new InheritanceTagFactory());
            _analyzer.ParseComplete += HandleParseComplete;
            _analyzer.RequestParse(false);
        }

        /// <inheritdoc/>
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        internal static CSharpInheritanceTagger CreateInstance(CSharpInheritanceTaggerProvider provider, ITextView view, ITextBuffer buffer)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            return view.Properties.GetOrCreateSingletonProperty(() => new CSharpInheritanceTagger(provider, view, buffer));
        }

        private static Type LoadAnalyzerType(SVsServiceProvider serviceProvider)
        {
            Version version;
            int vsMajorVersion;
            if (Version.TryParse(((DTE)serviceProvider.GetService(typeof(_DTE))).Version, out version))
            {
                vsMajorVersion = version.Major;
            }
            else
            {
                vsMajorVersion = 11;
            }

            bool vs2012 = vsMajorVersion == 11;
            bool vs2015 = vsMajorVersion == 14;
            bool vs2017 = vsMajorVersion == 15;
            bool vs2019 = vsMajorVersion == 16;
            bool vs17 = vsMajorVersion == 17;

            string assemblyName;
            if (vs2012)
                assemblyName = "Tvl.VisualStudio.InheritanceMargin.CSharp.11.0";
            else if (vs2015)
                assemblyName = "Tvl.VisualStudio.InheritanceMargin.CSharp.14.0";
            else if (vs2017)
                assemblyName = "Tvl.VisualStudio.InheritanceMargin.CSharp.15.0";
            else if (vs2019)
                assemblyName = "Tvl.VisualStudio.InheritanceMargin.CSharp.16.0";
            else if (vs17)
                assemblyName = "Tvl.VisualStudio.InheritanceMargin.CSharp.17.0";
            else if (RoslynUtilities.IsRoslynInstalled(serviceProvider))
                assemblyName = "Tvl.VisualStudio.InheritanceMargin.CSharp.Roslyn";
            else
                assemblyName = "Tvl.VisualStudio.InheritanceMargin.CSharp.12.0";

            Assembly assembly = Assembly.Load(assemblyName);
            return assembly.GetType("Tvl.VisualStudio.InheritanceMargin.CSharp.CSharpInheritanceAnalyzer");
        }

        /// <inheritdoc/>
        public IEnumerable<ITagSpan<IInheritanceTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            return _tags;
        }

        protected virtual void HandleParseComplete(object sender, InheritanceParseResultEventArgs e)
        {
            IEnumerable<ITagSpan<IInheritanceTag>> tags = NoTags;

            InheritanceParseResultEventArgs ie = e as InheritanceParseResultEventArgs;
            if (ie != null)
            {
                tags = ie.Tags;
            }

            _tags = tags.ToArray();
            OnTagsChanged(new SnapshotSpanEventArgs(new SnapshotSpan(e.Snapshot, new Span(0, e.Snapshot.Length))));
        }

        protected virtual void OnTagsChanged(SnapshotSpanEventArgs e)
        {
            var t = TagsChanged;
            if (t != null)
                t(this, e);
        }
    }
}
