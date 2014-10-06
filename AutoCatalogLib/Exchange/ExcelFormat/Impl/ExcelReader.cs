using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AutoCatalogLib.Utils;
using NetOffice.ExcelApi;
using NetOffice.ExcelApi.Enums;

namespace AutoCatalogLib.Exchange.ExcelFormat.Impl
{
    public class ExcelReader : IExternalReader
    {
        private static readonly Regex DoubleQuoteRegex = new Regex(@"""{2,}");
        private static readonly Regex LeadingTailingQuoteRegex = new Regex(@"(^"")|(""$)");

        private Application _application;
        private Workbook _workbook;
        private Worksheet _worksheet;
        private readonly IExcelRule _rule;
        private readonly ISource _source;

        private volatile bool _opened;
        private int _maxRowIndex = -1;
        private int _currentRowIndex = -1;

        private string _csvFile;

        public bool IsReading { get; private set; }

        public ExcelReader(ISource source, IExcelRule behavior)
        {
            _source = source;
            _rule = behavior;
            _currentRowIndex = _rule.RowOffset;
            //FilePath = source.File;
        }

        #region IExcelReader members

        //public string FilePath { get; private set; }

        public int RecordsCount
        {
            get { return _maxRowIndex; }
        }

        public void Open()
        {
            if (_application == null)
            {
                _source.Open();
                if (!File.Exists(_source.File))
                    throw new FileNotFoundException();

                _application = new Application();
                _workbook = _application.Workbooks.Open(_source.File);
                _worksheet = _workbook.Worksheets[_rule.WorksheetIndex] as Worksheet;

                if (_worksheet == null)
                    throw new Exception(@"Не удалось открыть рабочий лист Excel");

                if (_rule.CsvMode)
                {
                    _csvFile = SaveAsCsvFile();
                    CloseExcel();
                }

                _maxRowIndex = GetMaxRowIndex();

                _opened = true;
            }
        }

        private string SaveAsCsvFile()
        {
            try
            {
                var tmpFile = Path.GetTempFileName() + ".csv";
                _workbook.SaveAs(tmpFile, -4158);
                return tmpFile;
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                throw ex;
            }
        }

        private int GetMaxRowIndex()
        {
            if (_rule.CsvMode)
            {
                return GetMaxRowIndexCsv();
            }
            else
            {
                return GetMaxRowIndexExcel();
            }
        }

        private void CloseExcel()
        {
            if (_workbook != null)
                lock (_workbook)
                {
                    _workbook.Close(XlSaveAction.xlDoNotSaveChanges);
                    _workbook.Dispose();
                }

            _worksheet = null;
            _workbook = null;
            if (_application != null)
            {
                _application.Dispose();
                _application = null;
            }
        }

        private void CloseCsv()
        {
            if (String.IsNullOrEmpty(_csvFile))
                return;
            
            if(File.Exists(_csvFile))
                File.Delete(_csvFile);
        }

        public void Close()
        {
            _opened = false;

            if (_rule.CsvMode)
            {
                CloseCsv();
            }
            else
            {
                CloseExcel();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ExcelReader()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            Close();
        }

        #endregion

        private int GetMaxRowIndexExcel()
        {
            var columns = _rule.Columns.Select(c => c.Index + _rule.ColumnOffset).ToArray();
            var lastRows = columns.Select(c =>
                _worksheet.Range(GetCellAddress(_rule.RowOffset, c)).End(XlDirection.xlDown).Row);

            return lastRows.Max();
        }

        private int GetMaxRowIndexCsv()
        {
            int rows = 0;
            using (var input = new StreamReader(_csvFile, Encoding.Default))
            {
                while (!input.EndOfStream)
                {
                    ReadNextCsvLine(input);
                    rows++;
                }
            }

            return rows - _rule.RowOffset + 1;
        }

        public IEnumerable<RecordRow> GetRecords()
        {
            if (IsReading)
                throw new Exception(@"Уже читается");

            IsReading = true;

            if (_rule.CsvMode)
            {
                return GetCsvRecords();
            }
            else
            {
                return GetExcelRecords();
            }
        }

        private IEnumerable<RecordRow> GetExcelRecords()
        {
            if (!_opened)
                yield break;

            var columns = _rule.Columns.Where(c => c.Active).ToArray();
            var columnsCount = columns.Length;

            for (_currentRowIndex = _rule.RowOffset; _currentRowIndex <= _maxRowIndex; _currentRowIndex++)
            {
                bool isValidRow = true;
                var record = new RecordRow(columnsCount)
                {
                    Source = _source,
                    SourceArgument = String.Format(@"[{0}:{1},{2}]", _rule.WorksheetIndex, _currentRowIndex, _rule.ColumnOffset)
                };

                for (int index = 0; index < columnsCount; index++)
                {
                    var column = columns[index];
                    var range = _worksheet.Cells[_currentRowIndex, column.Index + _rule.ColumnOffset];
                    var value = (range.Value2 ?? range.Text).ToString();

                    if (column.Required && value.Length == 0)
                    {
                        isValidRow = false;
                        ReportCurrentRowSkip(@"Отсутствует обязательное поле");
                        break;
                    }

                    record[column.Name] = value;
                }

                if (isValidRow)
                    yield return record;
            }
        }

        private static string ReadNextCsvLine(StreamReader reader)
        {
            var line = reader.ReadLine();

            if (line.Count(c => c == '\"') % 2 >= 1)
            {
                while (!reader.EndOfStream)
                {
                    var prepend = reader.ReadLine();
                    line += prepend;

                    if (prepend.Count(c => c == '\"') % 2 >= 1)
                        break;

                    if (line.Length > 4096)
                        throw new Exception(@"Не удалось восстановить разрыв строки");
                }
            }

            return line;
        }

        private IEnumerable<RecordRow> GetCsvRecords()
        {
            var columns = _rule.Columns.Where(c => c.Active).ToArray();
            var columnsCount = columns.Length;
            var lastColumnIndex = columns.Max(c => c.Index) + _rule.ColumnOffset - 1;
            _currentRowIndex = _rule.RowOffset - 1;

            using (var input = new StreamReader(_csvFile, Encoding.Default))
            {
                for (int index = 0; index < _currentRowIndex; index++)
                    ReadNextCsvLine(input);

                RecordRow record = null;

                while (!input.EndOfStream && _opened)
                {
                    bool isValidRow = true;
                    _currentRowIndex++;

                    try
                    {
                        var row = ReadNextCsvLine(input).Split('\t');

                        if (row.Length < lastColumnIndex)
                        {
                            ReportCurrentRowSkip(@"Недостаточно полей");
                            continue;
                        }

                        record = new RecordRow(columnsCount)
                        {
                            Source = _source,
                            SourceArgument = String.Format(@"[{0}:{1},{2}]", _rule.WorksheetIndex, _currentRowIndex, _rule.ColumnOffset)
                        };

                        for (int index = 0; index < columnsCount; index++)
                        {
                            var column = columns[index];
                            var value = row[column.Index + _rule.ColumnOffset - 1];

                            if (value.StartsWith("\""))
                            {
                                value = LeadingTailingQuoteRegex.Replace(value, String.Empty);
                                value = DoubleQuoteRegex.Replace(value, "\"");
                            }

                            if (column.Required && value.Length == 0)
                            {
                                isValidRow = false;
                                ReportCurrentRowSkip(@"Отсутствует обязательное поле");
                                break;
                            }

                            record[column.Name] = value;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Exception(ex);
#if DEBUG
                        throw ex;
#endif
                    }

                    if (isValidRow)
                        yield return record;
                }
            }
        }

        private void ReportCurrentRowSkip(string reason)
        {
            var format = @"Пропущена строка {0} в файле " + _source.File + @": {1}";
            Log.Logger.Info(String.Format(format, _currentRowIndex, reason));
        }

        private static string GetCellAddress(int row, int column)
        {
            // ToDo: костыль!
            var columnLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            return columnLetters[column].ToString() + row;
        }

    }
}