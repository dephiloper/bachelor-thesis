using System.Linq.Expressions;
using UnityEngine;

public class BallResetScript : MonoBehaviour
{
    public Vector3 StartPos;
    public float LaunchSpeed = 1.5f;

    private Rigidbody _ballRb;
    
    private void OnEnable()
    {
        _ballRb = GetComponent<Rigidbody>();
        InvokeRepeating(nameof(RelocateBall), 0f, 4.0f); 
        InvokeRepeating(nameof(LaunchBall), 2.0f, 4.0f);
        StartPos = transform.position;
    }
    
    private void RelocateBall()
    {
        transform.position = StartPos;
        _ballRb.angularVelocity = Vector3.zero;
        _ballRb.transform.rotation = Quaternion.identity;
        _ballRb.isKinematic = true;
    }

    private void LaunchBall()
    {
        _ballRb.isKinematic = false;
        _ballRb.AddForce(Vector3.back * LaunchSpeed);
    }
}