using System;
using System.Collections.Generic;
using Agent;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Editor
{
    [CustomEditor(typeof(AgentBehaviour))]
    public class AgentEditor : UnityEditor.Editor
    {
        private static bool _showBase = false;
        private static bool _showDerived = false;
        
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

                        editorProps.ViewRadius =
                            EditorGUILayout.Slider(nameof(editorProps.ViewRadius), editorProps.ViewRadius, 0, 10);
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
            if (!_showBase) return;
            
            editorProps.Label = (Text)
                EditorGUILayout.ObjectField(nameof(editorProps.Label), editorProps.Label, typeof(Text), true);
            editorProps.MaxSpeed = EditorGUILayout.FloatField(nameof(editorProps.MaxSpeed), editorProps.MaxSpeed);
            editorProps.MaxTurnSpeed = EditorGUILayout.FloatField(nameof(editorProps.MaxTurnSpeed), editorProps.MaxTurnSpeed);
            GUI.enabled = false;
            EditorGUILayout.FloatField(nameof(editorProps.Speed), editorProps.Speed);
            EditorGUILayout.FloatField(nameof(editorProps.TurnSpeed), editorProps.TurnSpeed);
            EditorGUILayout.IntField(nameof(editorProps.Score), editorProps.Score);
            GUI.enabled = true;
        }

        private void OnSceneGUI()
        {
            var agentScript = target as AgentBehaviour;
            if (agentScript == null) return;
            
            var editorProps = agentScript.EditorProperties;
            
            Handles.color = Color.white;
            Handles.DrawWireArc (agentScript.transform.position, Vector3.up, Vector3.forward, 360, editorProps.ViewRadius);
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
    }
}