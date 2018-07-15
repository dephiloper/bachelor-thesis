using System;
using System.Collections.Generic;
using System.Reflection;
using NNSharp.DataTypes;
using NNSharp.KernelDescriptors;
using NNSharp.Models;
using NNSharp.SequentialBased.SequentialExecutors;
using NNSharp.SequentialBased.SequentialLayers;

namespace Extensions
{
    // ReSharper disable once InconsistentNaming
    // reason name of nuget package has inconsistent naming
    public static class NNSharpExtensions
    {
        public static void SetWeights(this SequentialModel sequentialModel, int[] layers, double[] weights)
        {
            if (sequentialModel.GetInputDimension().c != layers[0])
                throw new ArgumentOutOfRangeException(nameof(ArgumentOutOfRangeException),
                    "Size of input layer should be" +
                    $"{sequentialModel.GetInputDimension().c} but is {layers[0]}.");

            var data = new List<IData>();
            var count = 0;

            for (var i = 0; i < layers.Length - 1; i++)
            {
                // create weight list
                data.Add(new Data2D(1, 1, layers[i], layers[i + 1]));
                data.Add(new DataArray(layers[i + 1]));
                data.Add(new DataArray(0));

                // assign weights to weight list
                for (var j = 0; j < layers[i]; j++)
                for (var k = 0; k < layers[i + 1]; k++)
                    ((Data2D) data[i * 3])[0, 0, j, k] = weights[count++];

                // assign biases to weight list
                for (var j = 0; j < layers[i + 1]; j++)
                    ((DataArray) data[i * 3 + 1])[j] = weights[count++];
            }

            sequentialModel.SetWeights(data);
        }

        public static double[] GetWeights(this SequentialModel sequentialModel)
        {
            // reflection used because there is no other way to get the weights of a network in nnsharp
            var weights = new List<double>();
            var layers = sequentialModel.GetFieldValue<DefaultExecutor>("compiled")
                .GetFieldValue<IList<ILayer>>("layers");

            foreach (var layer in layers)
            {
                if (layer is Dense2DLayer) 
                {
                    var t = (layer as Dense2DLayer).GetFieldValue<IData>("weights").GetFieldValue<double[,,,]>("tensor")
                        .GetEnumerator();
                    while (t.MoveNext())
                        if (t.Current != null)
                            weights.Add((double)t.Current);
                } 
                else if (layer is Bias2DLayer)
                {
                    var a = layer.GetFieldValue<IData>("biases").GetFieldValue<double[]>("array");
                    weights.AddRange(a);
                }
            }

            return weights.ToArray();
        }

        public static int[] GetLayers(this SequentialModel sequentialModel)
        {
            var layers = new List<int>();
            var l = sequentialModel.GetFieldValue<DefaultExecutor>("compiled").GetFieldValue<List<ILayer>>("layers");

            var output = 0;
            
            foreach (var layer in l)
            {
                if (!(layer is Dense2DLayer)) continue;
                
                var denseSum = (layer as Dense2DLayer).GetLayerSummary();
                layers.Add(denseSum.InputChannel);
                output = denseSum.OutputChannel;
            }    

            layers.Add(output);
            
            return layers.ToArray();
        }

        public static void SetData(this Data2D data2D, double[] input)
        {
            for (var i = 0; i < input.Length; i++)
                data2D[0, 0, i, 0] = input[i];
        }

        public static double[] ToDoubleArray(this Data2D data2D, int[] layers)
        {
            var outputLayer = layers[layers.Length - 1];
            var arr = new double[outputLayer];
            for (var i = 0; i < outputLayer; i++)
            {
                arr[i] = data2D[0, 0, i, 0];
            }

            return arr;
        }

        private static T GetFieldValue<T>(this object obj, string name)
        {
            var field = obj.GetType()
                .GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return (T) field?.GetValue(obj);
        }
    }
}