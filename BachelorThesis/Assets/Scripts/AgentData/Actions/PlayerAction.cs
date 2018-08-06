using AgentData.Base;
using UnityEngine;

namespace AgentData.Actions
{
    public class PlayerAction : IAction
    {
        public bool AccelerateForward { get; }
        public bool AccelerateBackward { get; }
        public float AccelerateValue { get; }
        public bool SteerLeft { get; }
        public bool SteerRight { get; }
        public float SteerValue { get; }
        public double[] Raw { get; }
        public double ActivationThreshold { get; } = 0d;
    
        public PlayerAction(double horizontalAxis, double verticalAxis, bool isDiscrete = true)
        {
            //Debug.Log($"Input(vAxis:{verticalAxis}, hAxis: {horizontalAxis})");
            
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