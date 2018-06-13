using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallLaunchScript : MonoBehaviour
{
    public GameObject BallGameObject;
    public Transform Target;


    // Use this for initialization
    void Start()
    {
        Invoke(nameof(Go), 2f);
        
    }

    void Go()
    {
        ThrowBallAtTargetLocation(Target.position, 10f);
    }

    // Throws ball at location with regards to gravity (assuming no obstacles in path) and initialVelocity (how hard to throw the ball)
    private void ThrowBallAtTargetLocation(Vector3 targetLocation, float initialVelocity)
    {
        var direction = (targetLocation - transform.position).normalized;
        var distance = Vector3.Distance(targetLocation, transform.position);

        var firingElevationAngle = FiringElevationAngle(Physics.gravity.magnitude, distance, initialVelocity);
        var elevation = Quaternion.AngleAxis(firingElevationAngle, transform.right) * transform.up;
        var directionAngle = AngleBetweenAboutAxis(transform.forward, direction, transform.up);
        var velocity = Quaternion.AngleAxis(directionAngle, transform.up) * elevation * initialVelocity;

        // ballGameObject is object to be thrown
        BallGameObject.GetComponent<Rigidbody>().AddForce(velocity, ForceMode.VelocityChange);
    }

    // Helper method to find angle between two points (v1 & v2) with respect to axis n
    private static float AngleBetweenAboutAxis(Vector3 v1, Vector3 v2, Vector3 n)
    {
        return Mathf.Atan2(
                   Vector3.Dot(n, Vector3.Cross(v1, v2)),
                   Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
    }

    // Helper method to find angle of elevation (ballistic trajectory) required to reach distance with initialVelocity
    // Does not take wind resistance into consideration.
    private static float FiringElevationAngle(float gravity, float distance, float initialVelocity)
    {
        var angle = 0.5f * Mathf.Asin((gravity * distance) / (initialVelocity * initialVelocity)) * Mathf.Rad2Deg;
        return angle;
    }
}