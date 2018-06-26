using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Agent
{
    // only for displaying in the inspector
    public float MaxSpeed = 5f;
    public float Score;
    public float Speed;
    public float TurnSpeed = 2f;
    
    public Transform Transform { get; }

    protected readonly Rigidbody Rigidbody;
    protected Percept Percept;
    protected bool OnTrack;
    
    private readonly DriverSensor _driverSensor;
    private readonly float _onTrackSpeed;
    private readonly float _offTrackSpeed;

    protected Agent(Transform agentTransform)
    {
        Transform = agentTransform;
        Rigidbody = Transform.GetComponent<Rigidbody>();
        _driverSensor = new DriverSensor(agentTransform);

        _onTrackSpeed = MaxSpeed;
        _offTrackSpeed = MaxSpeed / 4f;
    }

    public virtual void Compute()
    {
        OnTrack = IsOnTrack();
        Percept = _driverSensor.PerceiveEnvironment(OnTrack);
        MaxSpeed = OnTrack ? _onTrackSpeed : _offTrackSpeed;
        Speed = new Vector2(Rigidbody.velocity.x, Rigidbody.velocity.z).magnitude;
    }

    protected void PerformAction(Action action)
    {
        if (action.AccelerateForward)
            Rigidbody.AddForce(Transform.forward * MaxSpeed);
        if (action.AccelerateBackward)
            Rigidbody.AddForce(-Transform.forward * MaxSpeed * 0.1f);
        if (action.SteerLeft)
            Rigidbody.MoveRotation(Quaternion.Euler(Transform.rotation.eulerAngles - Transform.up * TurnSpeed));
        if (action.SteerRight)
            Rigidbody.MoveRotation(Quaternion.Euler(Transform.rotation.eulerAngles + Transform.up * TurnSpeed));
    }

    private bool IsOnTrack()
    {
        if (Physics.Raycast(Transform.position, -Transform.up, float.MaxValue, LayerMask.GetMask("Track")))
        {
            return !Physics.Raycast(Transform.position, -Transform.up, float.MaxValue,
                LayerMask.GetMask("Wall"));
        }

        return false;
    }
}