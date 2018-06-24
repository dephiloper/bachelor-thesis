using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public Brain Brain { get; set; }

    //TODO: remove, only for debugging purposes
    [SerializeField] private double _score;

    [SerializeField] private bool _isHuman = true;

    [SerializeField] private float _speed = 5f;

    [SerializeField] private float _turnSpeed = 2f;

    private float _onTrackSpeed;
    private float _offTrackSpeed;
    private Rigidbody _rigidbody;
    private DriverSensor _driverSensor;

    public List<int> ReachedWaypointIds;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _driverSensor = new DriverSensor(transform);
        ReachedWaypointIds = new List<int>();

        _onTrackSpeed = _speed;
        _offTrackSpeed = _speed / 4f;
    }

    private void FixedUpdate()
    {
        var action = new Action();

        if (_isHuman)
            action = new Action(Input.GetAxisRaw("P1Horizontal"), Input.GetAxisRaw("P1Vertical"));
        else if (Brain != null && _rigidbody)
        {
            var percept = _driverSensor.PerceiveEnvironment();
            action = Brain.Think(percept);
            _speed = _offTrackSpeed;

            if (IsOnTrack())
            {
                _speed = _onTrackSpeed;
                var vel = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.z);
                Brain.Score += percept.WallDistances.Sum() * vel.magnitude;
            }
            else
            {
                Destroy(_rigidbody);
            }

            _score = Brain.Score;
        }
        
        PerformActions(action);
    }


    private void PerformActions(Action action)
    {
        if (action.AccelerateForward)
            _rigidbody.AddForce(transform.forward * _speed);
        /*if (action.AccelerateBackward)
            _rigidbody.AddForce(-transform.forward * _speed);*/
        if (action.SteerLeft)
            _rigidbody.MoveRotation(Quaternion.Euler(transform.rotation.eulerAngles - transform.up * _turnSpeed));
        if (action.SteerRight)
            _rigidbody.MoveRotation(Quaternion.Euler(transform.rotation.eulerAngles + transform.up * _turnSpeed));
    }

    private bool IsOnTrack()
        => Physics.Raycast(transform.position, -transform.up, float.MaxValue, LayerMask.GetMask("Track"));

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