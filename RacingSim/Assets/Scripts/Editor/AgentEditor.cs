using System;
using System.Collections.Generic;
using System.Linq;
using Agent;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(AgentScript))]
    public class AgentEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var agentScript = target as AgentScript;
            if (agentScript == null) return;

            agentScript.Type = (AgentType) EditorGUILayout.EnumPopup("Agent Type", agentScript.Type);

            var editorProps = agentScript.EditorProperties;

            EditorGUILayout.LabelField($"{agentScript.Type} Properties");
            switch (agentScript.Type)
            {
                case AgentType.Player:
                    var axes = ReadAxes();
                    editorProps.SelectedHAxis = EditorGUILayout.Popup("HorizontalAxis", editorProps.SelectedHAxis, axes);
                    editorProps.SelectedVAxis = EditorGUILayout.Popup("VerticalAxis", editorProps.SelectedVAxis, axes);
                    editorProps.HAxis = axes[editorProps.SelectedHAxis];
                    editorProps.VAxis = axes[editorProps.SelectedVAxis];
                    break;
                case AgentType.PlayerVr:
                    break;
                case AgentType.StateMachine:
                    break;
                case AgentType.PathFinding:
                    break;
                case AgentType.NeuralNet:
                    editorProps.BrainAsset = (TextAsset)
                        EditorGUILayout.ObjectField("Trained Network", editorProps.BrainAsset, typeof(TextAsset), true);

                    GUI.enabled = false;
                    EditorGUILayout.Toggle("Excluded", editorProps.IsExclude);
                    EditorGUILayout.Toggle("Is Trained", editorProps.IsTrained);
                    var maxWaypoint = editorProps.ReachedWaypointIds.Count == 0 ? 0 : editorProps.ReachedWaypointIds.Max();
                    EditorGUILayout.IntField("Reached Waypoint", maxWaypoint);
                    GUI.enabled = true;

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            EditorGUILayout.LabelField("Agent Properties");
            editorProps.MaxSpeed = EditorGUILayout.FloatField("Max Speed", editorProps.MaxSpeed);
            editorProps.TurnSpeed = EditorGUILayout.FloatField("Turn Speed", editorProps.TurnSpeed);
            GUI.enabled = false;
            EditorGUILayout.FloatField("Speed", editorProps.Speed);
            EditorGUILayout.IntField("Score", editorProps.Score);
            GUI.enabled = true;
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