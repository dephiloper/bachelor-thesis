using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Agent
{
    public class NeuralNetAgent : BaseAgent
    {
        public Brain Brain { get; set; }
        private const float ScoreReduction = 0.05f;

        public NeuralNetAgent(AgentScript agentScript) : base(agentScript)
        {
            if (EditorProps.BrainAsset)
            {
                Brain = Brain.Load(EditorProps.BrainAsset.text);
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
            PerformAction(action);
        }

        public void WaypointCrossed(int waypointIdentifier, int lastWaypointIdentifier)
        {
            // waypoint already satisfied
            if (EditorProps.ReachedWaypointIds.Contains(waypointIdentifier) || EditorProps.IsTrained) return;
        
            var maxId = EditorProps.ReachedWaypointIds.Count > 0 ? EditorProps.ReachedWaypointIds.Max() : 0;

            // when the reached waypoint is the next e.g. 0 = maxId (no waypoints), waypointIdentifier = 1
            // 0 + 1 == 1 -> true
            // skipping a waypoint will not work
            if (maxId + 1 == waypointIdentifier)
            {
                EditorProps.ReachedWaypointIds.Add(waypointIdentifier);
                var vel = new Vector2(Rigidbody.velocity.x, Rigidbody.velocity.z).magnitude;
                Brain.Score += vel;
            }
            else if (maxId != waypointIdentifier)
                Brain.Score -= EditorProps.MaxSpeed;

            // all waypoints reached
            if (EditorProps.ReachedWaypointIds.Contains(lastWaypointIdentifier))
                EditorProps.ReachedWaypointIds.Clear();
        }
    }
}
