public struct Action
{
    public bool AccelerateForward { get; }
    public bool AccelerateBackward { get; }
    public bool SteerLeft { get; }
    public bool SteerRight { get; }

    public Action(double[] data)
    {
        AccelerateForward = data[0] > 0.5d;
        AccelerateBackward = data[1] > 0.5d;
        SteerLeft = data[2] > 0.5d;
        SteerRight = data[3] > 0.5d;
    }

    public Action(double verticalAxis, double horizontalAxis)
    {
        AccelerateForward = horizontalAxis < 0d;
        AccelerateBackward = horizontalAxis > 0d;
        SteerLeft = verticalAxis < 0d;
        SteerRight = verticalAxis > 0d;
    }
}