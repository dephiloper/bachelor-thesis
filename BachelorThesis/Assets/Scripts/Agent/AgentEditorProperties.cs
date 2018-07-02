using System;
using System.Collections.Generic;
using UnityEngine;

namespace Agent
{
    [Serializable]
    public class AgentEditorProperties
    {
        #region Base
        
        public float MaxSpeed = 10f;
        public float TurnSpeed = 2f;
        public int Score;
        public float Speed;
        
        #endregion
        
        #region Player
        
        public string HAxis;
        public string VAxis;
        public int SelectedHAxis;
        public int SelectedVAxis;

        #endregion

        #region PathFinding

        public GameObject WaypointsPrefab;

        #endregion
        
        #region NeuralNet
        
        public TextAsset BrainAsset;
        public bool IsTrained;
        public List<int> ReachedWaypointIds;

        #endregion
    }
}