namespace Tvl.VisualStudio.InheritanceMargin
{
    using System;

    public interface IInheritanceParser
    {
        event EventHandler<InheritanceParseResultEventArgs> ParseComplete;

        void RequestParse(bool forceReparse);
    }
}
