using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkToCount;

/// <summary>
/// Perceptron AND gate implementation.
/// </summary>
internal class AND
{
    /// <summary>
    /// Make a singleton neural network.
    /// </summary>
    private readonly NeuralNetwork networkAND;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="inputs"></param>
    /// <param name="outputs"></param>
    internal AND(string name, string[] inputs, string[] outputs)
    {
        int[] layers = new int[3] { 2, 2, 1 };

        networkAND = new(layers, name, inputs, outputs);

        Train();
    }

    /// <summary>
    /// Train NN to compute "A AND B".
    /// </summary>
    /// <returns></returns>
    private bool Train()
    {
        Debug.WriteLine("------------------");
        Debug.WriteLine("TRAINING: a AND b");
        Debug.WriteLine("------------------");

        bool successFullyTrained = networkAND.Train(trainingData: new TrainingData[] {
                                        new TrainingData(input: new double[] { 1, 1 }, output: new double[] { 1 }),
                                        new TrainingData(input: new double[] { 1, 0 }, output: new double[] { 0 }),
                                        new TrainingData(input: new double[] { 0, 1 }, output: new double[] { 0 }),
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