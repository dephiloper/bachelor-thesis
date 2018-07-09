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
        [SerializeField] private List<Vector3> _normalizedVisibleAgents = new List<Vector3>();
        [SerializeField] private List<Vector3> _normalizedVisibleCollectables = new List<Vector3>();
        [SerializeField] private List<Vector3> _normalizedVisibleObstacles = new List<Vector3>();

        private const int AgentCount = 4;
        private const int CollectableCount = 2;
        private const int ObstacleCount = 2;
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
            //arr.AddRange(FilterEnvironmentals(_normalizedVisibleAgents, AgentCount));
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
        private static IEnumerable<double> FilterEnvironmentals(IReadOnlyList<Vector3> environmentals, int count)
        {
            var envs = new List<double>();
            for (var i = 0; i < count; i++)
                if (i >= environmentals.Count)
                    envs.AddRange(new double[] {-1, -1});
                else
                    envs.AddRange(new double[] { environmentals[i].x, environmentals[i].z });

            return envs;
        }

        public void Normalize(float maxSpeed, float sensorDistance, Vector3 position, float viewRadius)
        {
            _normalizedVelocity = _velocity / maxSpeed;

            _normalizedWallDistances = _wallDistances
                .Select(dist => dist / sensorDistance)
                .ToList();

            _normalizedVisibleAgents = VisibleAgents
                .Select(agent => (agent - position) / viewRadius)
                .Take(AgentCount)
                .ToList();
            
            _normalizedVisibleCollectables = VisibleCollectables
                .Select(collectable => (collectable - position) / viewRadius)
                .Take(CollectableCount)
                .ToList();
            
            _normalizedVisibleObstacles = VisibleObstacles
                .Select(obstacle => (obstacle - position) / viewRadius)
                .Take(ObstacleCount)
                .ToList();
        }
    }
}