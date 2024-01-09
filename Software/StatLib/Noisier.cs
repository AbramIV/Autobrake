using Helpers;

namespace StatLib
{
    public class Noisier
    {
        private readonly double[] inputs;

        public Noisier(IEnumerable<double> values)
        {
            inputs = values.ToArray();
        }

        public IEnumerable<double> GetNoise(int periodMax, int periodMin = 1, int first = 0, int last = 0)
        {
            List<double> noise = new();

            double value;
            int period = Randomizer.GetInt(periodMin, periodMax);
            if (last.Equals(0)) last = inputs.Length;

            for (int i = 0; i < last; i++)
            {
                if (i < first)
                {
                    noise.Add(inputs[i]);
                    continue;
                }

                if (i > first) period += period < 0 ? 1 : -1;

                value = inputs[i] - Randomizer.GetInt(0, (int)inputs[i]);

                if (period.Equals(1)) period = Randomizer.GetInt(periodMin, periodMax) * -1;
                if (period.Equals(-1)) period = Randomizer.GetInt(periodMin, periodMax);

                if (period > 0)
                {
                    noise.Add(inputs[i]);
                    continue;
                }

                noise.Add(value);
            }

            return noise;
        }
    }
}
