using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using Agent;
using Agent.AgentImpl;
using Environment;
using NNSharp.IO;
using NNSharp.Models;
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

        [Header("Hyperparams")] 
        public int PopulationSize = 1000;
        public int GenerationCount = 1000;
        public int LifespanMillis = 10000;
        public int LimitLifespanMillis = 120000;
        public float MutationRate = 0.01f;
        public double CrossoverProbabilty = 0.6f;
        public double UniformCrossoverProbability = 0.5f;
        public bool EliteSelection;

        [Header("Gen Information")]
        public int Generation;
        public float TopScore;
        public int LifetimeMillis;

        [Header("Sub Gen Information")] 
        public float SubTopScore;

        [Header("Others")] 
        public string ExportPath = "./Assets/Exports/";
        public string ImportPath = "";
        public string ModelPath = "./Assets/plain_model.json";
        public bool ShowSensors;
        public float TimeScale = 1;

        public NeuralNetAgent BestAgent { get; private set; }

        public SequentialModel DefaultModel => new ReaderKerasModel(ModelPath).GetSequentialExecutor();

        private NeuralNetAgent[] _agents;
        private Brain[] _brains;
        private Logger _logger;
        private int _subPopulationSize = 50;
        private int _subGenerationCount;
        private int _subGeneration;
        private int _initialSubPopulationSize;
    
        private void Awake()
        {
            if (!Instance)
                Instance = this;
        }

        private void Start()
        {            
            Time.timeScale = TimeScale;

            _logger = new Logger(new TrainLogger("log.txt"));
        
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
            _subGenerationCount = PopulationSize / _subPopulationSize;
            _subGenerationCount += PopulationSize % _subPopulationSize > 0 ? 1 : 0;
            _initialSubPopulationSize = _subPopulationSize;
        }

        private void FixedUpdate()
        {
            // Spawn new agents when there are no left (should happen when a sub generation is finished)
            if (_agents == null)
            {
                SpawnAgents();
                EnvironmentManager.Instance.SpawnEnvironmentals();
            }

            LifetimeMillis += (int)(Time.fixedDeltaTime * 1000);
            BestAgent = _agents.OrderByDescending(x => x.Brain.Score).FirstOrDefault();
    
            if (BestAgent != null)
                SubTopScore = BestAgent.Brain.Score;
            
            Label.text = $"Generation: {(float)Generation/10:0.0}/{GenerationCount/10}\n" +
                         $"Lifespan: {(float)LifespanMillis/1000:0.00} s\n" +
                         $"Topscore: {TopScore:0.00}\n" +
                         $"Lifetime: {(float)LifetimeMillis/1000:0.00} s\n" +
                         $"Sub-Topscore: {SubTopScore:0.00}";

            // Subgeneration is completed
            if (LifetimeMillis >= LifespanMillis || _agents.All(x => x.Transform.GetComponent<Rigidbody>() == null))
            {
                if (TopScore < SubTopScore)
                    TopScore = SubTopScore;

                DestroyAgents();
                LifetimeMillis = 0;
                _logger.Log(LogType.Log,
                    $"{DateTime.Now:yyy-MM-dd-HH-mm-ss}" +
                    $"-gen_{(float)Generation/10:000.0}" +
                    $"-maxlifespan_{(float)LifespanMillis/1000:0000}" +
                    $"-topscore_{TopScore:00000.0000}" +
                    $"-subtopscore_{SubTopScore:00000.0000}");

                _subGeneration++;
                Generation += (int)((float)_subPopulationSize / PopulationSize * 10);
            }

            // Generation is completed
            if (_subGeneration == _subGenerationCount)
            {
                if (Generation % 100 == 0)
                    SerializationUtils.SaveBrains(_brains, Generation, LifespanMillis, ExportPath);
                if (Generation >= GenerationCount)
                    FinishTraining();
            
                _brains = Repopulate();
                _subGeneration = 0;
                _subPopulationSize = _initialSubPopulationSize;
                if (LifespanMillis < LimitLifespanMillis)
                    LifespanMillis += 2000;
            }
        }

        private Brain[] Repopulate()
        {
            CalculateFitness();

            var newBrains = new Brain[PopulationSize];
            var i = 0;

            if (EliteSelection) {
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
            var remainder = PopulationSize % _subPopulationSize;

            if (_subGeneration == _subGenerationCount - 1)
                _subPopulationSize = remainder == 0 ? _subPopulationSize : remainder;

            _agents = new NeuralNetAgent[_subPopulationSize];

            for (var i = 0; i < _subPopulationSize; i++)
            {
                var agentGameObject = Instantiate(AgentPrefab, SpawnPoint);
                _agents[i] = agentGameObject.GetComponent<AgentBehaviour>().Agent as NeuralNetAgent;
                _agents[i].Brain = _brains[_subPopulationSize * _subGeneration + i];
            }
        }

        private void DestroyAgents()
        {
            foreach (var agent in _agents)
                Destroy(agent.Transform.gameObject);

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