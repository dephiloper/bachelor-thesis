using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Percept
{
    public List<double> WallDistances { get; set; }
    public List<double> PlayerDistances { get; set; }
    public Vector3 Velocity { private get; set; }

    public Percept()
    {	
    }
	
    public Percept(double[] values)
    {
        WallDistances = new List<double>(values.Take(3));
        PlayerDistances = new List<double>(values.Skip(3).Take(3));
        Velocity = new Vector3(
            (float)values[WallDistances.Count + PlayerDistances.Count], 
            0, 
            (float)values[WallDistances.Count + PlayerDistances.Count]);
    }

    public double[] ToDoubleArray()
    {
        var arr = WallDistances./*Concat(PlayerDistances).*/ToList();
        arr.Add(Velocity.x);
        arr.Add(Velocity.z);

        return arr.ToArray();
    }
}