using System;
using System.Collections.Generic;
using UnityEngine;

namespace Agent
{
    [Serializable]
    public abstract class BaseEditorProperties
    {
        public float MaxSpeed = 10f;
        public float TurnSpeed = 2f;
        public int Score;
        public float Speed;
    }

    [Serializable]
    public class PlayerEditorProperties : BaseEditorProperties
    {
        public string HAxis;
        public string VAxis;
        public int SelectedHAxis;
        public int SelectedVAxis;
    }

    [Serializable]
    public class NeuralNetEditorProperties : BaseEditorProperties
    {
        public TextAsset BrainAsset;
        public List<int> ReachedWaypointIds = new List<int>();
        public bool IsTrained;
        public bool IsExclude;
    }
}