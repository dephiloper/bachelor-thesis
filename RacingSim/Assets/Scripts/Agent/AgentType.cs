using System;
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
        public static BaseAgent NewInstance(this AgentType agentType, AgentScript script)
        {
            switch (agentType)
            {
                case AgentType.Player: return new PlayerAgent(script);
                case AgentType.PlayerVr: return new PlayerAgent(script);
                case AgentType.StateMachine: return new PlayerAgent(script);
                case AgentType.PathFinding: return new PlayerAgent(script);
                case AgentType.NeuralNet: return new NeuralNetAgent(script);
                default: throw new ArgumentOutOfRangeException(nameof(agentType), agentType, null);
            }
        }
    }
}