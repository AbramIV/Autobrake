using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatLib.DigitalFilters;

public class Selector : Secant
{
    private double lastValidValue;

    public Selector(int initial, double tolerance = 2, int averageSize = 0) : base(initial, tolerance, averageSize)
    {
        lastValidValue = Values.Take(averageSize > 0 ? averageSize : initial).Last();
    }

    public override double GetValue(double value, int digits = 0)
    {
        if (Math.Abs(value - lastValidValue) >= deviation)
        {
            average = runningAverage.GetValue((lastValidValue + average) / 2);
            lastValidValue = average;
            return Math.Round(average, digits);
        }

        lastValidValue = value;
        average = runningAverage.GetValue(value, digits);

        return Math.Round(value, digits);
    }
}
