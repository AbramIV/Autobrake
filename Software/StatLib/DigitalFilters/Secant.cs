using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatLib.DigitalFilters;

public class Secant : Filter
{
    public readonly RunningAverage runningAverage;
    public readonly double deviation;
    public double average;

    public Secant(int initial, double tolerance = 2, int averageSize = 0, int round = 0)
    {
        runningAverage = new(averageSize > 0 ? averageSize : initial);
        deviation = Solver.GetStandardDeviation(Values.Take(initial))*tolerance;
        average = runningAverage.GetValues(initial, round).Last();
    }

    public override double GetValue(double value, int digits = 0)
    {
        if (Math.Abs(average - value) > deviation)
            return Math.Round(average, digits);

        average = runningAverage.GetValue(value, digits);

        return Math.Round(value, digits);
    }
}
