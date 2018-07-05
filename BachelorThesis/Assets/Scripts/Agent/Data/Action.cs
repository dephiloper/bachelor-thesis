using UnityEngine;

namespace Agent.Data
{
    public struct Action
    {
        public bool AccelerateForward { get; }
        public bool AccelerateBackward { get; }
        public float AccelerateValue { get; }
        public bool SteerLeft { get; }
        public bool SteerRight { get; }
        public float SteerValue { get; }
        private const double ActivationThreshold = 0d;
    
        public Action(double[] data)
        {
            AccelerateForward = data[0] > ActivationThreshold;
            AccelerateBackward = data[0] < ActivationThreshold;
            AccelerateValue = Mathf.Abs((float)data[0]);
            SteerLeft = data[1] > ActivationThreshold;
            SteerRight = data[1] < ActivationThreshold;
            SteerValue = Mathf.Abs((float)data[1]);
        }

        public Action(double horizontalAxis, double verticalAxis, bool isDiscrete = true)
        {
            AccelerateForward = verticalAxis > ActivationThreshold;
            AccelerateBackward = verticalAxis < ActivationThreshold;
            AccelerateValue = isDiscrete ? 1 : Mathf.Abs((float) verticalAxis);
            SteerLeft =  horizontalAxis < ActivationThreshold;
            SteerRight = horizontalAxis > ActivationThreshold;
            SteerValue = isDiscrete ? 1 : Mathf.Abs((float) horizontalAxis);
        }
    }
}