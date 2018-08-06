using System;
using AgentImpl;

public enum AgentType
{
    Player,
    PlayerVr,
    SteeringBehaviour,
    PathFinding,
    NeuralNet
}

public static class AgentTypeMethods
{
    public static AgentImpl.Agent NewInstance(this AgentType agentType)
    {
        switch (agentType)
        {
            case AgentType.Player: return new PlayerAgent();
            case AgentType.PlayerVr: return new PlayerAgent();
            case AgentType.SteeringBehaviour: return new PlayerAgent();
            case AgentType.PathFinding: return new SteeringBehaviourAgent();
            case AgentType.NeuralNet: return new NeuralNetAgent();
            default: throw new ArgumentOutOfRangeException(nameof(agentType), agentType, null);
        }
    }
}