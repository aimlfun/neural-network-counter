using System.Collections.Generic;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;
using System.Linq;

namespace NeuralNetworkToCount;

/// <summary>
/// Supported "activation" functions for the neuron layer.
/// </summary>
public enum ActivationFunctions { Sigmoid, TanH, ReLU, LeakyReLU, BinaryStep };

/// <summary>
/// Implementation of a feedforward neural network.
/// </summary>
public class NeuralNetwork
{    
    /// <summary>
    /// How many layers of neurons (3+). Do not do 1.
    /// 2 => input connected to output.
    /// 1 => input is output, and feed forward will crash.
    /// </summary>
    internal readonly int[] Layers;

    /// <summary>
    /// The neurons.
    /// [layer][neuron]
    /// </summary>
    internal double[][] Neurons;

    /// <summary>
    /// NN Biases. Either improves or lowers the chance of this neuron fully firing.
    /// [layer][neuron]
    /// </summary>
    private double[][] Biases;

    /// <summary>
    /// NN weights. Reduces or amplifies the output for the relationship between neurons in each layer
    /// [layer][neuron][neuron]
    /// </summary>
    private double[][][] Weights;

    #region BACK PROPAGATION
    /// <summary>
    /// 
    /// </summary>
    private readonly float learningRate = 0.01f;
    #endregion
   
    /// <summary>
    /// 
    /// </summary>
    readonly Dictionary<string, int> inputMapping = new();
    
    /// <summary>
    /// 
    /// </summary>
    readonly Dictionary<string, int> outputMapping = new();
    
    /// <summary>
    /// 
    /// </summary>
    internal static readonly Dictionary<string, NeuralNetwork> networks = new();
    
    /// <summary>
    /// 
    /// </summary>
    internal string Name;

