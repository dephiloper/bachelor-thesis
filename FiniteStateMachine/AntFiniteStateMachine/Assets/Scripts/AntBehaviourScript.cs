using UnityEngine;
using Random = UnityEngine.Random;

public class AntBehaviourScript : MonoBehaviour
{
    public GameObject LeafPart;
    
    private StateMachine _stateMachine;
    private Rigidbody2D _rigidbody2D;

    void Start()
    {
        _rigidbody2D = transform.GetComponent<Rigidbody2D>();

        _stateMachine = PrepareStateMachine();
    }

    private StateMachine PrepareStateMachine()
    {
        var findLeaveState = new State();
        findLeaveState.Actions.Add(FindLeaf);
        findLeaveState.ExitActions.Add(StopMovement);
        var elopeState = new State();
        elopeState.Actions.Add(RunAway);
        var goHomeState = new State();
        goHomeState.Actions.Add(GoHome);
        goHomeState.EntryActions.Add(AttatchLeaf);
        goHomeState.ExitActions.Add(DetatchLeaf);
        
        var dangerTransition = new Transition
        {
            Condition = DangerCondition,
            TargetState = elopeState,
        };
        
        var dangerWithLeafTransition = new Transition
        {
            Condition = LeafDangerCondition,
            TargetState = elopeState,
        };

        var noDangerTransition = new Transition
        {
            Negate = true,
            Condition = DangerCondition,
            TargetState = findLeaveState,
        };

        var attatchLeafTransition = new Transition
        {
            Condition = NearLeafCondition,
            TargetState = goHomeState,
        };

        var carriedHomeTransition = new Transition
        {
            Condition = NearHomeCondition,
            TargetState = findLeaveState,
        };
        
        goHomeState.Transitions.AddRange(new [] {carriedHomeTransition, dangerWithLeafTransition});
        findLeaveState.Transitions.AddRange(new [] {dangerTransition, attatchLeafTransition});
        elopeState.Transitions.AddRange(new[] {noDangerTransition});
        
        return new StateMachine(findLeaveState);
    }

    void Update()
    {
        var states = _stateMachine.Update();
        if (states.Count > 0)
            states.ForEach(x => x());
    }

    #region ActionMethods

    private void FindLeaf()
    {
        _rigidbody2D.AddForce(new Vector2(Random.value - .5f, Random.value - .5f) * 20);
        if (_rigidbody2D.velocity.magnitude > 5)
        {
            _rigidbody2D.velocity *= 0.75f;
        }
        var dir = _rigidbody2D.velocity;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void RunAway()
    {
        _rigidbody2D.AddForce(new Vector2(Random.value - .5f, Random.value - .5f) * 200);
        var dir = _rigidbody2D.velocity;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void GoHome()
    {
        var nest = GameObject.FindGameObjectWithTag("Nest").transform;
        _rigidbody2D.AddForce((nest.position - transform.position) * 0.2f);
        if (_rigidbody2D.velocity.magnitude > 5)
            _rigidbody2D.velocity *= 0.75f;
        var dir = _rigidbody2D.velocity;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void AttatchLeaf()
    {
        Instantiate(LeafPart, transform);
    }

    private void DetatchLeaf()
    {
        Destroy(transform.GetChild(0).gameObject);
    }
    
    private void StopMovement() => _rigidbody2D.velocity *= 0.01f;
    
    #endregion

    #region ConditionMethods

    private bool DangerCondition()
    {
        if (transform.childCount > 0) return false;
        
        var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var distance = Vector2.Distance(worldPoint, transform.position);
        return distance < 1.5f;
    }

    private bool LeafDangerCondition()
    {
        if (transform.childCount == 0) return false;
        
        var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var distance = Vector2.Distance(worldPoint, transform.position);
        return distance < 1.5f;
    }
    
    private bool NearLeafCondition()
    {
        var leafScript = GameObject.FindWithTag("Leaf").GetComponent<LeafBehaviorScript>();
        
        if (leafScript == null) 
            return false;
        
        var distance = Vector2.Distance(leafScript.transform.position, transform.position);
        return distance < 1f;
    }

    private bool NearHomeCondition()
    {
        var nestTransform = GameObject.FindWithTag("Nest").transform;
        
        if (nestTransform == null) 
            return false;
        
        var distance = Vector2.Distance(nestTransform.position, transform.position);
        return distance < 1f;
    }
    
    #endregion
}