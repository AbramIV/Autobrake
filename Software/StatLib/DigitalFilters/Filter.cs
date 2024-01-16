using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatLib.DigitalFilters;

public abstract class Filter
{
    public readonly string Name;
    public static IEnumerable<double>? Values { get; set; }

    public Filter()
    {
        Name = GetType().Name;
    }

    public abstract double GetValue(double value, int digits = 0);

    public IEnumerable<double> GetValues(int digits = 0)
    {
        foreach (double value in Values)
            yield return GetValue(value, digits);
    }

    public IEnumerable<double> GetValues(int index, int digits = 0)
    {
        for (int i=0; i< index; i++)
            yield return GetValue(Values.ElementAt(i), digits);
    }

    public static Dictionary<char, List<double>> DataFromFile(string path, char separator, params char[] literals)
    {
        Dictionary<char, List<double>> data = new();
        string[] rows = File.ReadAllText(path).Split(separator, StringSplitOptions.RemoveEmptyEntries);

        foreach (char literal in literals) data.Add(literal, new List<double>());

        foreach (string row in rows)
        {
            if (data.ContainsKey(row[0]))
            {
                data[row[0]].Add(Convert.ToDouble(row.Replace(row[0].ToString(), "").Trim()));
            }
        }

        return data;
    }

    public static IEnumerable<double> LoadValues(string path, int digits = 0) => 
        File.ReadAllLines(path).Select(v => Math.Round(Convert.ToDouble(v), digits));
}
