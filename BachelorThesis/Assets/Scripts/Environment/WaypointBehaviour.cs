using AgentImpl;
using UnityEngine;

namespace Environment
{
    public class WaypointBehaviour : MonoBehaviour
    {
        public int WaypointIdentifier;

        private void OnTriggerEnter(Collider other)
        {
            var agent = other.GetComponent<AgentImpl.Agent>();
        
            if (agent != null)
                (agent as NeuralNetAgent)?.WaypointCrossed(WaypointIdentifier, transform.parent.childCount);
        }
    }
}