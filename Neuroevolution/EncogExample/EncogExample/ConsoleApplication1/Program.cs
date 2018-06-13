using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training.Propagation.Back;
using Encog.Neural.Networks.Training.Propagation.SGD;
using static System.Double;

namespace ConsoleApplication1
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var data = LoadData();
            var label = LoadLabel();
            var network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, 4));
            network.AddLayer(new BasicLayer(new ActivationReLU(), true, 8));
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 3));
            network.Structure.FinalizeStructure();
            network.Reset();

            var trainingSet = new BasicMLDataSet(data, label);

            network.Flat.Weights = LoadNetwork();
            //Train(network, trainingSet);
            //SaveNetwork(network);
            
            var sum = 0;
            
            foreach (var pair in trainingSet)
            {
                var output = network.Compute(pair.Input);
                Console.WriteLine($"Input: {pair.Input} Actual: {output.ToOneHot()} Ideal: {pair.Ideal}");
                if (CompareMLData(output.ToOneHot(), pair.Ideal))
                    sum++;
            }

            Console.WriteLine($"Result = {((float)sum / trainingSet.Count) * 100}");
        }

        private static void Train(BasicNetwork network, BasicMLDataSet trainingSet)
        {
            var propagation = new StochasticGradientDescent(network, trainingSet);

            var epoch = 1;

            for (var i = 0; i < 200; i++)
            {
                propagation.Iteration(1000);
                Console.WriteLine($"Epoch: {epoch} Error: {propagation.Error}");
                epoch++;
            }

            propagation.FinishTraining();
        }

        private static void SaveNetwork(BasicNetwork network)
        {
            using (var writer = new BinaryWriter(File.Open("weights.bin", FileMode.Create)))
            {
                var bytes = GetBytes(network.Flat.Weights);
                writer.Write(bytes.Length);
                writer.Write(bytes);
            }
        }
        
        private static double[] LoadNetwork()
        {
            double[] values;
            
            using (var reader = new BinaryReader(File.Open("weights.bin", FileMode.Open)))
            {
                var bytesToRead = reader.ReadInt32();
                var x = reader.ReadBytes(bytesToRead);
                values = GetDoubles(x);
            }

            return values;
        }

        private static bool CompareMLData(IMLData first, IMLData second)
        {
            var firstData = (first as BasicMLData)?.Data;
            var secondData = (second as BasicMLData)?.Data;

            return firstData.Where((t, i) => Math.Abs(t - secondData[i]) < 0.000001f).Count() == firstData.Length;

        }

        private static double[][] LoadData()
        {
            var lines = File.ReadLines("./iris.data").ToList();
            var data = new double[lines.Count - 1][];

            for (var i = 0; i < lines.Count - 1; i++)
            {
                var splittedLine = lines[i].Split(',');
                data[i] = new double[splittedLine.Length - 1];
                for (var j = 0; j < splittedLine.Length - 1; j++)
                {
                    data[i][j] = Parse(splittedLine[j]);
                }
            }

            return data;
        }

        private static double[][] LoadLabel()
        {
            var lines = File.ReadLines("./iris.data").ToList();
            var label = new double[lines.Count - 1][];

            for (var i = 0; i < lines.Count - 1; i++)
            {
                var splittedLine = lines[i].Split(',');
                for (var j = 0; j < splittedLine.Length - 1; j++)
                {
                    label[i] = splittedLine[splittedLine.Length - 1].GetLabelValue();
                }
            }

            return label;
        }

        private static double[] GetLabelValue(this string label)
        {
            switch (label)
            {
                case "Iris-setosa":
                    return new double[] {1, 0, 0};
                case "Iris-versicolor":
                    return new double[] {0, 1, 0};
                case "Iris-virginica":
                    return new double[] {0, 0, 1};
                default:
                    throw new NotSupportedException($"String value not supported: {label}");
            }
        }

        private static IMLData ToOneHot(this IMLData data)
        {
            if (data is BasicMLData mlData)
            {
                var maxValue = mlData.Data.ToList().Max();
                var maxIndex = mlData.Data.ToList().IndexOf(maxValue);
                var oneHotData = new BasicMLData(new double[mlData.Data.Length])
                {
                    [maxIndex] = 1
                };

                return oneHotData;
            }

            return null;
        }

        static byte[] GetBytes(double[] values) {
            var result = new byte[values.Length * sizeof(double)];
            Buffer.BlockCopy(values, 0, result, 0, result.Length);
            return result;
        }
        static double[] GetDoubles(byte[] values) {
            var result = new double[values.Length / 8];
            Buffer.BlockCopy(values, 0, result, 0, result.Length * 8);
            return result;
        }
    }
}