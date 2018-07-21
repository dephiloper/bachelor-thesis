namespace AgentData.Base
{
    public interface IPercept
    {
        double[] ToDoubleArray();
        void Normalize();
    }
}