using Extensions;
using UnityEngine;

namespace Car
{
    public static class SteeringBehavior
    {
        public static Vector3 Seek(Vector3 position, Vector3 target, Vector3 velocity, float maxSpeed)
        {
            var desiredVelocity = target.ToVector2() - position.ToVector2(); 
            desiredVelocity.Normalize();
            desiredVelocity *= maxSpeed;
            return (desiredVelocity - velocity.ToVector2()).ToVector3();
        }
    }
}