using System;
using AgentImpl;

public enum AgentType
{
    Player,
    PlayerVr,
    StateMachine,
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
            case AgentType.StateMachine: return new PlayerAgent();
            case AgentType.PathFinding: return new PathFindingAgent();
            case AgentType.NeuralNet: return new NeuralNetAgent();
            default: throw new ArgumentOutOfRangeException(nameof(agentType), agentType, null);
        }
    }
}