using System;
using System.Collections.Generic;
using System.Linq;
using Agent;
using UnityEngine;
using Random = UnityEngine.Random;

public class TrainManager : MonoBehaviour
{
    public static TrainManager Instance;

    public GameObject AgentPrefab;
    public bool FastRepopulate = false;
    public float MutationRate = 0.01f;
    public int PopulationSize = 100;
    public int MaxLifespan = 1000;
    public bool IncreaseLifespan;
    public int GenSaveInterval = 10;
    public float Speed;
    public float AvgSpeed;
    public NeuralNetAgent BestAgent { get; private set; }

    [HeaderAttribute("Current Gen Information")]
    [SerializeField] private int _generation;
    [SerializeField] private long _lifetime;
    [SerializeField] private double _topScore;
    [SerializeField] private double _lastTopFitness;


    private Brain[] _brains;
    private NeuralNetAgent[] _agents;
    private int _bestIndex;
    private long _speedSum;
    private long _frames;

    private void Awake()
    {
        if (!Instance)
            Instance = this;
    }

    private void Start()
    {
        _brains = new Brain[PopulationSize];
        _agents = new NeuralNetAgent[PopulationSize];

        for (var i = 0; i < PopulationSize; i++)
            _brains[i] = new Brain();

        SpawnAgents();
    }

    private void FixedUpdate()
    {
        _lifetime++;
        
        BestAgent = _agents[BestScoreIndex()];
        Speed = BestAgent.EditorProps.Speed;
        _speedSum += (long)Speed / 2;
        AvgSpeed = (float)_speedSum / _lifetime * 2;

        _topScore = _brains.Max(x => x.Score);

        if (_lifetime >= MaxLifespan || FastRepopulate)
        {
            FastRepopulate = false;
            CalculateFitness();
            _lastTopFitness = _brains.Max(x => x.Fitness);
            
            if ((_generation + 1) % GenSaveInterval == 0) {
                BestAgent.Brain.Save(_generation, MaxLifespan);
                print("Model Saved"); // TODO: remove debug msg
            }
            
            _generation++;
            DestroyAgents();
            _brains = Repopulate();
            SpawnAgents();
            MaxLifespan = IncreaseLifespan ? Convert.ToInt32(MaxLifespan * 1.1d) : MaxLifespan;
            _lifetime = 0;
            _speedSum = 0;
        }
    }

    private void DestroyAgents()
    {
        foreach (var agent in _agents)
        {
            Destroy(agent.Transform.gameObject);
        }
    }

    private void SpawnAgents()
    {
        _agents = new NeuralNetAgent[PopulationSize];

        for (var i = 0; i < PopulationSize; i++)
        {
            _agents[i] = Instantiate(AgentPrefab).GetComponent<AgentScript>().Agent as NeuralNetAgent;
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

    private int BestScoreIndex()
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