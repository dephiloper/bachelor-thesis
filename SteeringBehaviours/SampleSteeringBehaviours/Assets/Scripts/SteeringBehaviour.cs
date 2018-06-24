using UnityEngine;

public class SteeringBehaviour
{
    public Vector2 WanderTarget { get; private set; }

    public SteeringBehaviour()
    {
        WanderTarget = new Vector2(Random.value * 5f - 2.5f, Random.value * 5f - 2.5f);
    }

    public Vector2 Seek(Vehicle vehicle)
    {
        var desiredVelocity = vehicle.Target - vehicle.Position; // calulate desired velocity 
        desiredVelocity.Normalize();
        desiredVelocity *= vehicle.MaxSpeed;
        return desiredVelocity - vehicle.Velocity;
    }

    public Vector2 Arrive(Vehicle vehicle)
    {
        var desiredVelocity = vehicle.Target - vehicle.Position; // calulate desired velocity 
        var distance = Vector2.Distance(vehicle.Target, vehicle.Position);
        desiredVelocity.Normalize();

        const float speedReduceDistance = 4f;

        if (distance < speedReduceDistance)
            desiredVelocity *= (distance * vehicle.MaxSpeed) / speedReduceDistance;
        else
            desiredVelocity *= vehicle.MaxSpeed;

        return desiredVelocity - vehicle.Velocity;
    }

    public Vector2 Flee(Vehicle vehicle)
    {
        var distance = Vector2.Distance(vehicle.Target, vehicle.Position);
        const float fleeDistance = 4f;

        if (distance >= fleeDistance)
            return Vector2.zero;

        var desiredVelocity = vehicle.Position - vehicle.Target; // calulate desired velocity 
        desiredVelocity = desiredVelocity.normalized * vehicle.MaxSpeed;
        return desiredVelocity - vehicle.Velocity;
    }

    public Vector2 Wander(Vehicle vehicle)
    {
        WanderTarget += new Vector2((Random.value * 2f - 1f) * vehicle.WanderJitter, (Random.value * 2f - 1f) * vehicle.WanderJitter);
        WanderTarget = WanderTarget.normalized * vehicle.WanderRadius;

        var targetLocal = WanderTarget + new Vector2(vehicle.WanderDistance, 0);
        return targetLocal - vehicle.Position;
    }
}
