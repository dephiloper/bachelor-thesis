using Agent;
using Agent.AgentImpl;
using UnityEngine;

public class WaypointBehaviour : MonoBehaviour
{
    public int WaypointIdentifier;

    private void OnTriggerEnter(Collider other)
    {
        var agentScript = other.GetComponent<AgentScript>();
        
        if (agentScript != null)
            (agentScript.Agent as NeuralNetAgent)?.WaypointCrossed(WaypointIdentifier, transform.parent.childCount);
    }
}