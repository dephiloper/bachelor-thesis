using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject AntPrefab;
    public Text Label;
    public int AntCount = 5;
    public int MaxLifeTime = 500;
    public int LifeTime;

    private GameObject[] _ants;
    private Brain[] _brains;
    private int _generation;

    private void Start()
    {
        _ants = new GameObject[AntCount];
        _brains = new Brain[AntCount];
        for (var i = 0; i < AntCount; i++)
            _brains[i] = new Brain();

        SpawnAnts();
    }

    private void FixedUpdate()
    {
        LifeTime++;

        if (_ants.All(x => x == null) || LifeTime > MaxLifeTime)
        {
            CalculateFitness();
            DisplayText();
            _brains = Repopulate();
            DestroyOldAnts();
            SpawnAnts();
            LifeTime = 0;
        }
    }

    private void DisplayText()
    {
        Label.text = $"Generation: {_generation}\n" +
                     $"Max Fitness: {_brains.ToList().Max(x => x.Fitness):F}\n" +
                     $"Max Score: {_brains.ToList().Max(x => x.Score)}";
        _generation++;
    }

    private void DestroyOldAnts()
    {
        _ants.ToList().ForEach(Destroy);
    }

    private void SpawnAnts()
    {
        for (var i = 0; i < AntCount; i++)
        {
            _ants[i] = Instantiate(AntPrefab);
            _ants[i].GetComponent<Ant>().Brain = _brains[i];
        }
    }

    private Brain[] Repopulate()
    {
        var fittestBrains = new Brain[AntCount];
        for (var i = 0; i < AntCount; i++)
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

    private void CalculateFitness()
    {
        var sum = 0d;

        foreach (var brain in _brains)
            sum += brain.Score;

        for (var i = 0; i < AntCount; i++)
            _brains[i].Fitness = _brains[i].Score / sum;
    }
}