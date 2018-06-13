using UnityEngine;

public class SteeringBehaviour
{
    public Vector2 WanderTarget { get; private set; }

    public SteeringBehaviour()
    {
        WanderTarget = new Vector2(Random.value * 5f - 2.5f, Random.value * 5f - 2.5f);
    }

    public Vector2 Seek(Ant ant)
    {
        var v = new Vector2(3, 2);
        v.Normalize();
        var desiredVelocity = ant.Target - ant.Position; // calulate desired velocity 
        desiredVelocity.Normalize();
        desiredVelocity *= ant.MaxSpeed;
        return desiredVelocity - ant.Velocity;
    }

    public Vector2 Arrive(Ant ant)
    {
        var desiredVelocity = ant.Target - ant.Position; // calulate desired velocity 
        var distance = Vector2.Distance(ant.Target, ant.Position);
        desiredVelocity.Normalize();

        const float speedReduceDistance = 4f;

        if (distance < speedReduceDistance)
            desiredVelocity *= (distance * ant.MaxSpeed) / speedReduceDistance;
        else
            desiredVelocity *= ant.MaxSpeed;

        return desiredVelocity - ant.Velocity;
    }

    public Vector2 Flee(Ant ant)
    {
        var distance = Vector2.Distance(ant.Target, ant.Position);
        const float fleeDistance = 4f;

        if (distance >= fleeDistance)
            return Vector2.zero;

        var desiredVelocity = ant.Position - ant.Target; // calulate desired velocity 
        desiredVelocity = desiredVelocity.normalized * ant.MaxSpeed;
        return desiredVelocity - ant.Velocity;
    }

    public Vector2 Wander(Ant ant)
    {
        WanderTarget += new Vector2((Random.value * 2f - 1f) * ant.WanderJitter, (Random.value * 2f - 1f) * ant.WanderJitter);
        WanderTarget = WanderTarget.normalized * ant.WanderRadius;

        var targetLocal = WanderTarget + new Vector2(ant.WanderDistance, 0);
        return targetLocal - ant.Position;
    }
}