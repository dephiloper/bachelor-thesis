﻿using System;
using System.IO;
using System.Linq;
using AgentImpl;
using Environment;
using NNSharp.IO;
using NNSharp.Models;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Train
{
    public class TrainManager : MonoBehaviour
    {
        public static TrainManager Instance;

        public GameObject AgentPrefab;
        public Transform SpawnPoint;
        public Text Label;

        [Header("Hyperparams")] public int PopulationSize = 1000;
        public int SubPopulationSize = 100;
        public int GenerationCount = 10000;
        public int LifespanMillis = 10000;
        public int LimitLifespanMillis = 120000;
        public float MutationRate = 0.01f;
        public double CrossoverProbabilty = 0.6f;
        public double UniformCrossoverProbability = 0.5f;
        public bool EliteSelection;

        [Header("Gen Information")] public int Generation;
        public float TopScore;
        public int LifetimeMillis;

        [Header("Sub Gen Information")] public float SubTopScore;

        [Header("Others")] public UnityEngine.Object TrainModelAsset;
        public string ExportPath = "./Exports/";
        public string ImportPath = "./Exports/[FolderName]";
        public float TimeScale = 1;

        public NeuralNetAgent BestAgent { get; private set; }
        public SequentialModel DefaultModel => new ReaderKerasModel(_trainModelPath).GetSequentialExecutor();

        [HideInInspector] [SerializeField] private string _fileName;
        [HideInInspector] [SerializeField] private string _trainModelPath;

        private NeuralNetAgent[] _agents;
        private Brain[] _brains;
        private Logger _logger;
        private int _subGenerationCount;
        private int _subGeneration;
        private int _initialSubPopulationSize;

        private void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            _fileName = Path.GetFileName(AssetDatabase.GetAssetPath(TrainModelAsset));
#endif
        }

        private void Start()
        {
            Time.timeScale = TimeScale;
            _trainModelPath = $"{Application.dataPath}/StreamingAssets/{_fileName}";
            _logger = new Logger(new TrainLogger("log.txt"));
            EnvironmentManager.Instance.SpawnEnvironmentals();

            // setup brains
            _brains = new Brain[PopulationSize];

            // train from scratch
            if (string.IsNullOrEmpty(ImportPath))
                for (var i = 0; i < _brains.Length; i++)
                    _brains[i] = new Brain();

            // pretrain
            else if (Directory.Exists(ImportPath))
                _brains = SerializationUtils.LoadBrains(PopulationSize, ImportPath);

            // Import path not existing
            else
                throw new DirectoryNotFoundException();

            // calculate the number of sub generations (adds 1 if integer division is not possible w/o remaining)
            _subGenerationCount = PopulationSize / SubPopulationSize;
            _subGenerationCount += PopulationSize % SubPopulationSize > 0 ? 1 : 0;
            _initialSubPopulationSize = SubPopulationSize;
        }

        private void FixedUpdate()
        {
            // Spawn new agents when there are no left (should happen when a sub generation is finished)
            if (_agents == null)
            {
                SpawnAgents();
            }

            LifetimeMillis += (int) (Time.fixedDeltaTime * 1000);
            BestAgent = _agents.OrderByDescending(x => x.Brain.Score).FirstOrDefault();

            if (BestAgent != null)
                SubTopScore = BestAgent.Brain.Score;

            Label.text = $"Generation: {(float) Generation / 100:0.0}/{GenerationCount / 100}\n" +
                         $"Lifespan: {(float) LifespanMillis / 1000:0.00} s\n" +
                         $"Topscore: {TopScore:0.00}\n" +
                         $"Lifetime: {(float) LifetimeMillis / 1000:0.00} s\n" +
                         $"Sub-Topscore: {SubTopScore:0.00}";

            // Subgeneration is completed
            if (LifetimeMillis >= LifespanMillis || _agents.All(x => x.transform.GetComponent<Rigidbody>() == null))
            {
                if (TopScore < SubTopScore)
                    TopScore = SubTopScore;

                DestroyAgents();
                LifetimeMillis = 0;
                _logger.Log(LogType.Log,
                    $"{DateTime.Now:yyy-MM-dd-HH-mm-ss}" +
                    $"-gen_{(float) Generation / 1000:000.0}" +
                    $"-maxlifespan_{(float) LifespanMillis / 1000:0000}" +
                    $"-topscore_{TopScore:00000.0000}" +
                    $"-subtopscore_{SubTopScore:00000.0000}");

                _subGeneration++;
                Generation += (int)((float) SubPopulationSize / PopulationSize * 100);
            }

            // Generation is completed
            if (_subGeneration == _subGenerationCount)
            {
                if (Generation % 1000 == 0)
                    SerializationUtils.SaveBrains(_brains, Generation, LifespanMillis, ExportPath);
                if (Generation >= GenerationCount)
                    FinishTraining();

                _brains = Repopulate();
                _subGeneration = 0;
                SubPopulationSize = _initialSubPopulationSize;
                if (LifespanMillis < LimitLifespanMillis)
                    LifespanMillis += 2000;
                
                EnvironmentManager.Instance.SpawnEnvironmentals();
            }
        }

        private Brain[] Repopulate()
        {
            CalculateFitness();

            var newBrains = new Brain[PopulationSize];
            var i = 0;

            if (EliteSelection)
            {
                newBrains[0] = _brains.OrderByDescending(x => x.Score).FirstOrDefault();
                newBrains[0].Score = 0;
                i++;
            }

            for (; i < PopulationSize; i += 2)
            {
                var leftBrain = SelectBrainOnProbability();
                var rightBrain = SelectBrainOnProbability();
                var childs = leftBrain.UniformCrossover(rightBrain);
                foreach (var child in childs)
                    child.Mutate(MutationRate);

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

        private Brain SelectBrainOnProbability()
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

        private void SpawnAgents()
        {
            var remainder = PopulationSize % SubPopulationSize;

            if (_subGeneration == _subGenerationCount - 1)
                SubPopulationSize = remainder == 0 ? SubPopulationSize : remainder;

            _agents = new NeuralNetAgent[SubPopulationSize];

            for (var i = 0; i < SubPopulationSize; i++)
            {
                var agentGameObject = Instantiate(AgentPrefab, SpawnPoint);
                _agents[i] = agentGameObject.GetComponent<Agent>() as NeuralNetAgent;
                _agents[i].Brain = _brains[SubPopulationSize * _subGeneration + i];
            }
        }

        private void DestroyAgents()
        {
            foreach (var agent in _agents)
                Destroy(agent.transform.gameObject);

            _agents = null;
        }

        private static void FinishTraining()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }
    }
}