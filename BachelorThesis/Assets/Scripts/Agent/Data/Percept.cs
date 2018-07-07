using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Agent.Data
{
    public class Percept
    {
        public List<double> WallDistances;
        public List<double> NormalizedWallDistances;
        public Vector3 Velocity;
        public Vector3 NormalizedVelocity;
        public List<Vector3> ClosestCollectables = new List<Vector3>();
        public List<Vector3> NormalizedClosestCollectables = new List<Vector3>();

        public double[] ToDoubleArray()
        {
            var arr = NormalizedWallDistances;
            arr.Add(NormalizedVelocity.x);
            arr.Add(NormalizedVelocity.z);
            foreach (var collectable in NormalizedClosestCollectables)
            {
                arr.Add(collectable.x);
                arr.Add(collectable.z);
            }

            return arr.ToArray();
        }
    }
}