using Microsoft.Office.Interop.Excel;
using LocalTable = System.Data.DataTable;
using System.Diagnostics;
using Range = Microsoft.Office.Interop.Excel.Range;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Data;

namespace LabTestsHandler.Helpers;

internal class ExcelWorker : IDisposable
{
    private readonly Application? _app;
    private Workbook? _book;
    private FileInfo? _file;

    internal ExcelWorker()
    {
        _app = new()
        {
            DisplayAlerts = false,
            Visible = false
        };
    }

    internal void Open(string path)
    {
        if (string.IsNullOrEmpty(path)) throw new ArgumentException("File path is empty.");
        if (!File.Exists(path)) throw new ArgumentException("File doesn't exist.");

        _file = new FileInfo(path);
        _book = _app?.Workbooks.Open(_file.FullName);
    }

    internal DataSet GetDataSet()
    {
        DataSet tables = new()
        {
            DataSetName = Path.GetFileNameWithoutExtension(_file.Name)
        };

        foreach (Worksheet worksheet in _book.Worksheets)
            tables.Tables.Add(GetDataTable(worksheet.Name));

        return tables;
    }

    internal LocalTable GetDataTable(string name)
    {
        Worksheet? worksheet = _book?.Worksheets[char.ToLowerInvariant(name[0]) + name[1..]];

        LocalTable table = new()
        {
            TableName = worksheet?.Name
        };

        for (int c = 1; c <= worksheet?.UsedRange.Columns.Count; c++)
        {
            if (string.IsNullOrEmpty((string)worksheet.UsedRange.Cells[1, c].Value)) break;
            DataColumn column = new((string)worksheet.UsedRange.Cells[1, c].Value);
            table.Columns.Add(column);
        }

        for (int i = 2; i <= worksheet?.UsedRange.Rows.Count; i++)
        {
            var row = table.NewRow();

            for (int j = 1; j <= worksheet.UsedRange.Columns.Count; j++)
            {
                if (string.IsNullOrEmpty((string)worksheet.UsedRange.Cells[1, j].Value)) break;
                row[(string)worksheet.UsedRange.Cells[1, j].Value] =
                worksheet.UsedRange.Cells[i, j].Value;
            }

            table.Rows.Add(row);
        }

        table.AcceptChanges();
        return table;
    }

    internal IEnumerable<string> GetTitles(string sheetName)
    {
        List<string> titles = new();
        Worksheet? worksheet = _book?.Worksheets[sheetName];

        for (int i = 1; i < worksheet.UsedRange.Count; i++)
        {
            if (string.IsNullOrEmpty(worksheet.UsedRange.Cells[1, i].Value)) continue;

            titles.Add(worksheet.UsedRange.Cells[1, i].Value);
        }

        return titles;
    }

    internal void DuplicateSheet(string sheetName, string newSheetName)
    {
        DeleteSheet(newSheetName);

        ((Worksheet?)_book?.Worksheets[sheetName]).Copy(Type.Missing, _book?.Sheets[_book?.Sheets.Count]); // copy
        _book.Sheets[_book.Sheets.Count].Name = newSheetName;
        _book.Save();
    }

    internal void AddColumn(string title, int shift = 0)
    {
        _book.ActiveSheet.Cells[1, _book?.ActiveSheet.UsedRange.Columns.Count + shift + 1] = title;
        _book?.Save();
    }

    internal void FillColumnRowsByFormula(string value, string title = "")
    {
        int i = 2;
        int j;

        if (string.IsNullOrEmpty(title))
        {
            j = _book.ActiveSheet.UsedRange.Columns.Count;

            _book.ActiveSheet.Range[_book.ActiveSheet.Cells[i, j], _book.ActiveSheet.Cells[_book.ActiveSheet.UsedRange.Rows.Count, j]].Formula = value.Replace(",", ".");
        }
        else { }

        _book?.Save();
    }

    internal IEnumerable<string> GetSheetNames()
    {
        if (_book is null) throw new ArgumentNullException("Excel workbook is empty.");
        if (_book?.Worksheets.Count < 1) throw new ArgumentException("Excel has no any sheets.");

        foreach (Worksheet sheet in _book.Worksheets) yield return sheet.Name;
    }

