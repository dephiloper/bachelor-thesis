using Agent;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public int WaypointIdentifier;
    
    private void OnTriggerEnter(Collider other)
    {
        var agentScript = other.GetComponent<AgentScript>();
        
        if (agentScript != null)
        {
            (agentScript.Agent as NeuralNetAgent)?.WaypointCrossed(WaypointIdentifier, transform.parent.childCount);
        }
    }
}