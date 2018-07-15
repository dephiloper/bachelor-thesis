using System.Collections.Generic;
using System.Linq;
using Environment;
using Train;
using UnityEngine;

namespace Agent.Data
{
    public class Sensor
    {
        private readonly LayerMask _wallMask;
        private readonly Rigidbody _rigidbody;
        private readonly Transform _transform;
        private readonly AgentEditorProperties _editorProps;
        private bool _onTrack;

        public Sensor(AgentBehaviour agentBehaviour)
        {
            _transform = agentBehaviour.transform;
            _rigidbody = agentBehaviour.GetComponent<Rigidbody>();
            _editorProps = agentBehaviour.EditorProperties;
            _wallMask = LayerMask.GetMask("Wall");
        }

        public Percept PerceiveEnvironment(bool onTrack)
        {
            _onTrack = onTrack;
            
            var wallDistances = new List<double>
            {
                CalculateDistanceWithRay((-_transform.right).normalized),
                CalculateDistanceWithRay((_transform.forward * 0.5f - _transform.right).normalized),
                CalculateDistanceWithRay((_transform.forward - _transform.right).normalized),
                CalculateDistanceWithRay((_transform.forward - _transform.right * 0.5f).normalized),
                CalculateDistanceWithRay((_transform.forward - _transform.right * 0.25f).normalized),
                CalculateDistanceWithRay((_transform.forward - _transform.right * 0.125f).normalized),
                CalculateDistanceWithRay(_transform.forward.normalized, -_transform.right * 0.1f),
                CalculateDistanceWithRay(_transform.forward.normalized, _transform.right * 0.1f),
                CalculateDistanceWithRay((_transform.forward + _transform.right * 0.125f).normalized),
                CalculateDistanceWithRay((_transform.forward + _transform.right * 0.25f).normalized),
                CalculateDistanceWithRay((_transform.forward + _transform.right * 0.5f).normalized),
                CalculateDistanceWithRay((_transform.forward + _transform.right).normalized),
                CalculateDistanceWithRay((_transform.forward * 0.5f + _transform.right).normalized),
                CalculateDistanceWithRay((_transform.right).normalized),
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
                Physics.OverlapSphere(_transform.position, _editorProps.ViewRadius, envMask);

            environmentalsInViewRadius = environmentalsInViewRadius.OrderBy(env =>
                Vector3.Distance(_transform.position, env.transform.position)).ToArray();

            foreach (var environmental in environmentalsInViewRadius)
            {
                var dir = (environmental.transform.position - _transform.position).normalized;
                if (Vector3.Angle(_transform.forward, dir) < _editorProps.ViewAngle / 2)
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

            // normally max distance is RayDistance, so times 2 means like free to go there
            var distance =
                Physics.Raycast(_rigidbody.position + offset, direction, out hit, _editorProps.SensorDistance,
                    _wallMask)
                    ? Vector3.Distance(_rigidbody.position + offset, hit.point)
                    : _editorProps.SensorDistance;

            // if not on track flip distance
            if (!_onTrack)
                distance = distance >= _editorProps.SensorDistance ? 0 : _editorProps.SensorDistance;

            if (_editorProps.ShowSensors)
                Debug.DrawRay(_rigidbody.position + offset, direction * distance,
                    distance < _editorProps.SensorDistance ? Color.red : Color.green);

            return distance;
        }
    }
}