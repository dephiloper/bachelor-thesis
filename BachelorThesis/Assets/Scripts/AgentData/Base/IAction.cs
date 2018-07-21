namespace AgentData.Base
{
    public interface IAction
    {
        bool AccelerateForward { get; }
        bool AccelerateBackward { get; }
        float AccelerateValue { get; }
        bool SteerLeft { get; }
        bool SteerRight { get; }
        float SteerValue { get; }
        double[] Raw { get; }
        
        double ActivationThreshold { get; }
    }
}