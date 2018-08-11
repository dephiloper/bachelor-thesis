using System;
using AgentData.Base;
using UnityEngine;

namespace AgentData.Actions
{
    public class NeuralNetAction : IAction
    {
        public bool AccelerateForward { get; }
        public bool AccelerateBackward { get; }
        public float AccelerateValue { get; }
        public bool SteerLeft { get; }
        public bool SteerRight { get; }
        public float SteerValue { get; }
        public double[] Raw { get; }
        public double ActivationThreshold { get; } = 0.3d;
    
        public NeuralNetAction(double[] data)
        {
            //Debug.Log($"Input(vAxis:{data[0]}, hAxis: {data[1]})");
            
            data[0] = Math.Round(data[0]);
            data[1] = Math.Round(data[1]);
            
            Raw = data;
            AccelerateForward = data[0] > ActivationThreshold;
            AccelerateBackward = data[0] < -ActivationThreshold;
            AccelerateValue = Mathf.Abs((float)data[0]);
            SteerLeft = data[1] < ActivationThreshold;
            SteerRight = data[1] > -ActivationThreshold;
            SteerValue = Mathf.Abs((float)data[1]);
        }
    }
}