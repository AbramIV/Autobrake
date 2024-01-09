using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatLib.DigitalFilters
{
    public class Median : Filter
    {
        private readonly int Size;
        private readonly double[] Values;
        private int count;

        public Median(int size)
        {
            Size = size;
            Values = new double[Size];
            count = 0;
        }

        public override double GetValue(double value, int digits = 0)
        {
            Values[count] = value;
            if ((count < Size - 1) && (Values[count] > Values[count + 1]))
            {
                for (int i = count; i < Size - 1; i++)
                {
                    if (Values[i] > Values[i + 1])
                    {
                        (Values[i + 1], Values[i]) = (Values[i], Values[i + 1]);
                    }
                }
            }
            else
            {
                if ((count > 0) && (Values[count - 1] > Values[count]))
                {
                    for (int i = count; i > 0; i--)
                    {
                        if (Values[i] < Values[i - 1])
                        {
                            (Values[i - 1], Values[i]) = (Values[i], Values[i - 1]);
                        }
                    }
                }
            }
            if (++count >= Size) count = 0;
            return Math.Round(Values[Size / 2], digits);
        }
    }
}
