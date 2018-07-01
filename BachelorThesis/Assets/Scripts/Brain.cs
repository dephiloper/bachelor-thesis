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
    public float Score { get; set; }
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
        Network.AddLayer(new BasicLayer(new ActivationLinear(), false, 4));
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
            if (Random.value < TrainManager.Instance.MutationRate)
                Network.Flat.Weights[i] = Mathf.PerlinNoise((float) Network.Flat.Weights[i], 0.0f);
    }

    public Brain[] OnePointCrossover(Brain partner)
    {
        var weights = Network.Flat.Weights;
            var partnerWeights = partner.Network.Flat.Weights;

        var midpoint = Random.value * weights.Length;
        // creates two children with the DNA of each of the parents
        var childOne = (double[]) weights.Clone();
        var childTwo = (double[]) weights.Clone();
        if (Random.value < TrainManager.Instance.CrossoverProbabilty)
        {
            for (var i = 0; i < weights.Length; i++)
            {
                if (i < midpoint)
                    childOne[i] = partnerWeights[i];
                else
                    childTwo[i] = partnerWeights[i];
            }
        }

        return new[] {new Brain(childOne), new Brain(childTwo)};
    }
    
    public Brain[] UniformCrossover(Brain partner)
    {
        var weights = Network.Flat.Weights;
        var partnerWeights = partner.Network.Flat.Weights;

        // creates two children with the DNA of each of the parents
        var childOne = (double[]) weights.Clone();
        var childTwo = (double[]) weights.Clone();
        if (Random.value < TrainManager.Instance.CrossoverProbabilty)
        {
            for (var i = 0; i < weights.Length; i++)
            {
                if (Random.value < TrainManager.Instance.UniformCrossoverProbability)
                    childOne[i] = partnerWeights[i];
                else
                    childTwo[i] = partnerWeights[i];
            }
        }

        return new[] {new Brain(childOne), new Brain(childTwo)};
    }

    public void Save(int generation, float maxLifespan, string path = "./Assets/Exports/")
    {
        Directory.CreateDirectory(path);
        var serializableBrain = new SerializableBrain(this, generation, (int)maxLifespan);
        var jsonBrain = JsonUtility.ToJson(serializableBrain);
        path = $"{path}{DateTime.Now:yyyy-MM-dd-HH:mm:ss:ffffff}-gen_{generation}-score_{Score:F}-brain.json";
        File.WriteAllText(path, jsonBrain);
    }

    public static Brain Load(string json)
    {
        var serializableBrain = JsonUtility.FromJson<SerializableBrain>(json);
        var brain = serializableBrain.ToBrain();
        brain.Fitness = 0;
        brain.Score = 0;
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
            _score = Convert.ToInt32(brain.Score);
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