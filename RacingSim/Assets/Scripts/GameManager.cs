using System.Collections.Generic;
using Car;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject AgentPrefab;
    public float MutationRate = 0.01f;
    public int PopulationSize = 100;
    public int MaxLifespan = 1000;

    private int _lifetime;
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

    private void Update()
    {
        if (_lifetime < MaxLifespan)
            _lifetime++;
        else
        {
            CalculateFitness();
            DestroyAgents();
            _brains = Repopulate();
            SpawnAgents();
            _lifetime = 0;
        }
    }

    private void DestroyAgents()
    {
        for (var i = 0; i < _agents.Count; i++)
        {
            Destroy(_agents[i].gameObject); 
        }
    }

    private void SpawnAgents()
    {
        _agents.Clear();
        
        for (var i = 0; i < PopulationSize; i++)
        {
            _agents[i] = 
                Instantiate(AgentPrefab).GetComponent<Agent>();
            _agents[i].Brain = _brains[i];
        }
    }
    
    private void CalculateFitness()
    {
        var sum = 0d;

        foreach (var brain in _brains)
            sum += brain.Score;

        for (var i = 0; i < PopulationSize; i++)
            _brains[i].Fitness = _brains[i].Score / sum;
    }
    
    private List<Brain> Repopulate()
    {
        var fittestBrains = new List<Brain>(PopulationSize);
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

}