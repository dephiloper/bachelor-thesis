using System;
using UnityEditor;
using UnityEngine;

namespace CustomEditors
{
    [CustomEditor(typeof(AgentScript))]
    public class AgentEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var agentScript = target as AgentScript;
            if (agentScript == null) return;

            agentScript.Type = (AgentType) EditorGUILayout.EnumPopup("Agent Type", agentScript.Type);

            var editorProps = agentScript.EditorProperties;
            
            switch (agentScript.Type)
            {
                case AgentType.Player:
                    editorProps.PlayerId = EditorGUILayout.IntField("Player Id", editorProps.PlayerId);
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
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}