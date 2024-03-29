﻿using System;
using System.Collections.Generic;
using System.Linq;
using AgentData.Base;
using AgentData.Sensors;
using AgentImpl;
using Train;
using UnityEngine;

namespace AgentData.Percepts
{
    [Serializable]
    public class DistanceOnlyPercept : IPercept
    {
        [SerializeField] private List<double> _normalizedWallDistances;
        [SerializeField] private List<double> _normalizedObstacleDistances;
        [SerializeField] private List<double> _normalizedCollectableDistances;
        [SerializeField] private Vector3 _normalizedVelocity;

        private readonly Agent _agent;
        private readonly DistanceOnlySensor _sensor;
        private readonly List<double> _wallDistances;
        private readonly List<double> _obstacleDistances;
        private readonly List<double> _collectableDistances;

        public DistanceOnlyPercept(Agent agent, DistanceOnlySensor sensor, params List<double>[] distancesList)
        {
            _agent = agent;
            _sensor = sensor;
            _wallDistances = distancesList[0];
            _collectableDistances = distancesList[1];
            _obstacleDistances = distancesList[2];
        }

        /// <summary>
        /// Converts a normalized percept into a double array.
        /// </summary>
        /// <returns>Double representation of normalized percept.</returns>
        public double[] ToDoubleArray()
        {
            var arr = new List<double> {_normalizedVelocity.x, _normalizedVelocity.z};
            arr.AddRange(_normalizedWallDistances);
            arr.AddRange(_normalizedObstacleDistances);
            arr.AddRange(_normalizedCollectableDistances);
            arr.Add(_agent.OnTrack ? 0 : 1);
            arr.Add(_agent.RightDirection ? 0 : 1);
            return arr.ToArray();
        }

        public void Normalize()
        {
            if (_sensor == null)
                throw new NullReferenceException(
                    $"SensorType for {GetType().Name} should be {nameof(DistanceOnlySensor)}");

            var velocity = _agent.GetComponent<Rigidbody>().velocity;

            _normalizedVelocity = _agent.transform.InverseTransformDirection(velocity) / _agent.MaxSpeed * Agent.SpeedIncreaseFactor;

            _normalizedWallDistances = _wallDistances
                .Select(dist => dist / _sensor.Range)
                .ToList();

            _normalizedObstacleDistances = _obstacleDistances
                .Select(dist => dist / _sensor.Range)
                .ToList();

            _normalizedCollectableDistances = _collectableDistances
                .Select(dist => dist / _sensor.Range)
                .ToList();


            //Debug.Log(string.Join(",",_normalizedWallDistances.Select(i => $"{i:0.00}")));
            //Debug.Log(_agent.OnTrack);
            //Debug.Log(_agent.RightDirection);
            //Debug.Log($"vel: {_normalizedVelocity}");
        }
    }
}