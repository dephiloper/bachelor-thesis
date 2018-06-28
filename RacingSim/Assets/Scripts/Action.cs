public struct Action
{
    public bool AccelerateForward { get; }
    public bool AccelerateBackward { get; }
    public bool SteerLeft { get; }
    public bool SteerRight { get; }
    private const double ActivationThreshold = 0.5d;
    private const double InputThreshold = 0d;
    
    public Action(double[] data)
    {
        AccelerateForward = data[0] > ActivationThreshold;
        AccelerateBackward = data[1] > ActivationThreshold;
        SteerLeft = data[2] > ActivationThreshold;
        SteerRight = data[3] > ActivationThreshold;
    }

    public Action(double verticalAxis, double horizontalAxis)
    {
        AccelerateForward = horizontalAxis > InputThreshold;
        AccelerateBackward = horizontalAxis < InputThreshold;
        SteerLeft = verticalAxis < InputThreshold;
        SteerRight = verticalAxis > InputThreshold;
    }
}