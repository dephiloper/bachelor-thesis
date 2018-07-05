using System;
using System.Collections.Generic;
using System.Linq;
using Train;
using UnityEngine;

namespace Agent.AgentImpl
{
    public class NeuralNetAgent : BaseAgent
    {
        public Brain Brain { get; set; }

        public NeuralNetAgent(AgentScript agentScript) : base(agentScript)
        {
            if (EditorProps.BrainAsset)
            {
                Brain = Brain.Import(EditorProps.BrainAsset.text);
                EditorProps.IsTrained = true;
            }
            else if (!TrainManager.Instance)
                throw new ArgumentNullException(nameof(ArgumentNullException),
                    $"When there is no {nameof(EditorProps.BrainAsset)} " +
                    "specified, the agent performs training. Unfortunetly there is also no TrainManager " +
                    "set in this scene.");
        
            EditorProps.ReachedWaypointIds = new List<int>();
        }

        public override void Compute()
        {
            base.Compute();
            var action = Brain.Think(Percept);
            if (!OnTrack) 
                Brain.Score -= 0.1f;
            PerformAction(action);

            EditorProps.Score = Mathf.RoundToInt(Brain.Score);
        }

        public void WaypointCrossed(int waypointIdentifier, int lastWaypointIdentifier)
        {
            // waypoint already satisfied
            if (EditorProps.IsTrained) return;
        
            var vel = new Vector2(Rigidbody.velocity.x, Rigidbody.velocity.z).magnitude;
            
            if (EditorProps.ReachedWaypointIds.Contains(waypointIdentifier))
                Brain.Score -= vel;    
            
            var maxId = EditorProps.ReachedWaypointIds.Count > 0 ? EditorProps.ReachedWaypointIds.Max() : 0;

            // when the reached waypoint is the next e.g. 0 = maxId (no waypoints), waypointIdentifier = 1
            // 0 + 1 == 1 -> true
            // skipping a waypoint will not work
            if (maxId + 1 == waypointIdentifier)
            {
                EditorProps.ReachedWaypointIds.Add(waypointIdentifier);
                Brain.Score += vel;
            }
            else if (maxId != waypointIdentifier)
                Brain.Score -= vel;

            // all waypoints reached
            if (EditorProps.ReachedWaypointIds.Contains(lastWaypointIdentifier))
                EditorProps.ReachedWaypointIds.Clear();
        }
    }
}
