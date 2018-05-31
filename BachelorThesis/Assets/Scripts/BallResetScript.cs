using System.Linq.Expressions;
using UnityEngine;

public class BallResetScript : MonoBehaviour
{
    private Rigidbody _ballRb;
    
    private void OnEnable()
    {
        _ballRb = GetComponent<Rigidbody>();
        InvokeRepeating(nameof(RelocateBall), 0f, 4.0f); 
        InvokeRepeating(nameof(LaunchBall), 2.0f, 4.0f);
    }
    
    private void RelocateBall()
    {
        transform.position = new Vector3(0, 1.1f, 1);
        _ballRb.angularVelocity = Vector3.zero;
        _ballRb.transform.rotation = Quaternion.identity;
        _ballRb.useGravity = false;
        _ballRb.isKinematic = true;
    }

    private void LaunchBall()
    {
        _ballRb.useGravity = true;
        _ballRb.isKinematic = false;
        _ballRb.AddForce(Vector3.back * 1.5f);
    }
}