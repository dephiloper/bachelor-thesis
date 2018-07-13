using System;
using System.Collections.Generic;
using System.IO;
using Agent.Data;
using Encog.Engine.Network.Activation;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using NNSharp.DataTypes;
using NNSharp.IO;
using Train;
using UnityEngine;
using Action = Agent.Data.Action;
using Random = UnityEngine.Random;
using NNSharp.Models;

namespace Agent
{
    public class Brain
    {
        public double Fitness { get; set; }
        public float Score { get; set; }
        
        private readonly int[] _layers = {24, 13, 2};
        private readonly SequentialModel _model;
        private double[] _weights;
        
        public Brain(string defaultModel = "plain_model.json")
        {
            var reader = new ReaderKerasModel(defaultModel);
            _model = reader.GetSequentialExecutor();
            Setup();
        }

        private Brain(int[] layers, double[] weights)
        {
            _layers = layers;
            _weights = weights;
            _model.SetWeights(_layers, _weights);
        }
        
        private void Setup()
        {
            var weightCount = _model.GetWeightCount(_layers);
            _weights = new double[weightCount];
            
            // randomize initial weights
            for (var i = 0; i < weightCount; i++)
                _weights[i] = Random.Range(-1, 1);
            
            _model.SetWeights(_layers, _weights);
        }

        public Action Think(Percept percept)
        {
            var data = percept.ToDoubleArray();


            if (data.Length != _model.GetInputDimension().c)
                throw new ArgumentOutOfRangeException(nameof(ArgumentOutOfRangeException),
                    $"Percept count should be {_model.GetInputDimension().c}, but is {data.Length}, check if the " +
                    $"input layer in DefaultLayer is set to the right value (currently: {_layers[0]}");
            
            var input = new Data2D(1, 1, _layers[0], 1);
            var result = (Data2D)_model.ExecuteNetwork(input);
            return new Action(result.ToDoubleArray(_layers));
        }

        public void Mutate(float mutationRate)
        {
            for (var i = 0; i < _weights.Length; i++)
                if (Random.value < mutationRate)
                    _weights[i] = Mathf.PerlinNoise((float) _weights[i], 0.0f);
        }

        public Brain[] OnePointCrossover(Brain partner)
        {
            var midpoint = Random.value * _weights.Length;
            // creates two children with the DNA of each of the parents
            var childOne = (double[]) _weights.Clone();
            var childTwo = (double[]) _weights.Clone();
            if (Random.value < TrainManager.Instance.CrossoverProbabilty)
            {
                for (var i = 0; i < _weights.Length; i++)
                {
                    if (i < midpoint)
                        childOne[i] = partner._weights[i];
                    else
                        childTwo[i] = partner._weights[i];
                }
            }

            return new[] {new Brain(_layers, childOne), new Brain(_layers, childTwo)};
        }

        public Brain[] UniformCrossover(Brain partner)
        {
            // creates two children with the DNA of each of the parents
            var childOne = (double[]) _weights.Clone();
            var childTwo = (double[]) _weights.Clone();
            if (Random.value < TrainManager.Instance.CrossoverProbabilty)
            {
                for (var i = 0; i < _weights.Length; i++)
                {
                    if (Random.value < TrainManager.Instance.UniformCrossoverProbability)
                        childOne[i] = partner._weights[i];
                    else
                        childTwo[i] = partner._weights[i];
                }
            }

            return new[] {new Brain(_layers, childOne), new Brain(_layers, childTwo)};
        }

        public void Export(int generation, float maxLifespan, string path = "./Assets/Exports/")
        {
            Directory.CreateDirectory(path);
            var serializableBrain = new SerializableBrain(_layers, _weights);
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
            [SerializeField] 
            private int[] _layers;
            [SerializeField] 
            private double[] _weights;
            
            public SerializableBrain(int[] layers, double[] weights)
            {
                _layers = layers;
                _weights = weights;
            }

            public Brain ToBrain()
            {
                var brain = new Brain(_layers, _weights);
                return brain;
            }
        }
    }
}