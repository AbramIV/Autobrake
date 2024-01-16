using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers;

public class Randomizer
{
    private static readonly Random Generator;

    static Randomizer() => Generator = new Random((int)DateTime.Now.Ticks);

    public static int GetInt(int min, int max) => Generator.Next(min, max);
    public static double GetDouble(int min, int max) => GetInt(min, max) + Generator.NextDouble();

    /// <summary>
    /// Sin wave collection generator.
    /// </summary>
    /// <param name="count">Count of points.</param>
    /// <param name="amplitude">Max amplitude.</param>
    /// <param name="round">Count of numbers after comma.</param>
    /// <returns>Sequence of numbers to build sin wave.</returns>
    public static IEnumerable<double> GetSinWave(int count = 1000, double amplitude = 311, int round = 15)
    {
        return Enumerable.Range(0, count).Select(i => GetSinPoint(i, amplitude, round));
    }

    /// <summary>
    /// Sin point calculate
    /// </summary>
    /// <param name="value">Input value.</param>
    /// <param name="amplitude">Max amplitude.</param>
    /// <param name="round">Count of numbers after comma.</param>
    /// <returns>One point of sin wav.</returns>
    public static double GetSinPoint(double value, double amplitude = 311, int round = 15)
    {
        return Math.Round(Math.Sin(6.28 * value) * amplitude, round);
    }
}
