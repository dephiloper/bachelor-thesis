using System;
using AgentImpl;
using UnityEngine;

namespace AgentData.Base
{
    [Serializable]
    public class Sensor : ISensor
    {
        [Range(0, 10)] public float ViewRadius;
        [Range(0, 360)] public float ViewAngle;
        [Range(0, 10)] public float Range;
        public bool Show;
        protected Agent Agent;
        
        public virtual void Setup(Agent agent)
        {
            Agent = agent;
        }

        public virtual IPercept PerceiveEnvironment()
        {
            throw new NotImplementedException();
        }

        protected bool CheckRaycastInvalid(Vector3 position)
        {
            return Agent.OnTrack &&
                   (!Physics.Raycast(position, -Agent.transform.up, float.MaxValue, LayerMask.GetMask("Track")) ||
                    Physics.Raycast(position, -Agent.transform.up, float.MaxValue, LayerMask.GetMask("Wall")));
        }
    }

    public interface ISensor
    {
        void Setup(Agent agent);
        IPercept PerceiveEnvironment();
    }
}