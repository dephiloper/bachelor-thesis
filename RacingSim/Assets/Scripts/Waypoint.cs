using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public int WaypointIdentifier;
    
    private void OnTriggerEnter(Collider other)
    {
        var agent = other.GetComponent<Agent>();
        
        if (agent != null)
        {
            agent.WaypointCrossed(WaypointIdentifier, transform.parent.childCount);
        }
    }
}