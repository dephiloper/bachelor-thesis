using System.Linq.Expressions;
using UnityEngine;

public class BallResetScript : MonoBehaviour
{
    public float Speed = 1f;
    private Rigidbody _ballRb;
    private Vector3 _startPos;
    
    private void OnEnable()
    {
        _ballRb = GetComponent<Rigidbody>();
        _startPos = transform.position;
        InvokeRepeating(nameof(RelocateBall), 0f, 4.0f); 
        InvokeRepeating(nameof(LaunchBall), 2.0f, 4.0f);
    }
    
    private void RelocateBall()
    {
        transform.position = _startPos;
        _ballRb.angularVelocity = Vector3.zero;
        _ballRb.transform.rotation = Quaternion.identity;
        _ballRb.useGravity = false;
        _ballRb.isKinematic = true;
    }

    private void LaunchBall()
    {
        _ballRb.useGravity = true;
        _ballRb.isKinematic = false;
        _ballRb.AddForce(Vector3.forward * Speed);
    }
}