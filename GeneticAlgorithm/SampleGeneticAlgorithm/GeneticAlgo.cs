﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticAlgorithm
{
    public class GeneticAlgo
    {
        private const float MutationRate = 0.01f;
        private const int TotalPopulation = 2000;
        private static readonly Random Random = new Random();
        private IDna<char>[] _population;
        private List<IDna<char>> _normalisationMap;
        private string _target;
        private const int MaxGeneration = 5000;


        public static void Main(string[] args)
        {
            new GeneticAlgo().Run();
        }
        
        public void Run()
        {
            Setup();
            foreach (var entity in _population)
            {
                entity.CalculateFitness(_target.ToCharArray());
            }

            for (var i = 0; i < MaxGeneration; i++)
            {
                Update();
                var max = _population.OrderByDescending(p => p.Fitness).FirstOrDefault();
                Console.WriteLine($"{max} - fitness: {max?.Fitness} - generation: {i}");
                if (max?.Fitness >= 1) break;
            }
        }

        private void Setup()
        {
            _target = "Genetic Algorithms are working just fine, I guess!";
            _population = new IDna<char>[TotalPopulation];

            for (var i = 0; i < _population.Length; i++)
            {
                _population[i] = new PhraseDna(_target.Length);
            }
        }

        private void Update()
        {
            _normalisationMap = new List<IDna<char>>();

            var max = _population.Max(x => x.Fitness);
            foreach (var entity in _population)
            {
                var percentage = Math.Floor((entity.Fitness * 100) / max);
                for (var i = 0; i < percentage; i++)
                {
                    _normalisationMap.Add(entity);
                }
            }

            for (var i = 0; i < TotalPopulation; i++)
            {
                var entityA = _normalisationMap[Random.Next(_normalisationMap.Count)];
                var entityB = _normalisationMap[Random.Next(_normalisationMap.Count)];

                var childEntity = entityA.Crossover(entityB);
                childEntity.Mutate(MutationRate);

                _population[i] = childEntity;
            }
            
            foreach (var entity in _population)
            {
                entity.CalculateFitness(_target.ToCharArray());
            }
        }
    }
}