using System.Collections.Generic;
using UnityEngine;

namespace Agent.Data
{
    public class Sensor
    {
        private const float RayDistance = 10f;
        private const float NoCollisionDistance = RayDistance * 2f;

        private readonly LayerMask _wallMask;
        private readonly Rigidbody _rigidbody;
        private readonly Transform _transform;

        public Sensor(Transform transform)
        {
            _transform = transform;
            _rigidbody = transform.GetComponent<Rigidbody>();
            _wallMask = LayerMask.GetMask("Wall", "Obstacle");
        }

        public Percept PerceiveEnvironment(bool onTrack)
        {
            var percept = new Percept
            {
                WallDistances = new List<double>
                {
                    CalculateDistanceWithRay((-_transform.right).normalized, _wallMask, onTrack),
                    CalculateDistanceWithRay((_transform.forward * 0.5f - _transform.right).normalized, _wallMask, onTrack),
                    CalculateDistanceWithRay((_transform.forward - _transform.right).normalized, _wallMask, onTrack),
                    CalculateDistanceWithRay((_transform.forward - _transform.right * 0.5f).normalized, _wallMask, onTrack),
                    CalculateDistanceWithRay((_transform.forward - _transform.right * 0.25f).normalized, _wallMask, onTrack),
                    CalculateDistanceWithRay(_transform.forward.normalized, _wallMask, onTrack, - _transform.right * 0.1f),
                    CalculateDistanceWithRay(_transform.forward.normalized, _wallMask, onTrack, _transform.right * 0.1f),
                    CalculateDistanceWithRay((_transform.forward + _transform.right * 0.25f).normalized, _wallMask, onTrack),
                    CalculateDistanceWithRay((_transform.forward + _transform.right * 0.5f).normalized, _wallMask, onTrack),
                    CalculateDistanceWithRay((_transform.forward + _transform.right).normalized, _wallMask, onTrack),
                    CalculateDistanceWithRay((_transform.forward * 0.5f + _transform.right).normalized, _wallMask, onTrack),
                    CalculateDistanceWithRay((_transform.right).normalized, _wallMask, onTrack)
                },
                Velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z)
            };

            return percept;
        }

        private double CalculateDistanceWithRay(Vector3 direction, LayerMask layerMask, bool onTrack,
            Vector3 offset = new Vector3())
        {
            RaycastHit hit;

            // normally max distance is RayDistance, so times 2 means like free to go there
            var distance =
                Physics.Raycast(_rigidbody.position + offset, direction, out hit, RayDistance, layerMask)
                    ? Vector3.Distance(_rigidbody.position + offset, hit.point)
                    : NoCollisionDistance;

            // if not on track flip distance
            if (!onTrack)
                distance = distance > RayDistance ? 0 : NoCollisionDistance;

            Debug.DrawRay(_rigidbody.position + offset, direction * RayDistance,
                distance < NoCollisionDistance ? Color.red : Color.green);

            return distance;
        }
    }
}