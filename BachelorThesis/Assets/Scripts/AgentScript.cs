using Agent;
using UnityEngine;

public class AgentScript : MonoBehaviour
{
    public AgentType AgentType = AgentType.NeuralNet;
    public AgentEditorProperties EditorProperties;
    
    public BaseAgent Agent { get; private set; }

    private void Awake() => Agent = AgentType.NewInstance(this);

    private void FixedUpdate() 
    {
        Agent.Compute();
    }
}

