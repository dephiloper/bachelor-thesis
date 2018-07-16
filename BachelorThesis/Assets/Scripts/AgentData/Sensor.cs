using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AgentData
{
    [Serializable]
    public class Sensor
    {
        [Range(0, 10)] public float ViewRadius;
        [Range(0, 360)] public float ViewAngle;
        [Range(0, 10)] public float Range;
        public bool Show;

        private LayerMask _wallMask;
        private LayerMask _trackMask;
        private Rigidbody _rigidbody;
        private Transform _transform;
        private bool _onTrack;

        public void Setup(Component agent)
        {
            _transform = agent.transform;
            _rigidbody = agent.GetComponent<Rigidbody>();
            _wallMask = LayerMask.GetMask("Wall");
            _trackMask = LayerMask.GetMask("Track");
        }

        public Percept PerceiveEnvironment(bool onTrack)
        {
            _onTrack = onTrack;

            var wallDistances = new List<double>
            {
                CalculateDistanceWithRay((_transform.forward * 0.5f - _transform.right).normalized,
                    _transform.forward * 3.5f),
                CalculateDistanceWithRay((_transform.forward * 0.5f - _transform.right).normalized,
                    _transform.forward * 3f),
                CalculateDistanceWithRay((_transform.forward * 0.5f - _transform.right).normalized,
                    _transform.forward * 2.5f),
                CalculateDistanceWithRay((_transform.forward * 0.5f - _transform.right).normalized,
                    _transform.forward * 2f),
                CalculateDistanceWithRay((_transform.forward * 0.5f - _transform.right).normalized,
                    _transform.forward * 1.5f),
                CalculateDistanceWithRay((_transform.forward * 0.5f - _transform.right).normalized,
                    _transform.forward * 1f),
                CalculateDistanceWithRay((_transform.forward * 0.5f - _transform.right).normalized,
                    _transform.forward * 0.5f),
                CalculateDistanceWithRay((_transform.forward * 0.5f - _transform.right).normalized),
                /*CalculateDistanceWithRay((_transform.forward * 0.5f - _transform.right).normalized),
                CalculateDistanceWithRay((_transform.forward - _transform.right).normalized),
                CalculateDistanceWithRay((_transform.forward - _transform.right * 0.5f).normalized),
                CalculateDistanceWithRay((_transform.forward - _transform.right * 0.25f).normalized),
                CalculateDistanceWithRay((_transform.forward - _transform.right * 0.125f).normalized),*/
                CalculateDistanceWithRay(_transform.forward.normalized, -_transform.right * 0.1f),
                CalculateDistanceWithRay(_transform.forward.normalized),
                CalculateDistanceWithRay(_transform.forward.normalized, _transform.right * 0.1f),
                /*CalculateDistanceWithRay((_transform.forward + _transform.right * 0.125f).normalized),
                CalculateDistanceWithRay((_transform.forward + _transform.right * 0.25f).normalized),
                CalculateDistanceWithRay((_transform.forward + _transform.right * 0.5f).normalized),
                CalculateDistanceWithRay((_transform.forward + _transform.right).normalized),
                CalculateDistanceWithRay((_transform.forward * 0.5f + _transform.right).normalized),*/
                CalculateDistanceWithRay((_transform.forward * 0.5f + _transform.right).normalized),
                CalculateDistanceWithRay((_transform.forward * 0.5f + _transform.right).normalized,
                    _transform.forward * 0.5f),
                CalculateDistanceWithRay((_transform.forward * 0.5f + _transform.right).normalized,
                    _transform.forward * 1f),
                CalculateDistanceWithRay((_transform.forward * 0.5f + _transform.right).normalized,
                    _transform.forward * 1.5f),
                CalculateDistanceWithRay((_transform.forward * 0.5f + _transform.right).normalized,
                    _transform.forward * 2f),
                CalculateDistanceWithRay((_transform.forward * 0.5f + _transform.right).normalized,
                    _transform.forward * 2.5f),
                CalculateDistanceWithRay((_transform.forward * 0.5f + _transform.right).normalized,
                    _transform.forward * 3f),
                CalculateDistanceWithRay((_transform.forward * 0.5f + _transform.right).normalized,
                    _transform.forward * 3.5f),
            };

            var velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
            var visibleAgents = FindVisibleEnvironmentals(LayerMask.GetMask("Agent"),
                LayerMask.GetMask("Obstacle", "Bounding"));
            var visibleCollectables = FindVisibleEnvironmentals(LayerMask.GetMask("Collectable"),
                LayerMask.GetMask("Obstacle", "Agent", "Bounding"));
            var visibleObstacles = FindVisibleEnvironmentals(LayerMask.GetMask("Obstacle"),
                LayerMask.GetMask("Bounding"));

            return new Percept(wallDistances, velocity, visibleAgents, visibleCollectables, visibleObstacles);
        }

        private List<Vector3> FindVisibleEnvironmentals(LayerMask envMask, LayerMask blockMask)
        {
            var visibleEnvironmentals = new List<Vector3>();

            var environmentalsInViewRadius =
                Physics.OverlapSphere(_transform.position, ViewRadius, envMask);

            environmentalsInViewRadius = environmentalsInViewRadius.OrderBy(env =>
                Vector3.Distance(_transform.position, env.transform.position)).ToArray();

            foreach (var environmental in environmentalsInViewRadius)
            {
                var dir = (environmental.transform.position - _transform.position).normalized;
                if (Vector3.Angle(_transform.forward, dir) < ViewAngle / 2)
                {
                    var dist = Vector3.Distance(environmental.transform.position, _transform.position);
                    if (!Physics.Raycast(_transform.position, dir, dist, blockMask))
                        visibleEnvironmentals.Add(environmental.transform.position);
                }
            }

            return visibleEnvironmentals;
        }

        private double CalculateDistanceWithRay(Vector3 direction, Vector3 offset = new Vector3())
        {
            RaycastHit hit;

            // return negative range when the ray is casted outside of the track or on the wall
            if (_onTrack &&
                (!Physics.Raycast(_rigidbody.position + offset + _transform.up, -_transform.up, float.MaxValue,
                     _trackMask) || Physics.Raycast(_rigidbody.position + offset + _transform.up, -_transform.up,
                     float.MaxValue, _wallMask)))
                return -Range;

            // calculate from the origin of the ray to the target wall that gets hitted
            var distance = Physics.Raycast(_rigidbody.position + offset, direction, out hit, Range, _wallMask)
                ? Vector3.Distance(_rigidbody.position + offset, hit.point)
                : Range;

            // if the agent is not on track, flip distance (e.g. 0 -> 10, 10 -> 0)
            if (!_onTrack)
            {
                if (offset != Vector3.zero) return -Range;
                distance = Range - distance;
            }
                
            // show ray only if distance is greater 0
            if (Show && distance > 0)
                Debug.DrawRay(_rigidbody.position + offset, direction * distance,
                    distance < Range ? Color.red : Color.green);

            return distance;
        }
    }
}