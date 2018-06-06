using FiniteStateMachine;
using FiniteStateMachine.States;
using Interfaces;
using UnityEngine;
using UnityEngine.UI;
using StateMachine = FiniteStateMachine.StateMachine;

public class EnemyManager : MonoBehaviour
{
    public Transform Target;
    public Text Label;

    public float SeekDistance = 2f;
    public float HitDistance = .5f;
    public float MoveBatSpeed = 2f;
    
    public Vector3 StartPosition { get; private set; }
    public Vector3 StartRotationAngles { get; private set; }
    public Vector3 MaxMoveSpace { get; private set; }

    private IIntelligence _enemyAi;
    
    #region unity event function
    
    private void Awake()
    {
        StartPosition = transform.position;
        StartRotationAngles = transform.rotation.eulerAngles;
        MaxMoveSpace = new Vector3(1, 0.5f, 1);
    }

    private void Start()
    {
        var idle = new IdleState(transform);
        var seek = new SeekState(transform);
       
        idle.Transitions.AddRange(new[]
        {
            new Transition(seek, seek.IsBallSeekable)
        });

        seek.Transitions.AddRange(new[]
        {
            new Transition(idle, seek.IsBallSeekable, true),
        });

        _enemyAi = new StateMachine(idle);
    }

    private void Update()
    {
        var actions = _enemyAi.Update();
        Label.text = string.Empty;

        actions.ForEach(x =>
        {
            Label.text += x.Method.Name + " ";
            x();
        });
    }

    #endregion
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, SeekDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, HitDistance);
    }
}