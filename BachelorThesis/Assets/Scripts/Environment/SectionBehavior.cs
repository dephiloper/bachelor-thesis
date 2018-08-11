using AgentImpl;
using UnityEngine;

namespace Environment
{
    public class SectionBehavior : MonoBehaviour
    {
        public int WaypointIdentifier;

        private void OnTriggerExit(Collider other)
        {
            var agent = other.GetComponent<Agent>();
        
            if (agent != null)
                agent.WaypointCrossed(WaypointIdentifier, transform.parent.childCount);
        }
    }
}