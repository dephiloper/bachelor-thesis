using System;
using Agent.AgentImpl;
using UnityEngine;

namespace Agent
{
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
        public static BaseAgent NewInstance(this AgentType agentType, AgentBehaviour behaviour)
        {
            switch (agentType)
            {
                case AgentType.Player: return new PlayerAgent(behaviour);
                case AgentType.PlayerVr: return new PlayerAgent(behaviour);
                case AgentType.StateMachine: return new PlayerAgent(behaviour);
                case AgentType.PathFinding: return new PathFindingAgent(behaviour);
                case AgentType.NeuralNet: return new NeuralNetAgent(behaviour);
                default: throw new ArgumentOutOfRangeException(nameof(agentType), agentType, null);
            }
        }
    }
}