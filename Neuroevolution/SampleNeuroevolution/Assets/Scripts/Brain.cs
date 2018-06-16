using System;
using Encog.Engine.Network.Activation;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using UnityEngine;
using Random = UnityEngine.Random;


public class Brain
{
    public double Fitness { get; set; }
    public double Score { get; set; }

    private const double MutationRate = 0.1f;

    public BasicNetwork Network { get; private set; }

    public Brain()
    {
        Setup();
    }

    private Brain(double[] weights)
    {
        Setup();
        Network.Flat.Weights = weights;
    }

    private void Setup()
    {
        Network = new BasicNetwork();
        Network.AddLayer(new BasicLayer(null, true, 5));
        Network.AddLayer(new BasicLayer(new ActivationReLU(), true, 10));
        Network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 2));
        Network.Structure.FinalizeStructure();
        Network.Reset();
    }

    public double[] Think(double[] environment)
    {
        var mlData = new BasicMLData(environment);
        // TODO: check if environment -1 is okay
        var result = Network.Compute(mlData);
        return (result as BasicMLData)?.Data;
    }

    public void Mutate()
    {
        for (var i = 0; i < Network.Flat.Weights.Length; i++)
            if (Random.value < MutationRate) {
                Network.Flat.Weights[i] = Network.Flat.Weights[i] + (Random.value - 0.5f);
            }
    }

    public Brain Crossover(Brain rightBrain)
    {
        var leftWeights = Network.Flat.Weights;
        var rightWeights = rightBrain.Network.Flat.Weights;
        
        var midpoint = Random.value * rightWeights.Length;
        
        var childWeights = new double[rightWeights.Length];
            
        for (var i = 0; i < rightWeights.Length; i++)
        {
            if (i < midpoint) childWeights[i] = rightWeights[i];
            else childWeights[i] = leftWeights[i];
        }

        return new Brain(childWeights);
        
    }
}