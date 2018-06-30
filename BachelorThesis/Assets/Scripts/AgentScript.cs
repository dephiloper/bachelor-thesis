using Agent;
using UnityEngine;

public class AgentScript : MonoBehaviour
{
    public AgentType Type = AgentType.NeuralNet;
    public AgentEditorProperties EditorProperties;
    
    public BaseAgent Agent { get; private set; }

    private void Awake() => Agent = Type.NewInstance(this);

    private void FixedUpdate() => Agent.Compute();
}

