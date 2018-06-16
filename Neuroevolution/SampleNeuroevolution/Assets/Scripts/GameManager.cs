using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public GameObject AntPrefab;
    public Text MainLabel;
    public Text AvgLabel;
    public int AntCount = 5;
    public int MaxLifeTime = 500;
    public int LifeTime;

    private GameObject[] _ants;
    private Brain[] _brains;
    private int _generation;
    private double _sumScore;
    private int _avgScore;
    private Brain _bestBrain;

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
        _bestBrain = _brains.ToList().OrderByDescending(x => x.Score).FirstOrDefault();
        var maxScore = _brains.ToList().Max(x => x.Score);
        _sumScore += maxScore;

        var oldAvgScore = _avgScore;
        _avgScore = (int)_sumScore / (_generation + 1);
        
        AvgLabel.color = _avgScore > oldAvgScore ? Color.green : Color.red;
        
        MainLabel.text = $"Generation: {_generation}\n" +
                     $"Top Fitness: {_brains.ToList().Max(x => x.Fitness):F}\n" +
                     $"Top Score: {maxScore}\n";
        AvgLabel.text = $"Avg Score: {_avgScore} (avg of top)";

        
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

    private void OnApplicationQuit()
    {
        if (_bestBrain == null) return;

        using (var writer = new BinaryWriter(File.Open("weights.bin", FileMode.Create)))
        {
            var bytes = GetBytes(_bestBrain.Network.Flat.Weights);
            writer.Write(bytes.Length);
            writer.Write(bytes);
        }
    }

    static byte[] GetBytes(double[] values)
    {
        var result = new byte[values.Length * sizeof(double)];
        Buffer.BlockCopy(values, 0, result, 0, result.Length);
        return result;
    }
}