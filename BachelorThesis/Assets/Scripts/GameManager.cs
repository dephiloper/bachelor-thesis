using System.Collections.Generic;
using System.Linq;
using AgentImpl;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    
    public static GameManager Instance;

    public Text StartCounterLabel;
    public GameObject StartLight;
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
        
        if (!_agents.FirstOrDefault(x => x.GetComponent<PlayerVrAgent>()))
            InvokeRepeating(nameof(UpdateStartCounter), 5, 1);
    }

    public void FinishSetup()
    {
        InvokeRepeating(nameof(UpdateStartCounter), 2, 1);
    }

    private void Update()
    {
        var orderedAgents = _agents.OrderByDescending(x => x.CurrentLap)
            .ThenByDescending(x => x.ReachedWaypointId)
            .ThenBy(x => x.DistToNextWaypoint)
            .ToList();

        for (var i = 0; i < orderedAgents.Count; i++)
            orderedAgents[i].Place = i + 1;

        var winner = _agents.FirstOrDefault(x => x.CurrentLap >= 3);
        
        if (winner != null)
        {
            print($"The winner is {winner.transform.name}");
            
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }
    }

    private void UpdateStartCounter()
    {
        StartLight.transform.GetChild(3-_startCounter).GetComponent<MeshRenderer>().material.color = Color.green;
        _startCounter--;
        StartCounterLabel.text = _startCounter.ToString();

        if (_startCounter == 0)
        {
            Destroy(StartLight);
            StartRace = true;
            Destroy(StartCounterLabel);
            CancelInvoke(nameof(UpdateStartCounter));
        }
    }
}
