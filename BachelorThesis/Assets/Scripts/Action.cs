public struct Action
{
    public bool AccelerateForward { get; }
    public bool AccelerateBackward { get; }
    public bool SteerLeft { get; }
    public bool SteerRight { get; }
    private const double ActivationThreshold = 0d;
    
    public Action(double[] data)
    {
        AccelerateForward = data[0] > ActivationThreshold;
        AccelerateBackward = data[1] > ActivationThreshold;
        SteerLeft = data[2] > ActivationThreshold;
        SteerRight = data[3] > ActivationThreshold;
    }

    public Action(double verticalAxis, double horizontalAxis)
    {
        AccelerateForward = horizontalAxis > ActivationThreshold;
        AccelerateBackward = horizontalAxis < ActivationThreshold;
        SteerLeft = verticalAxis < ActivationThreshold;
        SteerRight = verticalAxis > ActivationThreshold;
    }
}