using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoCatalogLib.Exchange;
using AutoCatalogLib.Persist;
using NetOffice.ExcelApi;
using NetOffice.ExcelApi.Enums;

namespace AutoCatalogLib.Business.Invokers
{
    class ExcelSourceInvoker : IExternalSourceInvoker
    {
        private Application _application;

        private readonly List<ISource> _openedSources = new List<ISource>();

        private bool IsApplicationOpened
        {
            get { return _application != null; }
        }

        public void Invoke(ISource source, string arguments)
        {
            if(!IsApplicationOpened)
                OpenApplication();
            
            var book = OpenWorkBook(GetClosestSource(source));
            Navigate(book, arguments);
        }

        public bool IsInvokable(ISource source, string arguments)
        {
            var sourcePath = SourceLocator.GetTargetLocation(source.Location);

            if (SourceLocator.IsLocalPathLocation(source.Location))
                return File.Exists(sourcePath);

            if (SourceLocator.IsFileEntityLocation(source.Location))
            {
                using (var context = new Context())
                {
                    return context.EmbeddedBlobs.Any(b => b.Name == sourcePath);
                }
            }

            throw new NotImplementedException(@"Source location " + source.Location + " not implemented yet.");
        }

        private ISource GetClosestSource(ISource source)
        {
            var closest = _openedSources.FirstOrDefault(s => s.Location == source.Location);
            if (closest != null)
                return closest;

            source.Open();
            _openedSources.Add(source);
            return source;
        }

        private void Navigate(Workbook book, string arguments)
        {
            if (!String.IsNullOrEmpty(arguments))
            {
                var sheet = (Worksheet)book.Worksheets[GetSheetNumber(arguments)];
                sheet.Activate();
                sheet.Rows[GetRowNumber(arguments)].Activate();
            }
        }

        private Workbook OpenWorkBook(ISource source)
        {
            var book = _application.Workbooks.FirstOrDefault(b => b.FullName == source.File);
            if (book == null)
            {
                book = _application.Workbooks.Open(source.File);
                book.BeforeCloseEvent += BookOnBeforeCloseEvent;
            }

            if (_application.WindowState == XlWindowState.xlMinimized)
                _application.WindowState = XlWindowState.xlNormal;

            _application.Visible = true;
            book.Activate();
            return book;
        }

        private void BookOnBeforeCloseEvent(ref bool cancel)
        {
            // ignore. Юзер сам закроет эксель и ныть не будет, я надеюсь.
        }

        private int GetSheetNumber(string argument)
        {
            try
            {
                var token = argument.Substring(1, argument.IndexOf(':') - 1);
                return Int32.Parse(token);
            }
            catch
            {
                return 1;
            }
        }

        private int GetRowNumber(string argument)
        {
            try
            {
                var token = argument.Substring(argument.IndexOf(':') + 1);
                token = token.Substring(0, token.IndexOf(','));
                return Int32.Parse(token);
            }
            catch
            {
                return 1;
            }
        }

        private void OpenApplication()
        {
            _application = new Application();
            _application.WorkbookBeforeCloseEvent += ApplicationOnWorkbookBeforeCloseEvent;
            _application.Visible = true;
        }

        private void ApplicationOnWorkbookBeforeCloseEvent(Workbook wb, ref bool cancel)
        {
            if (_application.Workbooks.Count == 1)
                OnClosed();
        }

        public event EventHandler Closed;

        protected virtual void OnClosed()
        {
            var handler = Closed;

            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            if (_application != null)
            {
                foreach(var book in _application.Workbooks)
                    book.Close(XlSaveAction.xlDoNotSaveChanges);

                _application.Dispose();
                _application = null;

                foreach (var source in _openedSources)
                    source.Close();
            }
        }
    }
}