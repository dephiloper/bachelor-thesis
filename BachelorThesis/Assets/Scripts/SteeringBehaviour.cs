using UnityEngine;

public static class SteeringBehaviour
{
    public static Vector3 Seek(Vector3 position, Vector3 target, Vector3 velocity, float maxSpeed)
    {
        var desiredVelocity = target.ToVector2() - position.ToVector2(); 
        desiredVelocity = desiredVelocity.normalized * maxSpeed;
        var steering = desiredVelocity - velocity.ToVector2();
        return new Vector3(steering.x, 0, steering.y);
    }
}