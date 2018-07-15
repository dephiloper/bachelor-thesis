using System;
using System.IO;
using Agent.Data;
using Extensions;
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

        private static SequentialModel DefaultModel =>
            new ReaderKerasModel("./Assets/plain_model.json").GetSequentialExecutor();

        private readonly SequentialModel _model;
        private readonly int[] _layers;
        private double[] _weights;

        public Brain(SequentialModel model = null)
        {
            if (!TrainManager.Instance)
            {
                _model = model;
                _layers = _model.GetLayers();
                _weights = _model.GetWeights();
            }
            else
            {
                _model = DefaultModel;
                _weights = model?.GetWeights();
                _layers = _model.GetLayers();

                if (model == null)
                {
                    _weights = _model.GetWeights();
                    for (var i = 0; i < _weights.Length; i++)
                        _weights[i] = Random.value * 2 - 1f;
                }

                _model.SetWeights(_layers, _weights);
            }
        }

        private Brain(double[] weights)
        {
            _model = DefaultModel;
            _layers = _model.GetLayers();
            _weights = weights;
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
            input.SetData(data);
            var result = (Data2D) _model.ExecuteNetwork(input);
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

            return new[] {new Brain(childOne), new Brain(childTwo)};
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

            return new[] {new Brain(childOne), new Brain(childTwo)};
        }

        public void Export(int generation, string path = "./Assets/Exports/")
        {
            Directory.CreateDirectory(path);
            path = $"{path}{DateTime.Now:yyyy-MM-dd-HH_mm-ss-ffffff}-gen_{generation}-score_{Score:F}";
            PersistSequentialModel.SerializeModel(_model, $"{path}-brain.txt");
        }

        public static Brain Import(string path)
        {
            SequentialModel model = null;
            Brain brain = null;

            if (path.EndsWith(".json"))
                model = new ReaderKerasModel(path).GetSequentialExecutor();
            else if (path.EndsWith(".txt"))
                model = PersistSequentialModel.DeserializeModel(path);

            if (model != null)
                brain = new Brain(model) {_weights = model.GetWeights()};

            return brain;
        }
    }
}