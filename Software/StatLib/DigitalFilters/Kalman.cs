using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatLib.DigitalFilters;

public class Kalman : Filter
{
    public double MeasureVariation { get; set; }
    public double SpeedVariation { get; set; }

    private double Gain;
    private double EstimateVariation;
    private double CurrentEstimate;
    private double LastEstimate;

    public Kalman() { }

    public void AutoTune()
    {
        var stdev = Solver.GetStandardDeviation(Values);
        SetParameters(stdev, 1/stdev);
    }

    public void SetParameters(double measureVariaton, double speedVariation)
    {
        MeasureVariation = measureVariaton;
        EstimateVariation = measureVariaton;
        SpeedVariation = speedVariation;

        CurrentEstimate = 0;
        LastEstimate = 0;
        Gain = 0;
    }

    public override double GetValue(double value, int digits = 0)
    {
        Gain = EstimateVariation / (EstimateVariation + MeasureVariation);
        CurrentEstimate = LastEstimate + Gain * (value - LastEstimate);
        EstimateVariation = (1.0 - Gain) * EstimateVariation + Math.Abs(LastEstimate - CurrentEstimate) * SpeedVariation;
        LastEstimate = CurrentEstimate;

        return Math.Round(CurrentEstimate, digits);
    }
}
