using System;
using System.Collections.Generic;
using AgentData.Base;
using AgentData.Percepts;
using UnityEngine;
using System.Linq;
using Train;

namespace AgentData.Sensors
{
    [Serializable]
    public class DistanceOnlySensor : Sensor
    {
        private class SubSensor
        {
            public readonly LayerMask Mask;
            public readonly List<double> Distance = new List<double>();
            public readonly Color Color;
            public readonly bool Observe;
            public readonly bool OriginOnly;

            public SubSensor(LayerMask mask, Color color, bool observe = false, bool originOnly = false)
            {
                Mask = mask;
                Color = color;
                Observe = observe;
                OriginOnly = originOnly;
            }
        }
        
        public override IPercept PerceiveEnvironment()
        {
            var forwardLeft = (Agent.transform.forward * 0.5f - Agent.transform.right).normalized;
            var forwardRight = (Agent.transform.forward * 0.5f + Agent.transform.right).normalized;

            var directionOffset = new List<Tuple<Vector3, Vector3>>
            {
                new Tuple<Vector3, Vector3>(forwardLeft, Agent.transform.forward * 4f),
                new Tuple<Vector3, Vector3>(forwardLeft, Agent.transform.forward * 2f),
                new Tuple<Vector3, Vector3>(forwardLeft, Agent.transform.forward * 1f),
                new Tuple<Vector3, Vector3>(Agent.transform.forward.normalized - Agent.transform.right * 0.6f, Vector3.zero),
                new Tuple<Vector3, Vector3>(Agent.transform.forward.normalized - Agent.transform.right * 0.3f, Vector3.zero),
                new Tuple<Vector3, Vector3>(Agent.transform.forward.normalized - Agent.transform.right * 0.1f, Vector3.zero),
                new Tuple<Vector3, Vector3>(- Agent.transform.right, Vector3.zero),
                new Tuple<Vector3, Vector3>(Agent.transform.forward.normalized, Vector3.zero),
                new Tuple<Vector3, Vector3>(Agent.transform.right, Vector3.zero),
                new Tuple<Vector3, Vector3>(Agent.transform.forward.normalized + Agent.transform.right * 0.1f, Vector3.zero),
                new Tuple<Vector3, Vector3>(Agent.transform.forward.normalized + Agent.transform.right * 0.3f, Vector3.zero),
                new Tuple<Vector3, Vector3>(Agent.transform.forward.normalized + Agent.transform.right * 0.6f, Vector3.zero),
                new Tuple<Vector3, Vector3>(forwardRight, Agent.transform.forward * 1f),
                new Tuple<Vector3, Vector3>(forwardRight, Agent.transform.forward * 2f),
                new Tuple<Vector3, Vector3>(forwardRight, Agent.transform.forward * 4f),
            };

            var subSensors = new List<SubSensor>
            {
                new SubSensor(LayerMask.GetMask("Wall"), Color.red, true),
                new SubSensor(LayerMask.GetMask("Collectable"), Color.yellow, true, true),
                new SubSensor(LayerMask.GetMask("Obstacle"), Color.black, true, true),
            };

            foreach (var subSensor in subSensors)
            {
                foreach (var tuple in directionOffset)
                {
                    if (subSensor.OriginOnly && tuple.Item2 != Vector3.zero) continue;

                    var rigidbody = Agent.GetComponent<Rigidbody>();

                    if (CheckRaycastInvalid(rigidbody.position + tuple.Item2 + Agent.transform.up))
                    {
                        subSensor.Distance.Add(-Range);
                        continue;
                    }
                
                    RaycastHit hit;

                    if (!Agent.OnTrack && tuple.Item2 != Vector3.zero)
                        subSensor.Distance.Add(-10);
                    else if (subSensor.Observe && Physics.Raycast(rigidbody.position + tuple.Item2, tuple.Item1, out hit, Range, subSensor.Mask))
                    {
                        subSensor.Distance.Add(Agent.OnTrack ? hit.distance : Range - hit.distance);
                        Debug.DrawRay(rigidbody.position + tuple.Item2, tuple.Item1 * hit.distance, subSensor.Color);
                    }
                    else if (!Agent.OnTrack)
                        subSensor.Distance.Add(-Range);
                    else
                        subSensor.Distance.Add(Range);
                }
            }
            
            return new DistanceOnlyPercept(Agent, this, subSensors.Select(x => x.Distance).ToArray());
        }
    }
}