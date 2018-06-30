using System;
using System.Linq;
using Agent;
using UnityEngine;
using Random = UnityEngine.Random;

public class TrainManager : MonoBehaviour
{
    public static TrainManager Instance;

    public GameObject AgentPrefab;
    public Transform SpawnPoint;

    [Header("Hyperparams")] public int PopulationSize = 200;
    public int GenerationCount = 90;
    public int MaxLifespan = 500;
    public float MutationRate = 0.01f;
    public double CrossoverProbabilty = 0.6f;
    public double UniformCrossoverProbability = 0.5f;

    [Header("Gen Information")] public float Generation;
    public float LastTopScore;
    public float TopScore;
    public float Lifetime = 0;

    [Header("Sub Gen Information")] public float SubTopScore;
    public float TimeScale = 0;

    public NeuralNetAgent BestAgent { get; private set; }

    private int _subPopulationSize = 100;

    private NeuralNetAgent[] _agents;
    private Brain[] _brains;
    private int _subGenerationCount = 0;
    private int _subGeneration = 0;
    private int _initialSubPopulationSize;

    private void Awake()
    {
        if (!Instance)
            Instance = this;

        Time.timeScale = TimeScale;
    }

    private void Start()
    {
        // setup brains
        _brains = new Brain[PopulationSize];
        for (var i = 0; i < _brains.Length; i++)
            _brains[i] = new Brain();

        // calculate the number of sub generations (adds 1 if integer division is not possible w/o remaining)
        _subGenerationCount = PopulationSize / _subPopulationSize;
        _subGenerationCount += PopulationSize % _subPopulationSize > 0 ? 1 : 0;
        _initialSubPopulationSize = _subPopulationSize;
    }

    private void Update()
    {
        // Spawn new agents when there are no left (should happen when a sub generation is finished)
        if (_agents == null)
            SpawnAgents();

        SubTopScore = _brains.Max(x => x.Score);

        Lifetime += TimeScale;
        BestAgent = _agents.OrderByDescending(x => x.Brain.Score).FirstOrDefault();

        // Subgeneration is completed
        if (Lifetime >= MaxLifespan)
        {
            _subGeneration++;
            
            if (TopScore < SubTopScore)
                TopScore = SubTopScore;
            
            DestroyAgents();
            Lifetime = 0;
            Generation += (float) Math.Round((float) _subPopulationSize / PopulationSize, 2);
        }

        // Generation is completed
        if (_subGeneration == _subGenerationCount)
        {
            LastTopScore = _brains.Max(x => x.Score);
            _brains = Repopulate();
            _subGeneration = 0;
            Generation = (float) Math.Ceiling(Generation);
            _subPopulationSize = _initialSubPopulationSize;
            MaxLifespan += 50;
        }

        if (Generation >= GenerationCount)
        {
            var path = "./Assets/Brains/Test/";
            
            foreach (var brain in _brains)
                brain.Save((int) Generation, MaxLifespan, path);
            Application.Quit();
        }
    }

    private void SpawnAgents()
    {
        var remainder = PopulationSize % _subPopulationSize;

        if (_subGeneration == (_subGenerationCount - 1))
            _subPopulationSize = remainder == 0 ? _subPopulationSize : remainder;

        _agents = new NeuralNetAgent[_subPopulationSize];

        for (var i = 0; i < _subPopulationSize; i++)
        {
            var agentGameObject = Instantiate(AgentPrefab, SpawnPoint);
            _agents[i] = agentGameObject.GetComponent<AgentScript>().Agent as NeuralNetAgent;
            _agents[i].Brain = _brains[_subPopulationSize * _subGeneration + i];
        }
    }

    private void DestroyAgents()
    {
        foreach (var agent in _agents)
            Destroy(agent.Transform.gameObject);

        _agents = null;
    }

    private Brain[] Repopulate()
    {
        CalculateFitness();

        var newBrains = new Brain[PopulationSize];
        var i = 0;

        for (; i < PopulationSize; i += 2)
        {
            var leftBrain = SelectBrainOnProbabilty();
            var rightBrain = SelectBrainOnProbabilty();
            var childs = leftBrain.UniformCrossover(rightBrain);
            foreach (var child in childs)
                child.Mutate();

            newBrains[i] = childs[0];
            if (i + 1 < PopulationSize)
                newBrains[i + 1] = childs[1];
        }

        return newBrains;
    }

    private void CalculateFitness()
    {
        var sum = 0d;

        foreach (var brain in _brains)
        {
            if (brain.Score <= 0)
                brain.Score = 0.01F;

            sum += brain.Score;
        }

        for (var i = 0; i < PopulationSize; i++)
            _brains[i].Fitness = _brains[i].Score / sum;
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
}