using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatLib.DigitalFilters
{
    public class RunningAverage : Filter
    {
        private readonly double[] results;
        private int index = 0;
        private double result;

        public RunningAverage(int size)
        {
            results = new double[size];
        }

        public override double GetValue(double value, int round = 0)
        {
            result += value - results[index];
            results[index] = value;
            index = (index + 1) % results.Length;
            return Math.Round(result / results.Length, round);
        }
    }
}