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

        private readonly List<int> _reachedWaypointIds = new List<int>();

        public NeuralNetAgent(AgentBehaviour agentBehaviour) : base(agentBehaviour)
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
        }

        public override void Compute()
        {
            base.Compute();
            var action = Brain.Think(Percept);
            /*if (!OnTrack) 
                Brain.Score -= 0.1f;*/
            PerformAction(action);

            UpdateEditorProps();
        }

        protected override void UpdateEditorProps()
        {
            base.UpdateEditorProps();
            EditorProps.Score = Mathf.RoundToInt(Brain.Score);
        }

        public void WaypointCrossed(int waypointIdentifier, int lastWaypointIdentifier)
        {
            // waypoint already satisfied
            if (EditorProps.IsTrained) return;
        
            var vel = new Vector2(Rigidbody.velocity.x, Rigidbody.velocity.z).magnitude;
            
            if (_reachedWaypointIds.Contains(waypointIdentifier))
                Brain.Score -= vel;    
            
            var maxId = _reachedWaypointIds.Count > 0 ? _reachedWaypointIds.Max() : 0;

            // when the reached waypoint is the next e.g. 0 = maxId (no waypoints), waypointIdentifier = 1
            // 0 + 1 == 1 -> true
            // skipping a waypoint will not work
            if (maxId + 1 == waypointIdentifier)
            {
                _reachedWaypointIds.Add(waypointIdentifier);
                Brain.Score += vel;
            }
            else if (maxId != waypointIdentifier)
                Brain.Score -= vel;

            // all waypoints reached
            if (_reachedWaypointIds.Contains(lastWaypointIdentifier))
                _reachedWaypointIds.Clear();
        }

        public override void CollectableGathered() {
            base.CollectableGathered();
            Brain.Score += new Vector2(Rigidbody.velocity.x, Rigidbody.velocity.z).magnitude / 2;
        }

        public override void ObstacleCollided()
        {
            base.ObstacleCollided();
            Brain.Score -= new Vector2(Rigidbody.velocity.x, Rigidbody.velocity.z).magnitude / 2; 
        }
    }
}
