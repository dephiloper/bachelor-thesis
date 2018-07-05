using Agent.Data;
using UnityEditor;
using UnityEngine;

namespace Agent.AgentImpl
{
    public class PlayerAgent : BaseAgent
    {
        private readonly RecordManager _recordManager;

        public PlayerAgent(AgentScript agentScript) : base(agentScript)
        {
            if (EditorProps.Record)
                _recordManager = new RecordManager("decision.txt");
        }

        public override void Compute()
        {
            base.Compute();
            var hIput = EditorProps.IsDiscrete ? Input.GetAxisRaw(EditorProps.HAxis) : Input.GetAxis(EditorProps.HAxis);
            var vIput = EditorProps.IsDiscrete ? Input.GetAxisRaw(EditorProps.VAxis) : Input.GetAxis(EditorProps.VAxis);
            var action = new Action(hIput, vIput, EditorProps.IsDiscrete);
            PerformAction(action);
            _recordManager?.SaveDecision(Percept, action);
        }
    }
}