using System;
using System.Collections.Generic;
using System.Linq;
using NNSharp.DataTypes;
using NNSharp.Models;
using UnityEngine;

public static class ExtensionMethods
{
    public static Vector2 ToVector2(this Vector3 v3) => new Vector2(v3.x, v3.z);
    public static Vector3 ToVector3(this Vector2 v2) => new Vector3(v2.x, 0, v2.y);

    public static float Map(this float value, float inFrom, float inTo, float outFrom, float outTo) =>
        (value - inFrom) / (inTo - inFrom) * (outTo - outFrom) + outFrom;

    public static void SetWeights(this SequentialModel sequentialModel, int[] layers, double[] weights)
    {
        if (sequentialModel.GetInputDimension().c != layers[0])
            throw new ArgumentOutOfRangeException(nameof(ArgumentOutOfRangeException), "Size of input layer should be" +
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

    public static int GetWeightCount(this SequentialModel sequentialModel, int[] layers)
    {
        var count = 0;

        for (var i = 0; i < layers.Length - 1; i++)
            count += layers[i] * layers[i + 1] + layers[i + 1];

        return count;
    }

    public static void SetInput(this Data2D data2D, double[] input)
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
}