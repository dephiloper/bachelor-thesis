using System;
using System.Collections.Generic;
using Agent;
using Agent.AgentImpl;
using Environment;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Editor
{
    [CustomEditor(typeof(AgentBehaviour))]
    public class AgentEditor : UnityEditor.Editor
    {
        private static bool _showBase;
        private static bool _showDerived;
        private AgentBehaviour _agentScript;

        public override void OnInspectorGUI()
        {
            var agentScript = target as AgentBehaviour;
            if (agentScript == null) return;

            agentScript.AgentType =
                (AgentType) EditorGUILayout.EnumPopup(nameof(agentScript.AgentType), agentScript.AgentType);

            var editorProps = agentScript.EditorProperties;

            _showDerived = EditorGUILayout.Foldout(_showDerived, $"{agentScript.AgentType} Properties");

            if (_showDerived)
            {
                switch (agentScript.AgentType)
                {
                    case AgentType.Player:
                        var axes = ReadAxes();
                        editorProps.SelectedHAxis = EditorGUILayout.Popup(nameof(editorProps.SelectedHAxis),
                            editorProps.SelectedHAxis, axes);
                        editorProps.SelectedVAxis = EditorGUILayout.Popup(nameof(editorProps.SelectedVAxis),
                            editorProps.SelectedVAxis, axes);
                        editorProps.HAxis = axes[editorProps.SelectedHAxis];
                        editorProps.VAxis = axes[editorProps.SelectedVAxis];
                        editorProps.IsDiscrete =
                            EditorGUILayout.Toggle(nameof(editorProps.IsDiscrete), editorProps.IsDiscrete);
                        editorProps.Record =
                            editorProps.Record = EditorGUILayout.Toggle(nameof(editorProps.Record), editorProps.Record);
                        break;
                    case AgentType.PlayerVr:
                        break;
                    case AgentType.StateMachine:
                        break;
                    case AgentType.PathFinding:
                        editorProps.WaypointsPrefab = (GameObject)
                            EditorGUILayout.ObjectField(nameof(editorProps.WaypointsPrefab),
                                editorProps.WaypointsPrefab,
                                typeof(GameObject), true);
                        break;
                    case AgentType.NeuralNet:
                        editorProps.BrainAsset = (TextAsset)
                            EditorGUILayout.ObjectField(nameof(editorProps.BrainAsset), editorProps.BrainAsset,
                                typeof(TextAsset), true);

                        GUI.enabled = false;
                        EditorGUILayout.Toggle(nameof(editorProps.IsTrained), editorProps.IsTrained);
                        GUI.enabled = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            _showBase = EditorGUILayout.Foldout(_showBase, $"{nameof(BaseAgent)} Properties");
            if (_showBase)
            {
                GUI.enabled = false;
                EditorGUILayout.FloatField(nameof(editorProps.Speed), editorProps.Speed);
                EditorGUILayout.FloatField(nameof(editorProps.TurnSpeed), editorProps.TurnSpeed);
                EditorGUILayout.IntField(nameof(editorProps.Score), editorProps.Score);
                GUI.enabled = true;

                editorProps.MaxSpeed =
                    EditorGUILayout.FloatField(nameof(editorProps.MaxSpeed), editorProps.MaxSpeed);
                editorProps.MaxTurnSpeed =
                    EditorGUILayout.FloatField(nameof(editorProps.MaxTurnSpeed), editorProps.MaxTurnSpeed);

                EditorGUILayout.LabelField("Sensor Properties", EditorStyles.boldLabel);
                editorProps.ShowSensors =
                    EditorGUILayout.Toggle(nameof(editorProps.ShowSensors), editorProps.ShowSensors);
                editorProps.SensorDistance =
                    EditorGUILayout.Slider(nameof(editorProps.SensorDistance), editorProps.SensorDistance, 0, 20);

                EditorGUILayout.LabelField("Field of View Properties", EditorStyles.boldLabel);
                editorProps.ViewRadius =
                    EditorGUILayout.Slider(nameof(editorProps.ViewRadius), editorProps.ViewRadius, 0, 10);
                editorProps.ViewAngle =
                    EditorGUILayout.Slider(nameof(editorProps.ViewAngle), editorProps.ViewAngle, 0, 360);

                editorProps.Label = (Text)
                    EditorGUILayout.ObjectField(nameof(editorProps.Label), editorProps.Label, typeof(Text), true);
            }
        }

        private void OnSceneGUI()
        {
            _agentScript = target as AgentBehaviour;
            if (_agentScript == null) return;

            var editorProps = _agentScript.EditorProperties;

            Handles.color = Color.white;
            Handles.DrawWireArc(_agentScript.transform.position, Vector3.up, Vector3.forward, 360,
                editorProps.ViewRadius);
            var viewAngleA = DirFromAngle(-editorProps.ViewAngle / 2, false);
            var viewAngleB = DirFromAngle(editorProps.ViewAngle / 2, false);

            Handles.DrawLine(_agentScript.transform.position,
                _agentScript.transform.position + viewAngleA * editorProps.ViewRadius);
            Handles.DrawLine(_agentScript.transform.position,
                _agentScript.transform.position + viewAngleB * editorProps.ViewRadius);

            if (_agentScript.Agent?.VisibleAgents != null)
            {
                Handles.color = Color.magenta;
                foreach (var agent in _agentScript.Agent.VisibleAgents)
                    Handles.DrawLine(_agentScript.transform.position, agent);
            }
            
            if (_agentScript.Agent?.VisibleCollectables != null)
            {
                Handles.color = Color.yellow;
                foreach (var collectable in _agentScript.Agent.VisibleCollectables)
                    Handles.DrawLine(_agentScript.transform.position, collectable);
            }

            if (_agentScript.Agent?.VisibleObstacles != null)
            {
                Handles.color = Color.black;
                foreach (var obstacle in _agentScript.Agent.VisibleObstacles)
                    Handles.DrawLine(_agentScript.transform.position, obstacle);
            }
        }

        private static string[] ReadAxes()
        {
            var inputManager = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0];
            var obj = new SerializedObject(inputManager);
            var axisArray = obj.FindProperty("m_Axes");

            var names = new List<string>();

            for (var i = 0; i < axisArray.arraySize; ++i)
            {
                var axis = axisArray.GetArrayElementAtIndex(i);
                names.Add(axis.FindPropertyRelative("m_Name").stringValue);
            }

            return names.ToArray();
        }

        private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal) angleInDegrees += _agentScript.transform.eulerAngles.y;

            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
    }
}