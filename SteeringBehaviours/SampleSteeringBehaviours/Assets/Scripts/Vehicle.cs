using System;
using UnityEngine;
using UnityEngine.UI;

public class Vehicle : MonoBehaviour
{
    public float MaxSpeed = 2f;
    public Transform TargetTransform;
    public Text SteeringBehaviourText;
    
    public Vector2 Position => transform.position;
    public Vector2 Velocity => transform.GetComponent<Rigidbody2D>().velocity;
    public Vector2 Target => TargetTransform.position;
    public float WanderJitter { get; } = 2f;
    public float WanderRadius { get; } = 2f;
    public float WanderDistance { get; } = 0.5f;

    private Rigidbody2D _rigidbody2D;
    private SteeringBehaviour _steeringBehaviour;

    private Func<Vehicle, Vector2> _steeringFunction;

    private void Start()
    {
        _rigidbody2D = transform.GetComponent<Rigidbody2D>();
        _steeringBehaviour = new SteeringBehaviour();
        _steeringFunction = _steeringBehaviour.Seek;
    }
    
    // Update is called once per frame
    private void FixedUpdate()
    {
        _rigidbody2D.AddForce(_steeringFunction(this));
        AdjustRotation();
        ChangeSteeringType();
        MoveTarget();
    }

    private void MoveTarget()
    {
        if (!TargetTransform.GetComponent<FollowScript>().enabled) 
            TargetTransform.position = _steeringBehaviour.WanderTarget;
    }

    private void ChangeSteeringType()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            _steeringFunction = _steeringBehaviour.Wander;
            TargetTransform.GetComponent<FollowScript>().enabled = false;
            SteeringBehaviourText.text = $"Current Steering Behaviour\nWander";
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            _steeringFunction = _steeringBehaviour.Arrive;
            TargetTransform.GetComponent<FollowScript>().enabled = true;
            SteeringBehaviourText.text = $"Current Steering Behaviour\nArrive";
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            _steeringFunction = _steeringBehaviour.Seek;
            TargetTransform.GetComponent<FollowScript>().enabled = true;
            SteeringBehaviourText.text = $"Current Steering Behaviour\nSeek";
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            _steeringFunction = _steeringBehaviour.Flee;
            TargetTransform.GetComponent<FollowScript>().enabled = true;
            SteeringBehaviourText.text = $"Current Steering Behaviour\nFlee";
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            TargetTransform.GetComponent<SpriteRenderer>().enabled =
                !TargetTransform.GetComponent<SpriteRenderer>().enabled;
        }
    }

    private void AdjustRotation()
    {
        var angle = Mathf.Atan2(Velocity.y, Velocity.x) * Mathf.Rad2Deg + 270f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}