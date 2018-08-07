using System;
using AgentImpl;

public enum AgentType
{
    Player,
    PlayerVr,
    Movement,
    DecisionMaking,
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
            case AgentType.Movement: return new PlayerAgent();
            case AgentType.DecisionMaking: return new MovementAgent();
            case AgentType.NeuralNet: return new NeuralNetAgent();
            default: throw new ArgumentOutOfRangeException(nameof(agentType), agentType, null);
        }
    }
}