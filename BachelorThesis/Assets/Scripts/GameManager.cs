using System.Collections.Generic;
using System.Linq;
using AgentImpl;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    
    public static GameManager Instance;

    public Text StartCounterLabel;
    public GameObject StartLight;
    public bool StartRace { private set; get; }
    public GameObject FinishPanel;
    
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
        Time.timeScale = 1f;        

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
            Time.timeScale = 0.3f;
            FinishPanel.GetComponentInChildren<Text>().text = $"{winner.name} wins!\nWait for restart...";
            FinishPanel.SetActive(true);
            Invoke(nameof(RestartGame), 3);
        }
    }

    private void RestartGame()
    {
        SceneManager.LoadScene("MenuScene"); 
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
