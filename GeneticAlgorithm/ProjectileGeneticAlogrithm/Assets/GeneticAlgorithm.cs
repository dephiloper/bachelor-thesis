﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GeneticAlgorithm : MonoBehaviour
{
    public Transform Goal;
    public GameObject BloopPrefab;
    public int TotalPopulation;
    public int LifeSpan;
    public int CurrentFrame;
    public float MutationRate = 0.01f;
    public IDna<Vector2>[] Population;
    public bool ShowOnlyBestFitness = false;

    private Vector2 _target;
    private GameObject[] _bloops;
    private bool _restart;
    private int _generationCounter;

    void Start()
    {
        _target = Goal.GetPosition2D();
        _bloops = new GameObject[TotalPopulation];
        Population = new IDna<Vector2>[TotalPopulation];

        for (var i = 0; i < TotalPopulation; i++)
        {
            Population[i] = new VectorDna(LifeSpan);
            _bloops[i] = Instantiate(BloopPrefab, transform);
            _bloops[i].GetComponent<BloopScript>().BloopNumber = i;
        }
    }


    void Update()
    {
        for (var i = 0; i < TotalPopulation; i++)
        {            
            if (CurrentFrame < LifeSpan)
            {
                var bloopRigidbody2D = _bloops[i].transform.GetComponent<Rigidbody2D>();
                bloopRigidbody2D.AddForce(Population[i].Genes[CurrentFrame]);
                var dir = bloopRigidbody2D.velocity;
                var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90f;
                _bloops[i].transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                Population[i].CalculateFitness(_bloops[i].transform.GetPosition2D(), _target);
                if (Vector2.Distance(_bloops[i].transform.position, _target) < 0.5)
                    Population[i].Fitness *= 1.5f;
            }
            else
            {
                Destroy(_bloops[i]);
                _bloops[i] = Instantiate(BloopPrefab, transform);
                _restart = true;
            }
        }
        
        CurrentFrame++;
        
        if (_restart) {
            CurrentFrame = 0;
            _restart = false;
            Inheritance();
            Population.ToList().ForEach(x => x.Fitness = 0);
        }


        HighlightClosest();
    }

    private void Inheritance()
    {
        var normalisationMap = new List<IDna<Vector2>>();

        var max = Population.Max(x => x.Fitness);
        print(string.Format("max fitness: {0} on generation: {1}", max, _generationCounter));

        foreach (var entity in Population)
        {
            var percentage = Math.Floor(entity.Fitness * 100) / max;
            for (var i = 0; i < percentage; i++)
            {
                normalisationMap.Add(entity);
            }
        }

        for (var i = 0; i < TotalPopulation; i++)
        {
            var a = (int)(Random.value * normalisationMap.Count-1);
            var b = (int)(Random.value * normalisationMap.Count-1);

            var parterA = normalisationMap.ElementAt(a);
            var parterB = normalisationMap.ElementAt(b);
            var child = parterA.Crossover(parterB);
            child.Mutate(MutationRate);
            Population[i] = child;
        }

        _generationCounter++;
    }

    private void HighlightClosest()
    {
        var index = 0;
        var largestFitness = 0f;

        for (var i = 0; i < TotalPopulation; i++)
        {
            _bloops[i].GetComponent<SpriteRenderer>().color = Color.white;
            _bloops[i].GetComponent<SpriteRenderer>().enabled = !ShowOnlyBestFitness;

            if (Population[i].Fitness > largestFitness)
            {
                index = i;
                largestFitness = Population[i].Fitness;
            }
        }

        _bloops[index].GetComponent<SpriteRenderer>().color = Color.green;
        _bloops[index].GetComponent<SpriteRenderer>().enabled = true;

    }
}