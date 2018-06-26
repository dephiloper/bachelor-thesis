using Agent;
using CustomEditors;
using UnityEngine;

public class AgentScript : MonoBehaviour
{
    public AgentType Type;
    public AgentEditorProperties EditorProperties;
    public BaseAgent Agent { get; private set; }
    
    private void Start()
    {
        Agent = Type.NewInstance(this);
    }

    private void FixedUpdate()
    {
        Agent.Compute();
    }
}

