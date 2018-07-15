using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Train;
using UnityEditor;
using UnityEngine;

namespace AgentImpl
{
    public class NeuralNetAgent : Agent
    {
        [Header(nameof(NeuralNetAgent))]
        public TextAsset BrainAsset;
        public bool IsTrained;
        [SerializeField]
        [HideInInspector]
        private string _fileName;
        
        public Brain Brain { get; set; }

        private readonly List<int> _reachedWaypointIds = new List<int>();

        private void OnValidate()
        {
            #if UNITY_EDITOR
            _fileName = Path.GetFileName(AssetDatabase.GetAssetPath(BrainAsset));
            #endif
        }

        private void Start()
        {
            var modelPath = $"{Application.dataPath}/StreamingAssets/{_fileName}";
            if (!string.IsNullOrEmpty(_fileName))
            {
                Brain = Brain.Import(modelPath);
                IsTrained = true;
            }
            else if (!TrainManager.Instance)
                throw new ArgumentNullException(nameof(ArgumentNullException),
                    $"When there is no {nameof(modelPath)} " +
                    "specified, the agent performs training. Unfortunetly there is also no TrainManager " +
                    "set in this scene.");
        }

        protected override void Compute()
        {
            base.Compute();
            var action = Brain.Think(Percept);
            PerformAction(action);
            UpdateEditorProps();

            if (!OnTrack) 
                Brain.Score -= 0.1f;
        }

        protected override void UpdateEditorProps()
        {
            base.UpdateEditorProps();
            Score = Mathf.RoundToInt(Brain.Score);
        }

        public void WaypointCrossed(int waypointIdentifier, int lastWaypointIdentifier)
        {
            // waypoint already satisfied
            if (IsTrained) return;
        
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

        public override void SetValues(Agent agent)
        {
            base.SetValues(agent);
            var neuralNetAgent = agent as NeuralNetAgent;
            if (neuralNetAgent == null) return;
            
            BrainAsset = neuralNetAgent.BrainAsset;
            IsTrained = neuralNetAgent.IsTrained;
        }
    }
}