    /// <summary>
    /// Constructor.
    /// TODO remove _id, and use the .length of "s_networks".
    /// </summary>
    /// <param name="_id">Unique ID of the neuron.</param>
    /// <param name="layerDefinition">Defines size of the layers.</param>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable. !! INCORRECT WARNING
    internal NeuralNetwork(int[] layerDefinition, string name, string[] inputs, string[] outputs)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable. !! INTIALISE() METHODS POPULATE THEM
    {
        // (1) INPUT (2) HIDDEN (3) OUTPUT.
        if (layerDefinition.Length < 2) throw new ArgumentException(nameof(layerDefinition) + " insufficient layers.");

        // copy layerDefinition to Layers; although for cars this must not change.     
        Layers = new int[layerDefinition.Length];

        for (int layer = 0; layer < layerDefinition.Length; layer++)
        {
            Layers[layer] = layerDefinition[layer];
        }

        Name = name;

        for (int i = 0; i < inputs.Length; i++) inputMapping.Add($"{name}:INPUT:{inputs[i]}", i);
        for (int i = 0; i < outputs.Length; i++) outputMapping.Add($"{name}:OUTPUT:{outputs[i]}", i);

        networks.Add(name, this);

        InitialiseNeurons();
        InitialiseBiases();
        InitialiseWeights();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inpOut"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    /// <exception cref="ArgumentException"></exception>
    internal static double GetValue(string inpOut)
    {
        string[] tokens = inpOut.Split(":");
        string name = tokens[0];

        if (!networks.ContainsKey(name)) throw new Exception("network not found");
        if (tokens.Length != 3) throw new ArgumentException("name:output:field", nameof(inpOut));

        NeuralNetwork n = networks[name];

        if(tokens[1] != "OUTPUT") throw new Exception("output not found");

        int outputIndex = n.outputMapping[inpOut];

        return Math.Round(n.Neurons[^1][outputIndex]);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <param name="value"></param>
    /// <exception cref="Exception"></exception>
    /// <exception cref="ArgumentException"></exception>
    internal static void SetValue(string input, double value)
    {
        string[] tokens = input.Split(":");
        string name = tokens[0];

        if (!networks.ContainsKey(name)) throw new Exception("network not found");
        if (tokens.Length != 3) throw new ArgumentException("name:input:field", nameof(input));

        NeuralNetwork n = networks[name];

        if (tokens[1] != "INPUT") throw new Exception("input not found");

        int inputIndex = n.inputMapping[input];

        n.Neurons[0][inputIndex] = Math.Round(value);
    }

    #region ACTIVATION / DERIVATIVE FUNCTIONS

    /// <summary>
    /// Tanh squashes a real-valued number to the range [-1, 1]. It’s non-linear. 
    /// But unlike Sigmoid, its output is zero-centered. Therefore, in practice the tanh non-linearity is always preferred 
    /// to the sigmoid nonlinearity.
    /// 
    /// Activate is TANH      ?   1_       ___
    /// (hyperbolic tangent)     0_      /
    ///                         -1_  ___/
    ///                                | | |
    ///                     -infinity -2 0 2..infinity
    ///                               
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static double TanHActivationFunction(double value)
    {
        return (double)Math.Tanh(value);
    }

    /// <summary>
    /// Derivative (for back-propagation of TanH activation function).
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static double DerivativeOfTanHActivationFunction(double value)
    {
        return 1 - (value * value);
    }
    #endregion

    /// <summary>
    /// Create empty storage array for the neurons in the network.
    /// </summary>
    private void InitialiseNeurons()
    {
        List<double[]> neuronsList = new();

        // if layerDefinition is [2,3,2] ..   float[]
        // Neurons :      (o) (o)    <-2  ... [ 0, 0 ]
        //              (o) (o) (o)  <-3  ... [ 0, 0, 0 ]
        //                (o) (o)    <-2  ... [ 0, 0 ]
        //

        for (int layer = 0; layer < Layers.Length; layer++)
        {
            neuronsList.Add(new double[Layers[layer]]);
        }

        Neurons = neuronsList.ToArray();
    }

    /// <summary>
    /// Generate a cryptographic random number between -0.5...+0.5.
    /// </summary>
    /// <returns></returns>
    private static float RandomFloatBetweenMinusHalfToPlusHalf()
    {
        return (float)(RandomNumberGenerator.GetInt32(0, 10000) - 5000) / 10000;
    }

    /// <summary>
    /// initializes and populates biases.
    /// </summary>
    private void InitialiseBiases()
    {
        List<double[]> biasList = new();

        // for each layer of neurons, we have to set biases.
        for (int layer = 1; layer < Layers.Length; layer++)
        {
            double[] bias = new double[Layers[layer]];

            for (int biasLayer = 0; biasLayer < Layers[layer]; biasLayer++)
            {
                bias[biasLayer] = RandomFloatBetweenMinusHalfToPlusHalf();
            }

            biasList.Add(bias);
        }

        Biases = biasList.ToArray();
    }

    /// <summary>
    /// initializes random array for the weights being held in the network.
    /// </summary>
    private void InitialiseWeights()
    {
        List<double[][]> weightsList = new(); // used to construct weights, as dynamic arrays aren't supported

        for (int layer = 1; layer < Layers.Length; layer++)
        {
            List<double[]> layerWeightsList = new();

            int neuronsInPreviousLayer = Layers[layer - 1];

            for (int neuronIndexInLayer = 0; neuronIndexInLayer < Neurons[layer].Length; neuronIndexInLayer++)
            {
                double[] neuronWeights = new double[neuronsInPreviousLayer];

                for (int neuronIndexInPreviousLayer = 0; neuronIndexInPreviousLayer < neuronsInPreviousLayer; neuronIndexInPreviousLayer++)
                {
                    neuronWeights[neuronIndexInPreviousLayer] = RandomFloatBetweenMinusHalfToPlusHalf();
                }

                layerWeightsList.Add(neuronWeights);
            }

            weightsList.Add(layerWeightsList.ToArray());
        }

        Weights = weightsList.ToArray();
    }

    /// <summary>
    /// Feed forward, inputs >==> outputs.
    /// </summary>
    /// <param name="inputs"></param>
    /// <returns></returns>
    internal double[] FeedForward(double[]? inputs = null)
    {
        // no inputs? Assumes you fed inputs in via another mechanism
        if (inputs != null)
        {
            // put the INPUT values into layer 0 neurons
            for (int i = 0; i < inputs.Length; i++)
            {
                Neurons[0][i] = inputs[i];
            }
        }

        // we start on layer 1 as we are computing values from prior layers (layer 0 is inputs)

        for (int layer = 1; layer < Layers.Length; layer++)
        {
            for (int neuronIndexForLayer = 0; neuronIndexForLayer < Layers[layer]; neuronIndexForLayer++)
            {
                // sum of outputs from the previous layer
                double value = 0f;

                for (int neuronIndexInPreviousLayer = 0; neuronIndexInPreviousLayer < Layers[layer - 1]; neuronIndexInPreviousLayer++)
                {
                    // remember: the "weight" amplifies or reduces, so we take the output of the prior neuron and "amplify/reduce" it's output here
                    value += Weights[layer - 1][neuronIndexForLayer][neuronIndexInPreviousLayer] * Neurons[layer - 1][neuronIndexInPreviousLayer];
                }

                // any neuron fires or not based on the input. The point of a bias is to move the activation up or down.
                // e.g. the value could be 0.3, adding a bias of 0.5 takes it to 0.8. You might think why not just use the weights to achieve this
                // but remember weights are individual per prior layer neurons, the bias affects the SUM() of them.

                Neurons[layer][neuronIndexForLayer] = TanHActivationFunction(value + Biases[layer - 1][neuronIndexForLayer]);
            }
        }

        return Neurons[^1]; // final* layer contains OUTPUT
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputs"></param>
    /// <param name="expected"></param>
    public void BackPropagate(double[] inputs, double[] expected)
    {
        double[] output = FeedForward(inputs); //runs feed forward to ensure neurons are populated correctly

        double[][] gamma;

        List<double[]> gammaList = new();

        for (int i = 0; i < Layers.Length; i++)
        {
            gammaList.Add(new double[Layers[i]]);
        }

        gamma = gammaList.ToArray(); // gamma initialization

        int layer = Layers.Length - 2;

        for (int i = 0; i < output.Length; i++)
        {
            gamma[Layers.Length - 1][i] = (output[i] - expected[i]) * DerivativeOfTanHActivationFunction(output[i]); // Gamma calculation
        }

        for (int i = 0; i < Layers[^1]; i++) // calculates the w' and b' for the last layer in the network
        {
            Biases[Layers.Length - 2][i] -= gamma[Layers.Length - 1][i] * learningRate;

            for (int j = 0; j < Layers[^2]; j++)
            {
                Weights[Layers.Length - 2][i][j] -= gamma[Layers.Length - 1][i] * Neurons[Layers.Length - 2][j] * learningRate; //*learning 
            }
        }

        for (int i = Layers.Length - 2; i > 0; i--) // runs on all hidden layers
        {
            for (int j = 0; j < Layers[i]; j++) // outputs
            {
                gamma[i][j] = 0;

                for (int k = 0; k < gamma[i + 1].Length; k++)
                {
                    gamma[i][j] += gamma[i + 1][k] * Weights[i][k][j];
                }

                gamma[i][j] *= DerivativeOfTanHActivationFunction(Neurons[i][j]); //calculate gamma
            }

            for (int j = 0; j < Layers[i]; j++) // iterate over outputs of layer
            {
                Biases[i - 1][j] -= gamma[i][j] * learningRate; // modify biases of network

                for (int k = 0; k < Layers[layer]; k++) // iterate over inputs to layer
                {
                    Weights[i - 1][j][k] -= gamma[i][j] * Neurons[i - 1][k] * learningRate; // modify weights of network
                }
            }
        }
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="trainingData"></param>
    /// <param name="maxError"></param>
    /// <param name="maxAttempts"></param>
    /// <returns></returns>
    internal bool Train(TrainingData[] trainingData, double maxError, int maxAttempts, int checkAfter = 100)
    {
        bool trained = true;

        for (int i = 0; i < maxAttempts; i++)
        {
            foreach (TrainingData data in trainingData)
            {
                BackPropagate(data.input, data.output);
            }

            if (i % 1000 == 0) Debug.WriteLine($"epoch {i}");

            // don't do this until we reach a reasonable number of back propagations,
            // as it is unlikely to be trained.
            if (i < checkAfter) continue;

            trained = true;

            double error = 0;
            int t = 0;
            foreach (TrainingData data in trainingData)
            {
                double outp = FeedForward(data.input)[0];
                double err = Math.Abs(outp - data.output[0]);
                ++t;
                if (err > maxError)
                {
                    error = err;
                    trained = false;
                    break;
                }
            }

            if (trained)
            {
                Debug.WriteLine($"epoch {i}");
                break;
            }
            else
            {
                if (i % 1000 == 0) Debug.WriteLine($"epoch {i} item {t} error {error}");
            }
        }

        Debug.WriteLine((trained) ? "TRAINED" : "** TRAINING FAILURE **");

        foreach (TrainingData data in trainingData)
        {
            double result = FeedForward(data.input)[0];
            Debug.WriteLine($"{string.Join(" ", data.input)} = {result:#.###} Error: {Math.Abs(result - data.output[0]):#.###}");
        }

        Debug.WriteLine("");
 
        return trained;
    }
}

/// <summary>
/// 
/// </summary>
internal class TrainingData
{
    internal double[] input;
    internal double[] output;

    internal TrainingData(double[] input, double[] output)
    {
        this.input = input;
        this.output = output;
    }
}