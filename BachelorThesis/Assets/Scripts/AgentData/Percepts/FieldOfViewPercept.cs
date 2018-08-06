using System;
using System.Collections.Generic;
using System.Linq;
using AgentData.Base;
using AgentData.Sensors;
using AgentImpl;
using UnityEngine;

namespace AgentData.Percepts
{
    [Serializable]
    public class FieldOfViewPercept : IPercept
    {
        private readonly List<double> _wallDistances;
        public List<Vector3> VisibleAgents { get; }
        public List<Vector3> VisibleCollectables { get; }
        public List<Vector3> VisibleObstacles { get; }

        [SerializeField] private Vector3 _normalizedVelocity;
        [SerializeField] private List<double> _normalizedWallDistances;
        [SerializeField] private List<Vector2> _normalizedVisibleAgents = new List<Vector2>();
        [SerializeField] private List<Vector2> _normalizedVisibleCollectables = new List<Vector2>();
        [SerializeField] private List<Vector2> _normalizedVisibleObstacles = new List<Vector2>();

        private const int AgentCount = 2;
        private const int CollectableCount = 2;
        private const int ObstacleCount = 0;

        private readonly Agent _agent;
        private readonly FieldOfViewSensor _sensor;

        public FieldOfViewPercept(Agent agent, Sensor sensor, List<double> wallDistances, List<Vector3> visibleAgents,
            List<Vector3> visibleCollectables, List<Vector3> visibleObstacles)
        {
            _agent = agent;
            _sensor = (FieldOfViewSensor)sensor;
            _wallDistances = wallDistances;
            VisibleAgents = visibleAgents;
            VisibleCollectables = visibleCollectables;
            VisibleObstacles = visibleObstacles;
        }

        /// <summary>
        /// Converts a normalized percept into a double array.
        ///  0 -  1    - velocity: x, z
        ///  2 - 13    - 12 wall distances, clockwise TODO list angles
        /// 14 - 21    - 4 closest visible agents: x, z//
        /// 22 - 25    - 2 closest visible collectables: x, z
        /// 26 - 29    - 2 closest visible obstacles : x, z
        /// </summary>
        /// <returns>Double representation of normalized percept.</returns>
        public double[] ToDoubleArray()
        {
            var arr = new List<double>
            {
                _normalizedVelocity.x,
                _normalizedVelocity.z,
                _agent.OnTrack ? 0 : 1,
                _agent.RightDirection ? 0 : 1
            };
            arr.AddRange(_normalizedWallDistances);
            arr.AddRange(FilterEnvironmentals(_normalizedVisibleAgents, AgentCount));
            arr.AddRange(FilterEnvironmentals(_normalizedVisibleCollectables, CollectableCount));
            arr.AddRange(FilterEnvironmentals(_normalizedVisibleObstacles, ObstacleCount));
            return arr.ToArray();
        }

        /// <summary>
        /// Specify the environmentals and the count of the filtered environmentals.
        /// This methods fills empty environmentals with -1, -1 which is a location behind the agent.
        /// </summary>
        /// <param name="count">The count of possible visible environmentals.</param>
        /// <param name="environmentals">The list of environmentals.</param>
        /// <returns>The double representation of the visible environmentals.</returns>
        private static IEnumerable<double> FilterEnvironmentals(IReadOnlyList<Vector2> environmentals, int count)
        {
            var envs = new List<double>();
            for (var i = 0; i < count; i++)
                if (i >= environmentals.Count)
                    envs.AddRange(new double[] {-1, -1});
                else
                    envs.AddRange(new double[] {environmentals[i].x, environmentals[i].y});

            return envs;
        }

        public void Normalize()
        {
            if (_sensor == null)
                throw new NullReferenceException(
                    $"SensorType for {GetType().Name} should be {nameof(DistanceOnlySensor)}");

            _normalizedVelocity =
                _agent.transform.InverseTransformDirection(_agent.GetComponent<Rigidbody>().velocity) /
                _agent.MaxSpeed * Agent.SpeedIncreaseFactor;

            _normalizedWallDistances = _wallDistances
                .Select(dist => dist / _sensor.Range)
                .ToList();

            /*_normalizedVisibleAgents = VisibleAgents
                .Select(a =>
                    new Vector2(_agent.transform.InverseTransformPoint(a).x * _agent.transform.localScale.x,
                        _agent.transform.InverseTransformPoint(a).z * _agent.transform.localScale.z) /
                    _sensor.ViewRadius)
                .Take(AgentCount)
                .ToList();*/

            _normalizedVisibleAgents = Enumerable.Repeat(Vector2.zero, AgentCount).ToList();

            _normalizedVisibleCollectables = VisibleCollectables
                .Select(c =>
                    new Vector2(_agent.transform.InverseTransformPoint(c).x * _agent.transform.localScale.x,
                        _agent.transform.InverseTransformPoint(c).z * _agent.transform.localScale.z) /
                    _sensor.ViewRadius)
                .Take(CollectableCount)
                .ToList();

            _normalizedVisibleObstacles = VisibleObstacles
                .Select(o =>
                    new Vector2(_agent.transform.InverseTransformPoint(o).x * _agent.transform.localScale.x,
                        _agent.transform.InverseTransformPoint(o).z * _agent.transform.localScale.z) /
                    _sensor.ViewRadius)
                .Take(ObstacleCount)
                .ToList();

            //Debug.Log($"coll: ({string.Join(",", _normalizedVisibleCollectables.Select(x => x.ToString("0.0000")))})");
            //Debug.Log($"obst: ({string.Join(",", _normalizedVisibleObstacles.Select(x => x.ToString("0.0000")))})");

            //Debug.Log($"vel: {_normalizedVelocity}");
            //Debug.Log($"dists: ({string.Join(",", _normalizedWallDistances.Select(p => p.ToString("0.0000")))})");
        }
    }
}