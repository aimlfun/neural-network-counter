using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NeuralNetworkToCount;

/// <summary>
/// Neural Network vs. of Electronic counter
/// </summary>
internal class Counter
{
    int lastValue = 0;

    /// <summary>
    /// Constructor.
    /// </summary>
    internal Counter()
    {
        _ = new XOR(
            name: "A",
            inputs: new string[] { "JA", "QA" },
            outputs: new string[] { "QA" });

        _ = new XOR(
            name: "B",
            inputs: new string[] { "JB", "QB" },
            outputs: new string[] { "QB" });

        _ = new XOR(
            name: "C",
            inputs: new string[] { "JC", "QC" },
            outputs: new string[] { "QC" });

        _ = new XOR(
            name: "D",
            inputs: new string[] { "JD", "QD" },
            outputs: new string[] { "QD" });

        _ = new AND(
            name: "R",
            inputs: new string[] { "1", "2" },
            outputs: new string[] { "OUTPUT" });

        _ = new AND(
            name: "S",
            inputs: new string[] { "1", "2" },
            outputs: new string[] { "OUTPUT" });

        _ = new XOR(
            name: "E",
            inputs: new string[] { "QA", "DIRECTION" },
            outputs: new string[] { "OUTPUT" });

        _ = new XOR(
            name: "F",
            inputs: new string[] { "QB", "DIRECTION" },
            outputs: new string[] { "OUTPUT" });

        _ = new XOR(
            name: "G",
            inputs: new string[] { "QC", "DIRECTION" },
            outputs: new string[] { "OUTPUT" });
    }

    /// <summary>
    /// Returns the counter reversed from bits to int.
    /// </summary>
    internal int Value
    {
        get
        {
            return lastValue;
        }
    }

    /// <summary>
    /// Updates our neural network.
    /// The effect is rippled down thru each network.
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    internal int Feedback(int direction)
    {
        // A
        NeuralNetwork.SetValue("A:INPUT:JA", 1);
        NeuralNetwork.SetValue("A:INPUT:QA", NeuralNetwork.GetValue("A:OUTPUT:QA"));
        NeuralNetwork.networks["A"].FeedForward();

        NeuralNetwork.SetValue("E:INPUT:QA", NeuralNetwork.GetValue("A:OUTPUT:QA"));
        NeuralNetwork.SetValue("E:INPUT:DIRECTION", direction);
        NeuralNetwork.networks["E"].FeedForward();

        // B
        NeuralNetwork.SetValue("B:INPUT:JB", NeuralNetwork.GetValue("E:OUTPUT:OUTPUT"));
        NeuralNetwork.SetValue("B:INPUT:QB", NeuralNetwork.GetValue("B:OUTPUT:QB"));
        NeuralNetwork.networks["B"].FeedForward();

        NeuralNetwork.SetValue("F:INPUT:QB", NeuralNetwork.GetValue("B:OUTPUT:QB"));
        NeuralNetwork.SetValue("F:INPUT:DIRECTION", direction);
        NeuralNetwork.networks["F"].FeedForward();

        // R
        NeuralNetwork.SetValue("R:INPUT:1", NeuralNetwork.GetValue("E:OUTPUT:OUTPUT"));
        NeuralNetwork.SetValue("R:INPUT:2", NeuralNetwork.GetValue("F:OUTPUT:OUTPUT"));
        NeuralNetwork.networks["R"].FeedForward();

        // C
        NeuralNetwork.SetValue("C:INPUT:JC", NeuralNetwork.GetValue("R:OUTPUT:OUTPUT"));
        NeuralNetwork.SetValue("C:INPUT:QC", NeuralNetwork.GetValue("C:OUTPUT:QC"));
        NeuralNetwork.networks["C"].FeedForward();

        NeuralNetwork.SetValue("G:INPUT:QC", NeuralNetwork.GetValue("C:OUTPUT:QC"));
        NeuralNetwork.SetValue("G:INPUT:DIRECTION", direction);
        NeuralNetwork.networks["G"].FeedForward();

        // S:
        NeuralNetwork.SetValue("S:INPUT:1", NeuralNetwork.GetValue("R:OUTPUT:OUTPUT"));
        NeuralNetwork.SetValue("S:INPUT:2", NeuralNetwork.GetValue("G:OUTPUT:OUTPUT"));
        NeuralNetwork.networks["S"].FeedForward();

        // D
        NeuralNetwork.SetValue("D:INPUT:JD", NeuralNetwork.GetValue("S:OUTPUT:OUTPUT"));
        NeuralNetwork.SetValue("D:INPUT:QD", NeuralNetwork.GetValue("D:OUTPUT:QD"));
        NeuralNetwork.networks["D"].FeedForward();

        double[] output = new double[] { NeuralNetwork.GetValue("D:OUTPUT:QD"),
                                         NeuralNetwork.GetValue("C:OUTPUT:QC"),
                                         NeuralNetwork.GetValue("B:OUTPUT:QB"),
                                         NeuralNetwork.GetValue("A:OUTPUT:QA")};

        lastValue = BinaryToInt(output);

        return lastValue;
    }

    /// <summary>
    /// Converts the binary digits into a decimal number.
    /// </summary>
    /// <param name="output"></param>
    /// <returns></returns>
    private static int BinaryToInt(double[] output)
    {
        double result = 0;

        for (int d = 0; d < output.Length; d++) result += Math.Pow(2, d) * Math.Abs(Math.Round(output[output.Length - d - 1]));

        return (int)result;
    }
}