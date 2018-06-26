using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct Percept
{
    public List<double> WallDistances { get; set; }
    public Vector3 Velocity { private get; set; }

    public double[] ToDoubleArray()
    {
        var arr = WallDistances.ToList();
        arr.Add(Velocity.x);
        arr.Add(Velocity.z);

        return arr.ToArray();
    }
}