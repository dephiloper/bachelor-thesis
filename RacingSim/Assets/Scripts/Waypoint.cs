using Agent;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public int WaypointIdentifier;
    public int PassedCounter;
    private const int MaxPassedCount = 50;

    private void OnTriggerEnter(Collider other)
    {
        var agentScript = other.GetComponent<AgentScript>();
        
        if (agentScript != null)
        {
            if (PassedCounter < MaxPassedCount)
                PassedCounter++;
            (agentScript.Agent as NeuralNetAgent)?.WaypointCrossed(WaypointIdentifier, transform.parent.childCount, PassedCounter);
        }
    }
}