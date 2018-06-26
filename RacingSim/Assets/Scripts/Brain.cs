using System;
using System.IO;
using Encog.Engine.Network.Activation;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Random = UnityEngine.Random;
using UnityEngine;

public class Brain
{
    public double Fitness { get; set; }
    public int Score { get; set; }
    private BasicNetwork Network { get; set; }

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
        Network.AddLayer(new BasicLayer(null, true, 9));
        Network.AddLayer(new BasicLayer(new ActivationReLU(), true, 18));
        Network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 4));
        Network.Structure.FinalizeStructure();
        Network.Reset();
    }

    public Action Think(Percept percept)
    {
        var mlData = new BasicMLData(percept.ToDoubleArray());
        var result = Network.Compute(mlData);
        return new Action((result as BasicMLData)?.Data);
    }

    public void Mutate()
    {
        for (var i = 0; i < Network.Flat.Weights.Length; i++)
            if (Random.value < TrainManager.Instance.MutationRate) {
                Network.Flat.Weights[i] = Network.Flat.Weights[i] + (Random.value - 0.5f);
            }
    }

    public Brain Crossover(Brain partner)
    {
        var weights = Network.Flat.Weights;
        var partnerWeights = partner.Network.Flat.Weights;
        
        var midpoint = Random.value * partnerWeights.Length;
        
        var childWeights = new double[partnerWeights.Length];
            
        for (var i = 0; i < partnerWeights.Length; i++)
        {
            if (i < midpoint) childWeights[i] = partnerWeights[i];
            else childWeights[i] = weights[i];
        }

        return new Brain(childWeights);
        
    }

    public void Save(int generation, int maxLifespan, string path = "./Assets/Brains/")
    {
        var serializableBrain = new SerializableBrain(this, generation, maxLifespan);
        var jsonBrain =  JsonUtility.ToJson(serializableBrain);
        path = $"{path}{DateTime.Now:yyyy-MM-dd-HH-mm-ss}gen-{generation}-score-{Score}-brain.json";
        File.WriteAllText(path, jsonBrain);
    }
    
    public static Brain Load(string json)
    {
        var serializableBrain = JsonUtility.FromJson<SerializableBrain>(json);
        var brain = serializableBrain.ToBrain();

        return brain;
    }

    private class SerializableBrain
    {
        [SerializeField] private int _score;
        [SerializeField] private double[] _weights;
        [SerializeField] private int _maxLifespan;
        [SerializeField] private int _generation;

        public SerializableBrain(Brain brain, int generation, int maxLifespan)
        {
            _score = brain.Score;
            _weights = brain.Network.Flat.Weights;
            _generation = generation;
            _maxLifespan = maxLifespan;
        }

        public Brain ToBrain()
        {
            var brain = new Brain(_weights);
            brain.Score = _score;

            return brain;
        }
    }
}