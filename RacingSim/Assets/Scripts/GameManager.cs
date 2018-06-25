using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject AgentPrefab;
    public float MutationRate = 0.01f;
    public int PopulationSize = 100;
    public int MaxLifespan = 1000;
    public bool IncreaseLifespan = false;

    [SerializeField] private int _lifetime;

    [SerializeField] private double _topScore;
    [SerializeField] private double _topFitness;
    [SerializeField] private int _generation;


    private Brain[] _brains;
    private Agent[] _agents;

    private void Awake()
    {
        if (!Instance)
            Instance = this;
    }

    private void Start()
    {
        _brains = new Brain[PopulationSize];
        _agents = new Agent[PopulationSize];

        for (var i = 0; i < PopulationSize; i++)
            _brains[i] = new Brain();

        SpawnAgents();
    }

    private void FixedUpdate()
    {
        _lifetime++;
        var bestAgent = _agents[BestAgentIndex()];
        
        Camera.main.transform.position = new Vector3(bestAgent.transform.position.x,
            Camera.main.transform.position.y, bestAgent.transform.position.z);
        
        _topScore = _brains.Max(x => x.Score);

        if (_lifetime >= MaxLifespan || _agents.All(x => x.GetComponent<Rigidbody>() == null))
        {
            CalculateFitness();
            _topFitness = _brains.Max(x => x.Fitness);
            _generation++;
            DestroyAgents();
            _brains = Repopulate();
            SpawnAgents();
            MaxLifespan = IncreaseLifespan ? Convert.ToInt32(MaxLifespan * 1.1d) : MaxLifespan;
            _lifetime = 0;
        }
    }

    private void DestroyAgents()
    {
        foreach (var agent in _agents)
        {
            Destroy(agent.gameObject);
        }
    }

    private void SpawnAgents()
    {
        _agents = new Agent[PopulationSize];

        for (var i = 0; i < PopulationSize; i++)
        {
            _agents[i] = Instantiate(AgentPrefab).GetComponent<Agent>();
            _agents[i].Brain = _brains[i];
        }
    }

    private void CalculateFitness()
    {
        var sum = _brains.Sum(brain => brain.Score);

        for (var i = 0; i < PopulationSize; i++)
            _brains[i].Fitness = (double)_brains[i].Score / sum;
    }

    private Brain[] Repopulate()
    {
        var fittestBrains = new Brain[PopulationSize];
        for (var i = 0; i < PopulationSize; i++)
        {
            var leftBrain = SelectBrainOnProbabilty();
            var rightBrain = SelectBrainOnProbabilty();
            var child = leftBrain.Crossover(rightBrain);
            child.Mutate();

            fittestBrains[i] = child;
        }

        return fittestBrains;
    }

    private Brain SelectBrainOnProbabilty()
    {
        var random = Random.value;
        var sum = 0d;
        foreach (var brain in _brains)
        {
            sum += brain.Fitness;
            if (sum >= random)
                return brain;
        }

        return null;
    }

    private int BestAgentIndex()
    {
        var maxScore = -1d;
        var maxScoreIndex = 0;

        for (var i = 0; i < _agents.Length; i++)
        {
            if (_agents[i] != null && maxScore < _agents[i].Brain.Score)
            {
                maxScore = _agents[i].Brain.Score;
                maxScoreIndex = i;
            }
        }

        return maxScoreIndex;
    }
}