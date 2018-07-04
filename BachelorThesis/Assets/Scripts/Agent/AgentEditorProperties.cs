using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Agent
{
    [Serializable]
    public class AgentEditorProperties
    {
        #region Base

        public Text Label;
        public float MaxSpeed = 5f;
        public float MaxTurnSpeed = 2f;
        public int Score;
        public float Speed;
        public float TurnSpeed;
        
        #endregion
        
        #region Player
        
        public string HAxis;
        public string VAxis;
        public int SelectedHAxis;
        public int SelectedVAxis;
        public bool IsDiscrete;

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