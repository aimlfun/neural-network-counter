using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkToCount;

/// <summary>
/// Perceptron XOR gate implementation.
/// </summary>
internal class XOR
{
    /// <summary>
    /// Make a neural network.
    /// </summary>
    private readonly NeuralNetwork networkXOR;

    /// <summary>
    /// Constructor.
    /// </summary>
    internal XOR(string name, string[] inputs, string[] outputs)
    {
        int[] layers = new int[3] { 2, 2, 1 };

        networkXOR = new(layers, name, inputs, outputs);

        Train();
    }

    /// <summary>
    /// Train NN to compute "A XOR B".
    /// </summary>
    /// <returns></returns>
    private bool Train()
    {
        Debug.WriteLine("------------------");
        Debug.WriteLine("TRAINING: a XOR b");
        Debug.WriteLine("------------------");

        bool successFullyTrained = networkXOR.Train(trainingData: new TrainingData[] {
                                        new TrainingData(input: new double[] { 1, 1 }, output: new double[] { 0 }),
                                        new TrainingData(input: new double[] { 1, 0 }, output: new double[] { 1 }),
                                        new TrainingData(input: new double[] { 0, 1 }, output: new double[] { 1 }),
                                        new TrainingData(input: new double[] { 0, 0 }, output: new double[] { 0 }) },
                                        maxError: 0.01F,
                                        maxAttempts: 2000000);

        if (!successFullyTrained)
        {
            Debug.WriteLine("** TRAINING FAILED **");
            Debugger.Break();
        }

        Debug.WriteLine("------------------\n");

        return successFullyTrained;
    }
}