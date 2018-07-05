using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Agent.Data
{
    public class Percept
    {
        public List<double> WallDistances;
        public Vector3 Velocity;

        public double[] ToDoubleArray()
        {
            var arr = WallDistances.ToList();
            arr.Add(Velocity.x);
            arr.Add(Velocity.z);

            return arr.ToArray();
        }
    }
}