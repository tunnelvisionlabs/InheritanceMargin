namespace Tvl.VisualStudio.InheritanceMargin
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;

    public class InheritanceParseResultEventArgs : EventArgs
    {
        private readonly ITextSnapshot _snapshot;
        private readonly TimeSpan _elapsedTime;
        private readonly IEnumerable<ITagSpan<IInheritanceTag>> _tags;

        public InheritanceParseResultEventArgs(ITextSnapshot snapshot, TimeSpan elapsedTime, IEnumerable<ITagSpan<IInheritanceTag>> tags)
        {
            this._snapshot = snapshot;
            this._elapsedTime = elapsedTime;
            this._tags = tags;
        }

        public ITextSnapshot Snapshot
        {
            get
            {
                return _snapshot;
            }
        }

        public TimeSpan ElapsedTime
        {
            get
            {
                return _elapsedTime;
            }
        }

        public IEnumerable<ITagSpan<IInheritanceTag>> Tags
        {
            get
            {
                return _tags;
            }
        }
    }
}
