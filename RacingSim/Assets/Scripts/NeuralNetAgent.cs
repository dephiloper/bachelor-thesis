using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NeuralNetAgent : Agent
{
    public Brain Brain { get; set; }
    public List<int> ReachedWaypointIds;
    
    private const float ScoreReduction = 0.05f;
    
    private bool _exclude;
    private readonly bool _isTrained;

    public NeuralNetAgent(Transform transform, TextAsset brainAsset) : base(transform)
    {
        if (brainAsset)
        {
            Brain = Brain.Load(brainAsset.text);
            _isTrained = true;
        }
        else if (!TrainManager.Instance)
            throw new ArgumentNullException(nameof(ArgumentNullException),
                $"When there is no {nameof(brainAsset)} specified, the agent performs training. " +
                "Unfortunetly there is also no TrainManager set in this scene.");
        
        ReachedWaypointIds = new List<int>();
    }

    public override void Compute()
    {
        if (_exclude) return;
        
        base.Compute();
        var action = Brain.Think(Percept);
        if (!_isTrained)
            ApplyScore();
        
        PerformAction(action);
    }

    private void ApplyScore()
    {
        var vel = new Vector2(Rigidbody.velocity.x, Rigidbody.velocity.z).magnitude;
        if (OnTrack)
            Brain.Score += Convert.ToInt32(Percept.WallDistances.Sum() * ScoreReduction + vel);
        else
            _exclude = true;

        Score = Brain.Score;
    }

    public void WaypointCrossed(int waypointIdentifier, int lastWaypointIdentifier)
    {
        // waypoint already satisfied
        if (ReachedWaypointIds.Contains(waypointIdentifier) || !_isTrained        ) return;
        
        var maxId = ReachedWaypointIds.Count > 0 ? ReachedWaypointIds.Max() : 0;

        // when the reached waypoint is the next e.g. 0 = maxId (no waypoints), waypointIdentifier = 1
        // 0 + 1 == 1 -> true
        // skipping a waypoint will not work
        if (maxId + 1 == waypointIdentifier)
        {
            ReachedWaypointIds.Add(waypointIdentifier);
            Brain.Score = Convert.ToInt32(Brain.Score * 2f);
        }
        //TODO improvement: else would be moving in the wrong direction

        // all waypoints reached
        if (ReachedWaypointIds.Contains(lastWaypointIdentifier))
            ReachedWaypointIds.Clear();
    }
}