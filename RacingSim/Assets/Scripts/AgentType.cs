using System;
using CustomEditors;
using UnityEngine;

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
    public static Agent NewInstance(this AgentType agentType, Transform transform, AgentEditorProperties editorProps)
    {
        switch (agentType)
        {
            case AgentType.Player: return new PlayerAgent(transform, editorProps.PlayerId);
            case AgentType.PlayerVr: return new PlayerAgent(transform, editorProps.PlayerId);
            case AgentType.StateMachine: return new PlayerAgent(transform, editorProps.PlayerId);
            case AgentType.PathFinding: return new PlayerAgent(transform, editorProps.PlayerId);
            case AgentType.NeuralNet: return new NeuralNetAgent(transform, editorProps.BrainAsset);
            default: throw new ArgumentOutOfRangeException(nameof(agentType), agentType, null);
        }
    }
}