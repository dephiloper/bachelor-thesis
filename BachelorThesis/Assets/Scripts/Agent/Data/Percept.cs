using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Agent.Data
{
    public class Percept
    {
        public List<Vector3> VisibleCollectables { get; private set; }

        private List<double> _normalizedWallDistances;
        private Vector3 _normalizedVelocity;
        private List<Vector3> _normalizedVisibleCollectables = new List<Vector3>();

        private const int ClosestCollectablesCount = 2;
        private readonly List<double> _wallDistances;
        private readonly Vector3 _velocity;

        public Percept(List<double> wallDistances, Vector3 velocity, List<Vector3> visibleCollectables)
        {
            _wallDistances = wallDistances;
            _velocity = velocity;
            VisibleCollectables = visibleCollectables;
        }

        public double[] ToDoubleArray()
        {
            var arr = new List<double>();
            arr.AddRange(new double[] {_normalizedVelocity.x, _normalizedVelocity.z});
            arr.AddRange(_normalizedWallDistances);

            for (var i = 0; i < ClosestCollectablesCount; i++)
            {
                if (i >= _normalizedVisibleCollectables.Count)
                    arr.AddRange(new double[]
                    {
                        -1,
                        -1
                    });
                else
                    arr.AddRange(new double[]
                    {
                        _normalizedVisibleCollectables[i].x,
                        _normalizedVisibleCollectables[i].z
                    });
            }

            if (arr.Count != 18)
                throw new ArgumentOutOfRangeException(nameof(ArgumentOutOfRangeException),
                    $"Percept count should always be 18, but is  {arr.Count}");

            return arr.ToArray();
        }

        public void Normalize(float maxSpeed, float sensorDistance, Vector3 position, float viewRadius)
        {
            _normalizedVelocity = _velocity / maxSpeed;

            _normalizedWallDistances = _wallDistances
                .Select(dist => dist / sensorDistance)
                .ToList();

            _normalizedVisibleCollectables = VisibleCollectables
                .Select(collectable => (collectable - position) / viewRadius)
                .Take(2)
                .ToList();
        }
    }
}