    internal void SetActiveSheet(string sheetName)
    {
        if (!GetSheetNames().Contains(sheetName)) throw new ArgumentException($"File \"{_file.Name}\" doesn't contain sheet \"{sheetName}\".");

        ((Worksheet)_book.Worksheets[sheetName]).Select();
    }

    internal string GetActiveSheetName()
    {
        if (_book.ActiveSheet is null) throw new ArgumentNullException("Active sheet is empty.");

        return ((Worksheet)_book.ActiveSheet).Name;
    }

    internal void WriteCell(int row, int column, string data)
    {
        if (row <= 0) throw new ArgumentOutOfRangeException($"Incorrect row index: {row}");
        if (column <= 0) throw new ArgumentOutOfRangeException($"Incorrect column index: {column}");

        ((Worksheet)_book.ActiveSheet).Cells[row, column] = data;
    }

    internal string ReadCell(int row, int column)
    {
        return (string)_book?.ActiveSheet.Cells[row, column];
    }

    public void DeleteSheet(string sheetName)
    {
        if (GetSheetNames().Contains(sheetName))
        {
            ((Worksheet)_book?.Worksheets[sheetName]).Delete();
            _book.Save();
        }
    }

    public void ImportTable(LocalTable table)
    {
        DeleteSheet(table.TableName);

        _book.Sheets.Add(After: _book.Sheets[_book.Sheets.Count]);
        _book.Sheets[_book.Sheets.Count].Name = table.TableName;
        _book.Sheets[_book.Sheets.Count].Select();

        for (int i = 0; i < table.Columns.Count; i++)
            _book.Sheets[_book.Sheets.Count].Cells[1, i + 1] = table.Columns[i].ColumnName;

        for (int i = 0; i < table.Rows.Count; i++)
            for (int j = 0; j < table.Columns.Count; j++)
                _book.Sheets[_book.Sheets.Count].Cells[i + 2, j + 1] = table.Rows[i][j];

        _book.Save();
    }

    public void FormatRange()
    {
        ((Range)_book.ActiveSheet.UsedRange).NumberFormat = "0.00";
        ((Range)_book.ActiveSheet.UsedRange).VerticalAlignment = XlVAlign.xlVAlignCenter;
        ((Range)_book.ActiveSheet.UsedRange).HorizontalAlignment = XlHAlign.xlHAlignCenter;
        ((Range)_book.ActiveSheet.UsedRange).Borders[XlBordersIndex.xlEdgeLeft].LineStyle = XlLineStyle.xlContinuous;
        ((Range)_book.ActiveSheet.UsedRange).Borders[XlBordersIndex.xlEdgeRight].LineStyle = XlLineStyle.xlContinuous;
        ((Range)_book.ActiveSheet.UsedRange).Borders[XlBordersIndex.xlEdgeTop].LineStyle = XlLineStyle.xlContinuous;
        ((Range)_book.ActiveSheet.UsedRange).Borders[XlBordersIndex.xlEdgeBottom].LineStyle = XlLineStyle.xlContinuous;
        ((Range)_book.ActiveSheet.UsedRange).Borders.Color = Color.Black;
        ((Range)_book.ActiveSheet.UsedRange).BorderAround(Type.Missing, XlBorderWeight.xlThick, XlColorIndex.xlColorIndexAutomatic);
    }

    public void Transpose()
    {

    }

    public void PaintTitles(int startColumn, int count, Color color)
    {
        for (int i = startColumn; i <= startColumn + count; i++)
            _book.ActiveSheet.UsedRange.Cells[1, i].Interior.Color = color;
    }

    public IEnumerable<string> GetCellsValues(int row, int column)
    {
        return new string[1] { "" };
    }

    public static void TerminateExcelProcesses()
    {
        Process.GetProcessesByName("EXCEL").AsParallel().ForAll(p => p.Kill());
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }

    public string GetLitera(string title, int? rowReplacer = null)
    {
        var result = ((Range)_book.ActiveSheet.UsedRange).Find(title, LookAt: XlLookAt.xlWhole);
        var address = result.Address.Replace("$", "").Trim();

        if (rowReplacer < 0) return Regex.Replace(address, @"[\d-]", string.Empty);
        if (rowReplacer is null) return address;
        return Regex.Replace(address, @"[\d-]", rowReplacer.ToString());
    }

    public void Dispose()
    {
        _book?.Close(true);
        _app?.Quit();
        Marshal.ReleaseComObject(_app);
    }
}