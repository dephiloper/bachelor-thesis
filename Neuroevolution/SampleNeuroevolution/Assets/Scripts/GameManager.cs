using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject AntPrefab;
    public Text MainLabel;
    public Text AvgLabel;
    public Text AntCountLabel;
    public int PopulationSize = 200;
    public int MaxLifeTime = 2000;
    public float MutationRate = 0.1f;
    public int LifeTime;
    public GameObject[] Ants;

    private Brain[] _brains;
    private int _generation;
    private double _sumScore;
    private int _avgScore;
    private Brain _bestBrain;
    private Color _startColor;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
            var populationSize = PlayerPrefs.GetInt("PopulationSize");
            var maxLifeTime = PlayerPrefs.GetInt("MaxLifeTime");
            var mutationRate = PlayerPrefs.GetFloat("MutationRate");
            PopulationSize = populationSize == 0 ? PopulationSize : populationSize;
            MaxLifeTime = maxLifeTime == 0 ? MaxLifeTime : maxLifeTime;
            MutationRate = Math.Abs(mutationRate) < 0.0000001 ? MutationRate : mutationRate;
        }
    }

    private void Start()
    {
        Ants = new GameObject[PopulationSize];
        _brains = new Brain[PopulationSize];
        for (var i = 0; i < PopulationSize; i++)
            _brains[i] = new Brain();

        SpawnAnts();
        var color = Ants[0].GetComponent<SpriteRenderer>().color;
        _startColor = new Color(color.r, color.g, color.b, 0.5f);
    }

    private void FixedUpdate()
    {
        LifeTime++;

        if (Ants.All(x => x.GetComponent<Rigidbody2D>() == null) || LifeTime > MaxLifeTime)
        {
            CalculateFitness();
            DisplayText();
            _brains = Repopulate();
            DestroyOldAnts();
            SpawnAnts();
            LifeTime = 0;
        }
        HighlightBestAnt();
        DisplayAntCount();
    }

    private void DisplayAntCount()
    {
        AntCountLabel.text = $"Ants Alive: {Ants.Count(x => x.GetComponent<Rigidbody2D>() != null)}\n"+
                             $"Current Frame: {LifeTime}/{MaxLifeTime}";
    }

    private void HighlightBestAnt()
    {
        foreach (var ant in Ants)
            ant.GetComponent<SpriteRenderer>().color = _startColor;
        
        var bestAnt = Ants[BestAntIndex()];
        if (bestAnt)
            bestAnt.GetComponent<SpriteRenderer>().color = Color.green;
    }

    private void DisplayText()
    {
        _bestBrain = _brains.ToList().OrderByDescending(x => x.Score).FirstOrDefault();
        var maxScore = _brains.ToList().Max(x => x.Score);
        _sumScore += maxScore;

        var oldAvgScore = _avgScore;
        _avgScore = (int) _sumScore / (_generation + 1);

        AvgLabel.color = _avgScore > oldAvgScore ? Color.green : Color.red;

        MainLabel.text = $"Generation: {_generation}\n" +
                         $"Top Score: {maxScore}\n";
        AvgLabel.text = $"Avg Score: {_avgScore} (avg of top)";


        _generation++;
    }

    private void DestroyOldAnts()
    {
        Ants.ToList().ForEach(Destroy);
    }

    private void SpawnAnts()
    {
        for (var i = 0; i < PopulationSize; i++)
        {
            Ants[i] = Instantiate(AntPrefab);
            Ants[i].GetComponent<Ant>().Brain = _brains[i];
        }
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

    private void CalculateFitness()
    {
        var sum = 0d;

        foreach (var brain in _brains)
            sum += brain.Score;

        for (var i = 0; i < PopulationSize; i++)
            _brains[i].Fitness = _brains[i].Score / sum;
    }

    public int BestAntIndex()
    {
        var maxScore = -1d;
        var maxScoreIndex = 0;

        for (var i = 0; i < Ants.Length; i++)
        {
            if (Ants[i] != null && maxScore < Ants[i].GetComponent<Ant>().Score)
            {
                maxScore = Ants[i].GetComponent<Ant>().Score;
                maxScoreIndex = i;
            }
        }

        return maxScoreIndex;
    }
    
    private void OnApplicationQuit()
    {
        /*if (_bestBrain == null) return;

        using (var writer = new BinaryWriter(File.Open("weights.bin", FileMode.Create)))
        {
            var bytes = GetBytes(_bestBrain.Network.Flat.Weights);
            writer.Write(bytes.Length);
            writer.Write(bytes);
        }*/
    }

    static byte[] GetBytes(double[] values)
    {
        var result = new byte[values.Length * sizeof(double)];
        Buffer.BlockCopy(values, 0, result, 0, result.Length);
        return result;
    }
}