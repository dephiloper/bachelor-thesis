using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AgentData;
using AgentData.Actions;
using AgentData.Sensors;
using Train;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace AgentImpl
{
    public class NeuralNetAgent : Agent
    {
        [Header(nameof(NeuralNetAgent))] public UnityEngine.Object BrainAsset;
        public bool IsTrained;
        [SerializeField] [HideInInspector] private string _fileName;
        private int _wrongDirCounter;
        private int _toSlowCounter;

        public Brain Brain { get; set; }

        private void OnValidate()
        {
            #if UNITY_EDITOR
            _fileName = Path.GetFileName(AssetDatabase.GetAssetPath(BrainAsset));
            #endif
        }

        private void Start()
        {
            var modelPath = $"{Application.streamingAssetsPath}/{_fileName}";
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
            if (!Rigidbody || GameManager.Instance && !GameManager.Instance.StartRace) return;
            
            base.Compute();
            var action = Brain.Think(Percept);
            PerformAction(action);
            UpdateEditorProps();

            if (!OnTrack)
                //Destroy(Rigidbody);
                Brain.Score -= 0.01f;
        }

        protected override void UpdateEditorProps()
        {
            base.UpdateEditorProps();
            Score = Mathf.RoundToInt(Brain.Score);
        }

        public override void WaypointCrossed(int waypointIdentifier, int lastWaypointIdentifier)
        {
            if (!IsTrained)
            {
                var vel = new Vector2(Rigidbody.velocity.x, Rigidbody.velocity.z).magnitude;

                // waypoint already satisfied
                if (ReachedWaypointId > waypointIdentifier)
                    Brain.Score -= vel;

                // when the reached waypoint is the next e.g. 0 = maxId (no waypoints), waypointIdentifier = 1
                // 0 + 1 == 1 -> true
                // skipping a waypoint will not work
                if (ReachedWaypointId + 1 == waypointIdentifier)
                    Brain.Score += vel;
            }
            
            base.WaypointCrossed(waypointIdentifier, lastWaypointIdentifier);
        }

        public override void CollectableGathered()
        {
            base.CollectableGathered();
            Brain.Score += new Vector2(Rigidbody.velocity.x, Rigidbody.velocity.z).magnitude / 2;
        }

        public override void ObstacleCollided()
        {
            base.ObstacleCollided();
            //Destroy(Rigidbody);
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