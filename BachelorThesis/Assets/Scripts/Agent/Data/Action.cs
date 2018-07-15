using System;
using UnityEngine;

namespace Agent.Data
{
    public class Action
    {
        public bool AccelerateForward { get; }
        public bool AccelerateBackward { get; }
        public float AccelerateValue { get; }
        public bool SteerLeft { get; }
        public bool SteerRight { get; }
        public float SteerValue { get; }
        public double[] Raw { get; }
        
        //TODO reset activation Threshold
        private const double ActivationThreshold = 0d;
    
        public Action(double[] data)
        {
            Debug.Log($"Input(vAxis:{data[0]}, hAxis: {data[1]})");
            data[0] = Math.Round(data[0]);
            data[1] = Math.Round(data[1]);
            
            AccelerateForward = data[0] > ActivationThreshold + 0.3;
            AccelerateBackward = data[0] < -(ActivationThreshold + 0.3);
            AccelerateValue = Mathf.Abs((float)data[0]);
            SteerLeft = data[1] < ActivationThreshold + 0.3;
            SteerRight = data[1] > -(ActivationThreshold + 0.3);
            SteerValue = Mathf.Abs((float)data[1]);
        }

        public Action(double horizontalAxis, double verticalAxis, bool isDiscrete = true)
        {
            Debug.Log($"Input(vAxis:{verticalAxis}, hAxis: {horizontalAxis})");
            Raw = new[] {verticalAxis, horizontalAxis};
            AccelerateForward = verticalAxis > ActivationThreshold;
            AccelerateBackward = verticalAxis < ActivationThreshold;
            AccelerateValue = isDiscrete ? 1 : Mathf.Abs((float) verticalAxis);
            SteerLeft =  horizontalAxis < ActivationThreshold;
            SteerRight = horizontalAxis > ActivationThreshold;
            SteerValue = isDiscrete ? 1 : Mathf.Abs((float) horizontalAxis);
        }
    }
}