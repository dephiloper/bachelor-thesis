using UnityEngine;

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
        AccelerateBackward = data[1] > ActivationThreshold;
        AccelerateValue = 1;
        SteerLeft = data[2] > ActivationThreshold;
        SteerRight = data[3] > ActivationThreshold;
        SteerValue = 1;
    }

    public Action(double horizontalAxis, double verticalAxis)
    {
        AccelerateForward = verticalAxis > ActivationThreshold;
        AccelerateBackward = verticalAxis < ActivationThreshold;
        AccelerateValue = Mathf.Abs((float) verticalAxis);
        SteerLeft =  horizontalAxis < ActivationThreshold;
        SteerRight = horizontalAxis > ActivationThreshold;
        SteerValue = Mathf.Abs((float) horizontalAxis);
    }
}