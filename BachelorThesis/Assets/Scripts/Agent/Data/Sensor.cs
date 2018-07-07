using System.Collections.Generic;
using System.Linq;
using Environment;
using UnityEngine;

namespace Agent.Data
{
    public class Sensor
    {
        private const float RayDistance = 10f;

        private readonly LayerMask _wallMask;
        private readonly Rigidbody _rigidbody;
        private readonly Transform _transform;
        private readonly AgentEditorProperties _editorProps;

        public Sensor(AgentBehaviour agentBehaviour)
        {
            _transform = agentBehaviour.transform;
            _rigidbody = agentBehaviour.GetComponent<Rigidbody>();
            _editorProps = agentBehaviour.EditorProperties;
            _wallMask = LayerMask.GetMask("Wall", "Obstacle");
        }

        public Percept PerceiveEnvironment(bool onTrack)
        {
            var percept = new Percept
            {
                Velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z),
                WallDistances = new List<double>
                {
                    CalculateDistanceWithRay((-_transform.right).normalized, _wallMask, onTrack),
                    CalculateDistanceWithRay((_transform.forward * 0.5f - _transform.right).normalized, _wallMask,
                        onTrack),
                    CalculateDistanceWithRay((_transform.forward - _transform.right).normalized, _wallMask, onTrack),
                    CalculateDistanceWithRay((_transform.forward - _transform.right * 0.5f).normalized, _wallMask,
                        onTrack),
                    CalculateDistanceWithRay((_transform.forward - _transform.right * 0.25f).normalized, _wallMask,
                        onTrack),
                    CalculateDistanceWithRay(_transform.forward.normalized, _wallMask, onTrack,
                        -_transform.right * 0.1f),
                    CalculateDistanceWithRay(_transform.forward.normalized, _wallMask, onTrack,
                        _transform.right * 0.1f),
                    CalculateDistanceWithRay((_transform.forward + _transform.right * 0.25f).normalized, _wallMask,
                        onTrack),
                    CalculateDistanceWithRay((_transform.forward + _transform.right * 0.5f).normalized, _wallMask,
                        onTrack),
                    CalculateDistanceWithRay((_transform.forward + _transform.right).normalized, _wallMask, onTrack),
                    CalculateDistanceWithRay((_transform.forward * 0.5f + _transform.right).normalized, _wallMask,
                        onTrack),
                    CalculateDistanceWithRay((_transform.right).normalized, _wallMask, onTrack)
                },
                ClosestCollectables = FindVisibleCollectables()
            };

            percept.NormalizedVelocity = percept.Velocity / _editorProps.MaxSpeed;
            
            percept.NormalizedWallDistances = percept.WallDistances
                .Select(x => x / RayDistance)
                .ToList();
            
            //TODO normalize Collectables right
            percept.NormalizedClosestCollectables = percept.ClosestCollectables
                .Select(x => _transform.InverseTransformPoint(x) / _editorProps.ViewRadius)
                .ToList();

            return percept;
        }

        private List<Vector3> FindVisibleCollectables()
        {
            var visibleCollectables = new List<Vector3>();

            var collectablesInViewRadius = Physics.OverlapSphere(_transform.position, _editorProps.ViewRadius,
                LayerMask.GetMask("Collectable"));

            foreach (var collectable in collectablesInViewRadius)
            {
                var dir = (collectable.transform.position - _transform.position).normalized;
                if (Vector3.Angle(_transform.forward, dir) < _editorProps.ViewAngle / 2)
                {
                    var dist = Vector3.Distance(collectable.transform.position, _transform.position);
                    if (!Physics.Raycast(_transform.position, dir, dist, LayerMask.GetMask("Obstacle")))
                        visibleCollectables.Add(collectable.GetComponent<CollectableBehaviour>().transform.position);
                }
            }

            return visibleCollectables;
        }

        private double CalculateDistanceWithRay(Vector3 direction, LayerMask layerMask, bool onTrack,
            Vector3 offset = new Vector3())
        {
            RaycastHit hit;

            // normally max distance is RayDistance, so times 2 means like free to go there
            var distance =
                Physics.Raycast(_rigidbody.position + offset, direction, out hit, RayDistance, layerMask)
                    ? Vector3.Distance(_rigidbody.position + offset, hit.point)
                    : RayDistance;

            // if not on track flip distance
            if (!onTrack)
                distance = distance > RayDistance ? 0 : RayDistance;

            Debug.DrawRay(_rigidbody.position + offset, direction * RayDistance,
                distance < RayDistance ? Color.red : Color.green);

            return distance;
        }
    }
}