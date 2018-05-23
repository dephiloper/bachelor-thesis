using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class VectorDna : IDna<Vector2>
{
    public Vector2[] Genes { get; set; }
    public float Fitness { get; set; }
    public int Punishment { get; set; }

    public VectorDna(int lifeSpan)
    {
        Genes = new Vector2[lifeSpan];
            
        for (var i = 0; i < Genes.Length; i++)
        {
            Genes[i] = new Vector2(Random.value * 2 - 1, Random.value * 2 - 1) * 20f;
        }
    }
    
    public void CalculateFitness(Vector2 position, Vector2 target)
    {
        Fitness += 1 / Vector2.Distance(position, target) + Punishment    ;
    }

    public IDna<Vector2> Crossover(IDna<Vector2> partner)
    {
        var child = new VectorDna(Genes.Length);
        var midpoint = Convert.ToInt32(Random.value * Genes.Length);

        for (var i = 0; i < Genes.Length; i++)
        {
            if (i < midpoint) child.Genes[i] = Genes[i];
            else child.Genes[i] = partner.Genes[i];
        }

        return child;
    }

    public void Mutate(float mutationRate)
    {
        for (var i = 0; i < Genes.Length; i++)
        {
            if (Random.value < mutationRate) {
                Genes[i] = new Vector2(Random.value * 2 - 1, Random.value * 2 - 1) * 20f;
            }
        }
    }
}