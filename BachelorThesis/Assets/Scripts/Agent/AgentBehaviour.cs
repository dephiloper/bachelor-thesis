using UnityEngine;

namespace Agent
{
    public class AgentBehaviour : MonoBehaviour
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
}

