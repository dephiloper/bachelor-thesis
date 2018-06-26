using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TrainManager : MonoBehaviour
{
    public static TrainManager Instance;

    public GameObject AgentPrefab;
    public float MutationRate = 0.01f;
    public int PopulationSize = 100;
    public int MaxLifespan = 1000;
    public bool IncreaseLifespan;
    public int GenSaveInterval = 10;

    [HeaderAttribute("Current Gen Information")]
    [SerializeField] private int _generation;
    [SerializeField] private int _lifetime;
    [SerializeField] private double _topScore;
    [SerializeField] private double _lastTopFitness;


    private Brain[] _brains;
    private NeuralNetAgent[] _agents;
    private int _bestIndex;

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
        _bestIndex = BestScoreIndex();
        
        _lifetime++;
        var bestAgent = _agents[_bestIndex];

        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, 
            new Vector3(bestAgent.Transform.position.x, Camera.main.transform.position.y,
                bestAgent.Transform.position.z), 0.1f);

        _topScore = _brains.Max(x => x.Score);

        if (_lifetime >= MaxLifespan)
        {
            CalculateFitness();
            _lastTopFitness = _brains.Max(x => x.Fitness);
            
            if ((_generation + 1) % GenSaveInterval == 0) {
                _agents[_bestIndex].Brain.Save(_generation, MaxLifespan);
                print("Model Saved"); // TODO: remove debug msg
            }
            
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