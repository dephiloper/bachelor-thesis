using System;
using System.IO;
using System.Linq;
using Encog.Engine.Network.Activation;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training.Propagation.SGD;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Scenes
{
	public class SampleScript : MonoBehaviour {

		// Use this for initialization
		void Start () {
			ItWorks();
		}

		void ItWorks()
		{
			var network2 = new BasicNetwork();
			
			var data = LoadData();
			var label = LoadLabel();
			var network = new BasicNetwork();
			network.AddLayer(new BasicLayer(null, true, 4));
			network.AddLayer(new BasicLayer(new ActivationReLU(), true, 8));
			network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 1));
			network.Structure.FinalizeStructure();
			network.Reset();

			var trainingSet = new BasicMLDataSet(data, label);

			var propagation = new StochasticGradientDescent(network, trainingSet);

			var epoch = 1;

			for (var i = 0; i < 5000; i++)
			{
				propagation.Iteration();
				Console.WriteLine($"Epoch: {epoch} Error: {propagation.Error}");
				epoch++;
			}

			propagation.FinishTraining();


			var weights = network.Flat.Weights;
			var biases = network.Flat.BiasActivation;
            
			foreach(var pair in trainingSet ) {
				var output = network.Compute(pair.Input);
				Debug.Log(pair.Input[0] + "," + pair.Input[1]
				                  + ", actual=" + output[0] + ",ideal=" + pair.Ideal[0]);
			}
		}
		
		private static double[][] LoadData()
		{
			var lines = File.ReadLines("Assets/Scripts/iris.data").ToList();
			var data = new double[lines.Count - 1][];

			for (var i = 0; i < lines.Count - 1; i++)
			{
				var splittedLine = lines[i].Split(',');
				data[i] = new double[splittedLine.Length - 1];
				for (var j = 0; j < splittedLine.Length - 1; j++)
				{
					data[i][j] = double.Parse(splittedLine[j]);
				}
			}

			return data;
		}

		private static double[][] LoadLabel()
		{
			var lines = File.ReadLines("Assets/Scripts/iris.data").ToList();
			var label = new double[lines.Count - 1][];

			for (var i = 0; i < lines.Count - 1; i++)
			{
				var splittedLine = lines[i].Split(',');
				for (var j = 0; j < splittedLine.Length - 1; j++)
				{
					label[i] = new[] {splittedLine[splittedLine.Length - 1].GetLabelValue()};
				}
			}

			return label;
		}
	}
	
	public enum Label
	{
		Setosa,
		Versicolor,
		Virginica
	}

	public static class StringExtensions
	{
		public static double GetLabelValue(this string label)
		{
			switch (label)
			{
				case "Iris-setosa":
					return (double)Label.Setosa / 2;
				case "Iris-versicolor":
					return (double)Label.Versicolor / 2;
				case "Iris-virginica":
					return (double)Label.Virginica / 2;
				default:
					throw new NotSupportedException($"String value not supported: {label}");
			}
		}
	}
}
