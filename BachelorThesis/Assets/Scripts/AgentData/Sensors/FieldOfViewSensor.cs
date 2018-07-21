using System;
using System.Collections.Generic;
using System.Linq;
using AgentData.Base;
using AgentData.Percepts;
using AgentImpl;
using UnityEngine;

namespace AgentData.Sensors
{
    [Serializable]
    public class FieldOfViewSensor : Sensor
    {
        private LayerMask _wallMask;
        private LayerMask _trackMask;
        
        public override void Setup(Agent agent)
        {
            base.Setup(agent);
            _wallMask = LayerMask.GetMask("Wall", "Obstacle");
            _trackMask = LayerMask.GetMask("Track");
        }

        public override IPercept PerceiveEnvironment()
        {
            var forwardLeft = (Agent.transform.forward * 0.5f - Agent.transform.right).normalized;
            var forwardRight = (Agent.transform.forward * 0.5f + Agent.transform.right).normalized;
            var wallDistances = new List<double>
            {
                CalculateDistanceWithRay(forwardLeft, Agent.transform.forward * 3.5f),
                CalculateDistanceWithRay(forwardLeft, Agent.transform.forward * 3f),
                CalculateDistanceWithRay(forwardLeft, Agent.transform.forward * 2.5f),
                CalculateDistanceWithRay(forwardLeft, Agent.transform.forward * 2f),
                CalculateDistanceWithRay(forwardLeft, Agent.transform.forward * 1.5f),
                CalculateDistanceWithRay(forwardLeft, Agent.transform.forward * 1f),
                CalculateDistanceWithRay(forwardLeft, Agent.transform.forward * 0.5f),
                CalculateDistanceWithRay(forwardLeft),
                CalculateDistanceWithRay(Agent.transform.forward.normalized, -Agent.transform.right * 0.1f),
                CalculateDistanceWithRay(Agent.transform.forward.normalized),
                CalculateDistanceWithRay(Agent.transform.forward.normalized, Agent.transform.right * 0.1f),
                CalculateDistanceWithRay(forwardRight),
                CalculateDistanceWithRay(forwardRight, Agent.transform.forward * 0.5f),
                CalculateDistanceWithRay(forwardRight, Agent.transform.forward * 1f),
                CalculateDistanceWithRay(forwardRight, Agent.transform.forward * 1.5f),
                CalculateDistanceWithRay(forwardRight, Agent.transform.forward * 2f),
                CalculateDistanceWithRay(forwardRight, Agent.transform.forward * 2.5f),
                CalculateDistanceWithRay(forwardRight, Agent.transform.forward * 3f),
                CalculateDistanceWithRay(forwardRight, Agent.transform.forward * 3.5f),
            };
            
            var visibleAgents = FindVisibleEnvironmentals(LayerMask.GetMask("Agent"),
                LayerMask.GetMask("Obstacle", "Bounding"));
            var visibleCollectables = FindVisibleEnvironmentals(LayerMask.GetMask("Collectable"),
                LayerMask.GetMask("Obstacle", "Agent", "Bounding"));
            var visibleObstacles = FindVisibleEnvironmentals(LayerMask.GetMask("Obstacle"),
                LayerMask.GetMask("Bounding"));

            return new FieldOfViewPercept(Agent, this, wallDistances, visibleAgents, visibleCollectables, visibleObstacles);
        }

        private List<Vector3> FindVisibleEnvironmentals(LayerMask envMask, LayerMask blockMask)
        {
            var visibleEnvironmentals = new List<Vector3>();

            var environmentalsInViewRadius =
                Physics.OverlapSphere(Agent.transform.position, ViewRadius, envMask);

            environmentalsInViewRadius = environmentalsInViewRadius.OrderBy(env =>
                Vector3.Distance(Agent.transform.position, env.transform.position)).ToArray();

            foreach (var environmental in environmentalsInViewRadius)
            {
                var dir = (environmental.transform.position - Agent.transform.position).normalized;
                if (Vector3.Angle(Agent.transform.forward, dir) < ViewAngle / 2)
                {
                    var dist = Vector3.Distance(environmental.transform.position, Agent.transform.position);
                    if (!Physics.Raycast(Agent.transform.position, dir, dist, blockMask))
                        visibleEnvironmentals.Add(environmental.transform.position);
                }
            }

            return visibleEnvironmentals;
        }

        private double CalculateDistanceWithRay(Vector3 direction, Vector3 offset = new Vector3())
        {
            var rigidbody = Agent.GetComponent<Rigidbody>();
            RaycastHit hit;

            // return negative range when the ray is casted outside of the track or on the wall
            if (Agent.OnTrack &&
                (!Physics.Raycast(rigidbody.position + offset + Agent.transform.up, -Agent.transform.up,
                     float.MaxValue,
                     _trackMask) || Physics.Raycast(rigidbody.position + offset + Agent.transform.up,
                     -Agent.transform.up,
                     float.MaxValue, _wallMask)))
                return -Range;

            // calculate from the origin of the ray to the target wall that gets hitted
            var distance = Physics.Raycast(rigidbody.position + offset, direction, out hit, Range, _wallMask)
                ? Vector3.Distance(rigidbody.position + offset, hit.point)
                : Range;

            // if the agent is not on track, flip distance (e.g. 0 -> 10, 10 -> 0)
            if (!Agent.OnTrack)
            {
                if (offset != Vector3.zero) return -Range;
                distance = Range - distance;
            }

            // show ray only if distance is greater 0
            if (Show && distance > 0)
                Debug.DrawRay(rigidbody.position + offset, direction * distance,
                    distance < Range ? Color.red : Color.green);

            return distance;
        }
    }
}