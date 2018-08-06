﻿using System.Collections.Generic;
using System.Linq;
using AgentImpl;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    
    public static GameManager Instance;

    public Text StartCounterLabel;
    public bool StartRace { private set; get; }
    
    private List<Agent> _agents;
    private int _startCounter = 3;    
    private void Awake()
    {
        if (!Instance)
            Instance = this;
    }

    private void Start()
    {
        _agents = GetComponentsInChildren<Agent>().ToList();
        InvokeRepeating(nameof(UpdateStartCounter), 5, 1);
    }

    private void Update()
    {
        var orderedAgents = _agents.OrderByDescending(x => x.CurrentLap)
            .ThenByDescending(x => x.ReachedWaypointId)
            .ThenBy(x => x.DistToNextWaypoint)
            .ToList();

        for (var i = 0; i < orderedAgents.Count; i++)
            orderedAgents[i].Place = i + 1;
    }

    private void UpdateStartCounter()
    {
        _startCounter--;
        StartCounterLabel.text = _startCounter.ToString();

        if (_startCounter == 0)
        {
            StartRace = true;
            Destroy(StartCounterLabel);
            CancelInvoke(nameof(UpdateStartCounter));
        }
    }
}
