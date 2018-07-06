using Agent;
using Agent.AgentImpl;
using UnityEngine;

namespace Environment
{
    public class WaypointBehaviour : MonoBehaviour
    {
        public int WaypointIdentifier;

        private void OnTriggerEnter(Collider other)
        {
            var agentScript = other.GetComponent<AgentBehaviour>();
        
            if (agentScript != null)
                (agentScript.Agent as NeuralNetAgent)?.WaypointCrossed(WaypointIdentifier, transform.parent.childCount);
        }
    }
}