using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Agent.Data
{
    [Serializable]
    public class Percept
    {
        public List<Vector3> VisibleAgents { get; }
        public List<Vector3> VisibleCollectables { get; }
        public List<Vector3> VisibleObstacles { get; }

        [SerializeField] private List<double> _normalizedWallDistances;
        [SerializeField] private Vector3 _normalizedVelocity;
        [SerializeField] private List<Vector2> _normalizedVisibleAgents = new List<Vector2>();
        [SerializeField] private List<Vector2> _normalizedVisibleCollectables = new List<Vector2>();
        [SerializeField] private List<Vector2> _normalizedVisibleObstacles = new List<Vector2>();

        private const int AgentCount = 0;
        private const int CollectableCount = 0;
        private const int ObstacleCount = 0;
        private readonly List<double> _wallDistances;
        private readonly Vector3 _velocity;

        public Percept(List<double> wallDistances, Vector3 velocity, List<Vector3> visibleAgents,
            List<Vector3> visibleCollectables, List<Vector3> visibleObstacles)
        {
            _wallDistances = wallDistances;
            _velocity = velocity;
            VisibleAgents = visibleAgents;
            VisibleCollectables = visibleCollectables;
            VisibleObstacles = visibleObstacles;
        }

        /// <summary>
        /// Converts a normalized percept into a double array.
        ///  0 -  1    - velocity: x, z
        ///  2 - 13    - 12 wall distances, clockwise TODO list angles
        /// 14 - 21    - 4 closest visible agents: x, z
        /// 22 - 25    - 2 closest visible collectables: x, z
        /// 26 - 29    - 2 closest visible obstacles : x, z
        /// </summary>
        /// <returns>Double representation of normalized percept.</returns>
        public double[] ToDoubleArray()
        {
            var arr = new List<double> {_normalizedVelocity.x, _normalizedVelocity.z};
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

        public void Normalize(float maxSpeed, float sensorDistance, Transform transform, float viewRadius)
        {
            _normalizedVelocity = transform.InverseTransformDirection(_velocity) / maxSpeed;

            Debug.Log($"vel: {_normalizedVelocity}");
            
            _normalizedWallDistances = _wallDistances
                .Select(dist => dist / sensorDistance)
                .ToList();
            
            
            Debug.Log($"dists: ({string.Join(",", _normalizedWallDistances.Select(p=>p.ToString("0.0000")))})");
            
            _normalizedVisibleAgents = VisibleAgents
                .Select(a =>
                    new Vector2(transform.InverseTransformPoint(a).x * transform.localScale.x,
                        transform.InverseTransformPoint(a).z * transform.localScale.z) / viewRadius)
                .Take(AgentCount)
                .ToList();

            _normalizedVisibleCollectables = VisibleCollectables
                .Select(c =>
                    new Vector2(transform.InverseTransformPoint(c).x * transform.localScale.x,
                        transform.InverseTransformPoint(c).z * transform.localScale.z) / viewRadius)
                .Take(CollectableCount)
                .ToList();

            _normalizedVisibleObstacles = VisibleObstacles
                .Select(o =>
                    new Vector2(transform.InverseTransformPoint(o).x * transform.localScale.x,
                        transform.InverseTransformPoint(o).z * transform.localScale.z) / viewRadius)
                .Take(ObstacleCount)
                .ToList();
        }
    }
}