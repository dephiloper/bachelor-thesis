using System.Collections.Generic;
using AgentData.Sensors;
using AgentImpl;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Agent), true)]
    public class AgentEditor : UnityEditor.Editor
    {
        private Agent _agent;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var agent = target as PlayerAgent;
            if (agent == null) return;

            var axis = ReadAxes();
            agent.HAxisIndex = EditorGUILayout.Popup("Horizonal Axis", agent.HAxisIndex, axis);
            agent.VAxisIndex = EditorGUILayout.Popup("Vertical Axis", agent.VAxisIndex, axis);
            agent.HAxis = axis[agent.HAxisIndex];           
            agent.VAxis = axis[agent.VAxisIndex];           
        }

        private void OnSceneGUI()
        {
            _agent = target as Agent;
            if (_agent == null || !(_agent.Sensor is FieldOfViewSensor)) return;

            Handles.color = Color.white;
            Handles.DrawWireArc(_agent.transform.position, Vector3.up, Vector3.forward, 360,
                ((FieldOfViewSensor) _agent.Sensor).ViewRadius);
            var viewAngleA = DirFromAngle(-((FieldOfViewSensor) _agent.Sensor).ViewAngle / 2, false);
            var viewAngleB = DirFromAngle(((FieldOfViewSensor) _agent.Sensor).ViewAngle / 2, false);

            Handles.DrawLine(_agent.transform.position,
                _agent.transform.position + viewAngleA * ((FieldOfViewSensor) _agent.Sensor).ViewAngle);
            Handles.DrawLine(_agent.transform.position,
                _agent.transform.position + viewAngleB * ((FieldOfViewSensor) _agent.Sensor).ViewAngle);

            if (_agent.VisibleAgents != null)
            {
                Handles.color = Color.magenta;
                foreach (var visibleAgent in _agent.VisibleAgents)
                    Handles.DrawLine(_agent.transform.position, visibleAgent);
            }
            
            if (_agent.VisibleCollectables != null)
            {
                Handles.color = Color.yellow;
                foreach (var collectable in _agent.VisibleCollectables)
                    Handles.DrawLine(_agent.transform.position, collectable);
            }

            if (_agent.VisibleObstacles != null)
            {
                Handles.color = Color.black;
                foreach (var obstacle in _agent.VisibleObstacles)
                    Handles.DrawLine(_agent.transform.position, obstacle);
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
            if (!angleIsGlobal) angleInDegrees += _agent.transform.eulerAngles.y;

            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
    }
}