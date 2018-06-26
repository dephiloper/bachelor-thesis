﻿using UnityEngine;

public class AgentScript : MonoBehaviour
{
    public AgentType Type;
    public AgentEditorProperties EditorProperties;
    public Agent Agent { get; private set; }
    
    private void Start()
    {
        Agent = Type.NewInstance(transform, EditorProperties);
    }

    private void FixedUpdate()
    {
        Agent.Compute();
    }
}
