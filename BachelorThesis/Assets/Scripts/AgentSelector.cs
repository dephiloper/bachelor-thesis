using System;
using System.Collections.Generic;
using AgentImpl;
using UnityEngine;

[ExecuteInEditMode]
public class AgentSelector : MonoBehaviour
{
    
    public AgentType Type;
    [SerializeField] [HideInInspector] private AgentType _oldType;
    [SerializeField] [HideInInspector] private bool _isNew = true;


    private void Update()
    {
        if (_oldType == Type && !_isNew) return;
        _isNew = false;
        _oldType = Type;

        var agent = GetComponent<Agent>();
        Agent newAgent;
        switch (Type)
        {
            case AgentType.Player:
                newAgent = gameObject.AddComponent<PlayerAgent>();
                break;
            case AgentType.PlayerVr:
                newAgent = gameObject.AddComponent<PlayerVrAgent>();
                break;
            case AgentType.Movement:
                newAgent = gameObject.AddComponent<MovementAgent>();
                break;
            case AgentType.DecisionMaking:
                newAgent = gameObject.AddComponent<DecisionMakingAgent>();
                break;
            case AgentType.NeuralNet:
                newAgent = gameObject.AddComponent<NeuralNetAgent>();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (!agent) return;
        newAgent.SetValues(agent);
        DestroyImmediate(agent);
    }
}