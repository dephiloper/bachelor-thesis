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
            if (EditorProps.IsExclude) return;
        
            base.Compute();
            var action = Brain.Think(Percept);
            if (!EditorProps.IsTrained)
                ApplyScore();
        
            PerformAction(action);
        }

        private void ApplyScore()
        {
            var vel = new Vector2(Rigidbody.velocity.x, Rigidbody.velocity.z).magnitude;
            if (OnTrack)
                Brain.Score += ScoreReduction * vel;
                //Brain.Score += Convert.ToInt32(Percept.WallDistances.Sum() * ScoreReduction * vel);
            else
                EditorProps.IsExclude = true;

            EditorProps.Score = Convert.ToInt32(Brain.Score);
        }

        public void WaypointCrossed(int waypointIdentifier, int lastWaypointIdentifier, int passedCounter)
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
                Brain.Score *= 1 + 1 / passedCounter;
            }
            //TODO improvement: else would be moving in the wrong direction
            else if (maxId != waypointIdentifier)
                EditorProps.IsExclude = true;

            // all waypoints reached
            if (EditorProps.ReachedWaypointIds.Contains(lastWaypointIdentifier))
                EditorProps.ReachedWaypointIds.Clear();
        }
    }
}
