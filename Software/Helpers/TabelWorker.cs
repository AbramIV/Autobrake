using System.Data;

namespace Helpers;

internal static class TableHelper
{
    public static void ValidateDataSet(DataSet dataSet, params string[] sheetNames)
    {
        List<string> tableNames = new();
        foreach (DataTable table in dataSet.Tables) tableNames.Add(table.TableName);
        foreach (var sheetName in sheetNames)
            if (!tableNames.Contains(sheetName))
                throw new ArgumentException($"Table \"{sheetName}\" not found!");
    }

    public static IEnumerable<double[]> GetRowsAsArrays(DataTable table)
    {
        foreach (DataRow row in table.Rows)
            yield return GetRowAsArray(row);
    }

    public static double[] GetRowAsArray(DataRow row) =>
        Array.ConvertAll<object, double>(row.ItemArray, x => Convert.ToDouble(x));

    public static double[] GetRowAsArray(DataTable table, int index) =>
        Array.ConvertAll<object, double>(table.Rows[index].ItemArray, x => (double)x);

    public static IEnumerable<double[]> GetColumnsAsArrays(DataTable table)
    {
        foreach (DataColumn column in table.Columns)
            yield return GetColumnAsArray(table, column.ColumnName);
    }

    public static double[] GetColumnAsArray(DataTable table, string columnName) =>
        table.Select().Select(row => Convert.ToDouble(row[columnName])).ToArray();

    public static IEnumerable<string> GetTitles(DataTable table)
    {
        foreach (DataColumn column in table.Columns)
            yield return column.ColumnName;
    }

    public static void RemoveEmptyColumns(DataTable table)
    {
        for (int i = table.Columns.Count - 1; i >= 0; i--)
        {
            bool isEmpty = true;
            foreach (DataRow row in table.Rows)
            {
                if (!string.IsNullOrWhiteSpace(row[i].ToString()))
                {
                    isEmpty = false;
                    break;
                }
            }

            if (isEmpty)
            {
                table.Columns.RemoveAt(i);
            }
        }
    }

    public static void FillDataTableWithRowCopies(DataTable dt, int count)
    {
        if (count < 2) return;

        DataRow firstRow = dt.Rows[0];

        for (int i = 0; i < count - 1; i++)
            dt.Rows.Add(firstRow.ItemArray);
    }

    public static DataTable MergeTables(DataTable table1, DataTable table2, string newName)
    {
        List<DataColumn> columns = new();
        var merged = table1.Copy();
        merged.TableName = newName;

        foreach (DataColumn column in table2.Columns)
        {
            var copy = new DataColumn(column.ColumnName)
            {
                DataType = column.DataType,
            };

            columns.Add(copy);
        }

        merged.Columns.AddRange(columns.ToArray());

        for (int i = 0; i < merged.Rows.Count; i++)
            for (int j = table1.Columns.Count; j < merged.Columns.Count; j++)
                merged.Rows[i].SetField(j, Math.Round(Convert.ToDouble(table2.Rows[i].ItemArray[j - table1.Columns.Count]), 2));

        merged.AcceptChanges();

        return merged;
    }
}