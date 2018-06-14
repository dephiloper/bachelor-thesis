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
    public long Score { get; set; }

    private const double MutationRate = 0.1f;

    private BasicNetwork _network;

    public Brain()
    {
        Setup();
    }

    private Brain(double[] weights)
    {
        Setup();
        _network.Flat.Weights = weights;
    }

    private void Setup()
    {
        _network = new BasicNetwork();
        _network.AddLayer(new BasicLayer(null, true, 5));
        _network.AddLayer(new BasicLayer(new ActivationReLU(), true, 10));
        _network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 2));
        _network.Structure.FinalizeStructure();
        _network.Reset();
    }

    public double[] Think(double[] environment)
    {
        var mlData = new BasicMLData(environment);
        // TODO: check if environment -1 is okay
        var result = _network.Compute(mlData);
        return (result as BasicMLData)?.Data;
    }

    public void Mutate()
    {
        for (var i = 0; i < _network.Flat.Weights.Length; i++)
            if (Random.value < MutationRate) {
                _network.Flat.Weights[i] = _network.Flat.Weights[i] + (Random.value - 0.5f);
            }
    }

    public Brain Crossover(Brain rightBrain)
    {
        var leftWeights = _network.Flat.Weights;
        var rightWeights = rightBrain._network.Flat.Weights;
        
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