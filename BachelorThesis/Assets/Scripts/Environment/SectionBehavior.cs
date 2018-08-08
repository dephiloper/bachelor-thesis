﻿using AgentImpl;
using UnityEngine;

namespace Environment
{
    public class SectionBehavior : MonoBehaviour
    {
        public int WaypointIdentifier;

        private void OnTriggerEnter(Collider other)
        {
            var agent = other.GetComponent<Agent>();
        
            if (agent != null)
                agent.WaypointCrossed(WaypointIdentifier, transform.parent.childCount);
        }
    }
}