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

            public SubSensor(LayerMask mask, Color color, bool observe)
            {
                Mask = mask;
                Color = color;
                Observe = observe;
            }
        }
        
        public override IPercept PerceiveEnvironment()
        {
            var directionOffset = new List<Tuple<Vector3, Vector3>>
            {
                new Tuple<Vector3, Vector3>((Agent.transform.forward * 0.5f - Agent.transform.right).normalized,
                    Agent.transform.forward * 3),
                new Tuple<Vector3, Vector3>((Agent.transform.forward * 0.5f - Agent.transform.right).normalized,
                    Agent.transform.forward * 2),
                new Tuple<Vector3, Vector3>((Agent.transform.forward * 0.5f - Agent.transform.right).normalized,
                    Agent.transform.forward),
                new Tuple<Vector3, Vector3>((Agent.transform.forward * 0.25f - Agent.transform.right).normalized,
                    Vector3.zero),
                new Tuple<Vector3, Vector3>((Agent.transform.forward * 0.5f - Agent.transform.right).normalized,
                    Vector3.zero),
                new Tuple<Vector3, Vector3>((Agent.transform.forward * 0.75f - Agent.transform.right).normalized,
                    Vector3.zero),
                new Tuple<Vector3, Vector3>((Agent.transform.forward - Agent.transform.right * 0.25f).normalized,
                    Vector3.zero),
                new Tuple<Vector3, Vector3>((Agent.transform.forward - Agent.transform.right * 0.5f).normalized,
                    Vector3.zero),
                new Tuple<Vector3, Vector3>((Agent.transform.forward - Agent.transform.right * 0.75f).normalized,
                    Vector3.zero),
                new Tuple<Vector3, Vector3>(Agent.transform.forward.normalized, -Agent.transform.right * 0.1f),
                new Tuple<Vector3, Vector3>(Agent.transform.forward.normalized, Vector3.zero),
                new Tuple<Vector3, Vector3>(Agent.transform.forward.normalized, Agent.transform.right * 0.1f),
                new Tuple<Vector3, Vector3>((Agent.transform.forward + Agent.transform.right * 0.75f).normalized,
                    Vector3.zero),
                new Tuple<Vector3, Vector3>((Agent.transform.forward + Agent.transform.right * 0.5f).normalized,
                    Vector3.zero),
                new Tuple<Vector3, Vector3>((Agent.transform.forward + Agent.transform.right * 0.25f).normalized,
                    Vector3.zero),
                new Tuple<Vector3, Vector3>((Agent.transform.forward * 0.75f + Agent.transform.right).normalized,
                    Vector3.zero),
                new Tuple<Vector3, Vector3>((Agent.transform.forward * 0.5f + Agent.transform.right).normalized,
                    Vector3.zero),
                new Tuple<Vector3, Vector3>((Agent.transform.forward * 0.25f + Agent.transform.right).normalized,
                    Vector3.zero),
                new Tuple<Vector3, Vector3>((Agent.transform.forward * 0.5f + Agent.transform.right).normalized,
                    Agent.transform.forward),
                new Tuple<Vector3, Vector3>((Agent.transform.forward * 0.5f + Agent.transform.right).normalized,
                    Agent.transform.forward * 2),
                new Tuple<Vector3, Vector3>((Agent.transform.forward * 0.5f + Agent.transform.right).normalized,
                    Agent.transform.forward * 3),
            };

            var subSensors = new List<SubSensor>
            {
                new SubSensor(LayerMask.GetMask("Wall", "Obstacle"), Color.red, true),
                new SubSensor(LayerMask.GetMask("Collectable"), Color.yellow, !TrainManager.Instance),
                new SubSensor(LayerMask.GetMask("Agent"), Color.green, !TrainManager.Instance),
            };
    
            foreach (var tuple in directionOffset)
            {
                var rigidbody = Agent.GetComponent<Rigidbody>();

                if (CheckRaycastInvalid(rigidbody.position + tuple.Item2 + Agent.transform.up))
                {
                    foreach (var s in subSensors)
                        s.Distance.Add(-Range);

                    continue;
                }

                foreach (var subSensor in subSensors)
                {
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