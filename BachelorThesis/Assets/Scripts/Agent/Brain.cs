using System;
using System.IO;
using Agent.Data;
using Encog.Engine.Network.Activation;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Train;
using UnityEngine;
using Action = Agent.Data.Action;
using Random = UnityEngine.Random;

namespace Agent
{
    public class Brain
    {
        public double Fitness { get; set; }
        public float Score { get; set; }
        
        private static readonly int[] DefaultLayer = {24, 13, 2};
        private BasicNetwork Network { get; set; }

        public Brain(params int[] layer)
        {
            if (layer.Length == 0)
                layer = DefaultLayer;
            Setup(layer);
        }

        private Brain(double[] weights, params int[] layer) : this(layer)
        {
            Network.Flat.Weights = weights;
        }

        private void Setup(params int[] layer)
        {
            Network = new BasicNetwork();

            // Input Layer
            Network.AddLayer(new BasicLayer(null, true, layer[0]));

            // Hidden Layer
            for (var i = 1; i < layer.Length - 1; i++)
                Network.AddLayer(new BasicLayer(new ActivationReLU(), true, layer[i]));

            // Output Layer
            Network.AddLayer(new BasicLayer(new ActivationTANH(), false, layer[layer.Length - 1]));
            Network.Structure.FinalizeStructure();
            Network.Reset();

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            for (var i = 0; i < Network.Flat.Weights.Length; i++)
                Network.Flat.Weights[i] = Random.Range(-1f, 1f);
#endif
        }

        public Action Think(Percept percept)
        {
            var data = percept.ToDoubleArray();


            if (data.Length != DefaultLayer[0])
                throw new ArgumentOutOfRangeException(nameof(ArgumentOutOfRangeException),
                    $"Percept count should be {DefaultLayer[0]}, but is {data.Length}");
            
            var mlData = new BasicMLData(data);
            var result = Network.Compute(mlData);
            return new Action((result as BasicMLData)?.Data);
        }

        public void Mutate(float mutationRate)
        {
            for (var i = 0; i < Network.Flat.Weights.Length; i++)
                if (Random.value < mutationRate)
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

        public void Export(int generation, float maxLifespan, string path = "./Assets/Exports/")
        {
            Directory.CreateDirectory(path);
            var serializableBrain = new SerializableBrain(this, generation, (int) maxLifespan);
            var jsonBrain = JsonUtility.ToJson(serializableBrain);
            path = $"{path}{DateTime.Now:yyyy-MM-dd-HH_mm-ss-ffffff}-gen_{generation}-score_{Score:F}-brain.json";
            File.WriteAllText(path, jsonBrain);
        }

        public static Brain Import(string json)
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
                _score = (int) brain.Score;
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
}