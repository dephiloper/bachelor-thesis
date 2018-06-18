using UnityEngine;

public class BallResetScript : MonoBehaviour
{
    /// <summary>
    /// speed that is applied to the ball when the launch ball method gets invoked
    /// </summary>
    public float Speed = 1f;
    
    /// <summary>
    /// the rigid body of the table tennis ball
    /// </summary>
    private Rigidbody _ballRigidbody;
    
    /// <summary>
    /// the start position in the global space
    /// </summary>
    private Vector3 _startPosition;
    
    private void OnEnable()
    {
        _ballRigidbody = GetComponent<Rigidbody>();
        _startPosition = transform.position;
        InvokeRepeating(nameof(ResetBallPosition), 0f, 4.0f); 
        InvokeRepeating(nameof(LaunchBall), 2.0f, 4.0f);
    }

    /// <summary>
    /// moves the ball position to its initial value with initial rotation directions
    /// </summary>
    private void ResetBallPosition()
    {
        transform.position = _startPosition;
        _ballRigidbody.angularVelocity = Vector3.zero;
        _ballRigidbody.transform.rotation = Quaternion.identity;
        _ballRigidbody.transform.Rotate(Vector3.up * (Random.value - 0.5f) * 20);
        _ballRigidbody.useGravity = false;
        _ballRigidbody.isKinematic = true;
    }

    /// <summary>
    /// applies a force to the ball and enables gravity
    /// </summary>
    private void LaunchBall()
    {
        _ballRigidbody.useGravity = true;
        _ballRigidbody.isKinematic = false;
        _ballRigidbody.AddForce(transform.forward * Speed);
    }
}