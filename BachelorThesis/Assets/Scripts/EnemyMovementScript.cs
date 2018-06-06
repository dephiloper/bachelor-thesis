using FiniteStateMachine;
using Interfaces;
using UnityEngine;
using UnityEngine.UI;
using StateMachine = FiniteStateMachine.StateMachine;

public class EnemyMovementScript : MonoBehaviour
{
    public Transform Target;
    public Text Label;

    public float SeekDistance = 2f;
    public float HitDistance = 2f;
    public float MoveBatSpeed = 1f;

    private const float FloatingSpeed = 12f;
    private const float FloatingInterval = 0.01f;

    private IIntelligence _enemyAI;
    private Rigidbody _rigidbody;
    private Vector3 _startPosition;
    private Vector3 _startRotationAngles;

    private readonly Vector3 _maxMoveSpace = new Vector3(1, 0.5f, 1);

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        var chill = new State();
        var seek = new State();

        chill.Actions.Add(ChillMove);
        chill.Actions.Add(FlipPaddle);

        chill.Transitions.AddRange(new[]
        {
            new Transition(seek, IsBallSeekable)
        });

        seek.Actions.Add(FollowBall);
        seek.Actions.Add(FlipPaddle);

        seek.Transitions.AddRange(new[]
        {
            new Transition(chill, IsBallSeekable, true),
        });

        _enemyAI = new StateMachine(chill);
        _startPosition = transform.position;
        _startRotationAngles = transform.rotation.eulerAngles;
    }

    private void Update()
    {
        var actions = _enemyAI.Update();
        Label.text = string.Empty;

        actions.ForEach(x =>
        {
            Label.text += x.Method.Name + " ";
            x();
        });
    }

    private bool IsBallSeekable()
    {
        var dist = CalulateDistance();
        return dist <= SeekDistance;
    }

    private void ChillMove()
    {
        var yPos = _startPosition.y + Mathf.Sin(Time.time * FloatingSpeed) * FloatingInterval;
        var step = MoveBatSpeed * Time.deltaTime;
        var newPosition = new Vector3(_startPosition.x, yPos, _startPosition.z);
        _rigidbody.MovePosition(Vector3.MoveTowards(transform.position, newPosition, step));
    }

    private void FlipPaddle()
    {
        // flip from forehand to backhand when on the left half of the table
        // otherwhise flip back!
        // for more performance do only once
        if (transform.position.x >= 0)
            _rigidbody.MoveRotation(Quaternion.Euler(_startRotationAngles.x, 180, _startRotationAngles.z));
        else         
            _rigidbody.MoveRotation(Quaternion.Euler(_startRotationAngles.x, 0, _startRotationAngles.z));
    }

    private void FollowBall()
    {
        // define boundaries, so that the bat can not move further
        var clampX = Mathf.Clamp(Target.position.x, _startPosition.x - _maxMoveSpace.x,
            _startPosition.x + _maxMoveSpace.x);
        var clampY = Mathf.Clamp(Target.position.y, _startPosition.y - _maxMoveSpace.y,
            _startPosition.y + _maxMoveSpace.y);

        // specify the new position of the bat, but follow only on the x and y axis
        var newPosition = new Vector3(clampX, clampY, transform.position.z);

        // move the bat depending on the specified speed
        var step = MoveBatSpeed * Time.deltaTime;
        _rigidbody.MovePosition(Vector3.MoveTowards(transform.position, newPosition, step));
    }

    private float CalulateDistance()
    {
        return Vector3.Distance(Target.position, transform.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, SeekDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, HitDistance);
    }
}