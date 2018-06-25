using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public Brain Brain { get; set; }

    [SerializeField] private bool _isHuman = true;

    [SerializeField] private float _speed = 5f;

    [SerializeField] private float _turnSpeed = 2f;

    private float _onTrackSpeed;
    private float _offTrackSpeed;
    private Rigidbody _rigidbody;
    private DriverSensor _driverSensor;
    private const float ScoreReduction = 0.05f;

    public List<int> ReachedWaypointIds;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _driverSensor = new DriverSensor(transform);
        ReachedWaypointIds = new List<int>();

        _onTrackSpeed = _speed;
        _offTrackSpeed = _speed / 4f;
        var r = GetComponent<MeshRenderer>();
        var c = r.material.color;
        c.a = 0.5f;
        r.material.color = c;
    }

    private void FixedUpdate()
    {
        var action = new Action();

        if (_isHuman)
            action = new Action(Input.GetAxisRaw("P1Horizontal"), Input.GetAxisRaw("P1Vertical"));
        else if (Brain != null && _rigidbody)
        {
            var percept = _driverSensor.PerceiveEnvironment(IsOnTrack());
            action = Brain.Think(percept);
            _speed = _offTrackSpeed;

            if (IsOnTrack())
            {
                var vel = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.z).magnitude;
                _speed = _onTrackSpeed;
                Brain.Score += Convert.ToInt32(percept.WallDistances.Sum() * vel * ScoreReduction);
            }
        }
        
        PerformActions(action);
    }


    private void PerformActions(Action action)
    {
        if (action.AccelerateForward)
            _rigidbody.AddForce(transform.forward * _speed);
        if (action.AccelerateBackward)
            _rigidbody.AddForce(-transform.forward * _speed * 0.1f);
        if (action.SteerLeft)
            _rigidbody.MoveRotation(Quaternion.Euler(transform.rotation.eulerAngles - transform.up * _turnSpeed));
        if (action.SteerRight)
            _rigidbody.MoveRotation(Quaternion.Euler(transform.rotation.eulerAngles + transform.up * _turnSpeed));
    }

    private bool IsOnTrack()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, -transform.up, out hit, float.MaxValue, LayerMask.GetMask("Track")))
        {
            return !Physics.Raycast(transform.position, -transform.up, out hit, float.MaxValue,
                LayerMask.GetMask("Wall"));
        }

        return false;
    }

    public void WaypointCrossed(int waypointIdentifier, int lastWaypointIdentifier)
    {
        // waypoint already satisfied
        if (ReachedWaypointIds.Contains(waypointIdentifier)) return;

        var maxId = ReachedWaypointIds.Count > 0 ? ReachedWaypointIds.Max() : 0;

        // when the reached waypoint is the next e.g. 0 = maxId (no waypoints), waypointIdentifier = 1
        // 0 + 1 == 1 -> true
        // skipping a waypoint will not work
        if (maxId + 1 == waypointIdentifier)
        {
            ReachedWaypointIds.Add(waypointIdentifier);
            Brain.Score *= 2;
        }

        // all waypoints reached
        if (ReachedWaypointIds.Contains(lastWaypointIdentifier))
            ReachedWaypointIds.Clear();
    }
}