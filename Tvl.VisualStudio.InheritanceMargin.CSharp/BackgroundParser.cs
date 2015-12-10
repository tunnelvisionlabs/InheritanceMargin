namespace Tvl.VisualStudio.InheritanceMargin.CSharp
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.OutputWindow.Interfaces;
    using ErrorHandler = Microsoft.VisualStudio.ErrorHandler;
    using Path = System.IO.Path;
    using Timer = System.Threading.Timer;

    internal abstract class BackgroundParser : IInheritanceParser, IDisposable
    {
        private readonly WeakReference _textBuffer;
        private readonly TaskScheduler _taskScheduler;
        private readonly ITextDocumentFactoryService _textDocumentFactoryService;
        private readonly IOutputWindowService _outputWindowService;
        private readonly string _outputWindowName;
        private readonly Timer _timer;

        private TimeSpan _reparseDelay;
        private DateTimeOffset _lastEdit;
        private bool _dirty;
        private int _parsing;

        public event EventHandler<InheritanceParseResultEventArgs> ParseComplete;

        [Obsolete]
        public BackgroundParser(ITextBuffer textBuffer, ITextDocumentFactoryService textDocumentFactoryService, IOutputWindowService outputWindowService)
            : this(textBuffer, TaskScheduler.Default, textDocumentFactoryService, outputWindowService, PredefinedOutputWindowPanes.TvlDiagnostics)
        {
        }

        public BackgroundParser(ITextBuffer textBuffer, TaskScheduler taskScheduler, ITextDocumentFactoryService textDocumentFactoryService, IOutputWindowService outputWindowService)
            : this(textBuffer, taskScheduler, textDocumentFactoryService, outputWindowService, PredefinedOutputWindowPanes.TvlDiagnostics)
        {
        }

        public BackgroundParser(ITextBuffer textBuffer, TaskScheduler taskScheduler, ITextDocumentFactoryService textDocumentFactoryService, IOutputWindowService outputWindowService, string outputPaneName)
        {
            if (textBuffer == null)
                throw new ArgumentNullException("textBuffer");
            if (taskScheduler == null)
                throw new ArgumentNullException("taskScheduler");
            if (textDocumentFactoryService == null)
                throw new ArgumentNullException("textDocumentFactoryService");
            if (outputWindowService == null)
                throw new ArgumentNullException("outputWindowService");

            this._textBuffer = new WeakReference(textBuffer);
            this._taskScheduler = taskScheduler;
            this._textDocumentFactoryService = textDocumentFactoryService;
            this._outputWindowService = outputWindowService;
            this._outputWindowName = outputPaneName;

            textBuffer.PostChanged += TextBufferPostChanged;

            this._dirty = true;
            this._reparseDelay = TimeSpan.FromMilliseconds(1500);
            this._timer = new Timer(ParseTimerCallback, null, _reparseDelay, _reparseDelay);
            this._lastEdit = DateTimeOffset.MinValue;
        }

        public ITextBuffer TextBuffer
        {
            get
            {
                return (ITextBuffer)_textBuffer.Target;
            }
        }

        public ITextDocument TextDocument
        {
            get
            {
                ITextBuffer textBuffer = TextBuffer;
                if (textBuffer == null)
                    return null;

                ITextDocument textDocument;
                if (!TextDocumentFactoryService.TryGetTextDocument(textBuffer, out textDocument))
                    return null;

                return textDocument;
            }
        }

        public bool Disposed
        {
            get;
            private set;
        }

        public TimeSpan ReparseDelay
        {
            get
            {
                return _reparseDelay;
            }

            set
            {
                TimeSpan originalDelay = _reparseDelay;
                try
                {
                    _reparseDelay = value;
                    _timer.Change(value, value);
                }
                catch (ArgumentException)
                {
                    _reparseDelay = originalDelay;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual string Name
        {
            get
            {
                return string.Empty;
            }
        }

        protected ITextDocumentFactoryService TextDocumentFactoryService
        {
            get
            {
                return _textDocumentFactoryService;
            }
        }

        protected IOutputWindowService OutputWindowService
        {
            get
            {
                return _outputWindowService;
            }
        }

        public void RequestParse(bool forceReparse)
        {
            TryReparse(forceReparse);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ITextBuffer textBuffer = TextBuffer;
                if (textBuffer != null)
                    textBuffer.PostChanged -= TextBufferPostChanged;

                _timer.Dispose();
            }

            Disposed = true;
        }

        protected abstract void ReParseImpl();

        protected virtual void OnParseComplete(InheritanceParseResultEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            var t = ParseComplete;
            if (t != null)
                t(this, e);
        }

        protected void MarkDirty(bool resetTimer)
        {
            this._dirty = true;
            this._lastEdit = DateTimeOffset.Now;

            if (resetTimer)
                _timer.Change(_reparseDelay, _reparseDelay);
        }

        private void TextBufferPostChanged(object sender, EventArgs e)
        {
            MarkDirty(true);
        }

        private void ParseTimerCallback(object state)
        {
            if (TextBuffer == null)
            {
                Dispose();
                return;
            }

            TryReparse(_dirty);
        }

        private void TryReparse(bool forceReparse)
        {
            if (!_dirty && !forceReparse)
                return;

            if (DateTimeOffset.Now - _lastEdit < TimeSpan.FromSeconds(2))
                return;

            if (Interlocked.CompareExchange(ref _parsing, 1, 0) == 0)
            {
                try
                {
                    Task task = Task.Factory.StartNew(ReParse, CancellationToken.None, TaskCreationOptions.None, _taskScheduler);
                    task.ContinueWith(_ => _parsing = 0);
                }
                catch
                {
                    _parsing = 0;
                    throw;
                }
            }
        }

        private void ReParse()
        {
            try
            {
                _dirty = false;

                IOutputWindowPane outputWindow = null;
                if (_outputWindowService != null && !string.IsNullOrEmpty(_outputWindowName))
                    outputWindow = _outputWindowService.TryGetPane(_outputWindowName);

                Stopwatch stopwatch = Stopwatch.StartNew();

                string message = "{0}: Background parse {1}{2} in {3}ms. {4}";
                string name = Name;
                if (!string.IsNullOrEmpty(name))
                    name = "(" + name + ") ";

                string filename = "<Unknown File>";
                ITextDocument textDocument = TextDocument;
                if (textDocument != null)
                {
                    filename = textDocument.FilePath;
                    if (filename != null)
                        filename = filename.Substring(filename.LastIndexOfAny(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }) + 1);
                }

                try
                {
                    ReParseImpl();

                    if (outputWindow != null)
                    {
                        long time = stopwatch.ElapsedMilliseconds;
                        outputWindow.WriteLine(string.Format(message, filename, name, "succeeded", time, string.Empty));
                    }
                }
                catch (Exception e2)
                {
                    if (ErrorHandler.IsCriticalException(e2))
                        throw;

                    try
                    {
                        if (outputWindow != null)
                        {
                            long time = stopwatch.ElapsedMilliseconds;
                            outputWindow.WriteLine(string.Format(message, filename, name, "failed", time, e2.Message + e2.StackTrace));
                        }
                    }
                    catch (Exception e3)
                    {
                        if (ErrorHandler.IsCriticalException(e3))
                            throw;
                    }
                }
            }
            catch (Exception ex)
            {
                if (ErrorHandler.IsCriticalException(ex))
                    throw;
            }
        }
    }
}
