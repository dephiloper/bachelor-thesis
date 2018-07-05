using Agent.Data;
using UnityEngine;

namespace Agent.AgentImpl
{
    public class PlayerAgent : BaseAgent
    {
        public PlayerAgent(AgentScript agentScript) : base(agentScript) { }

        public override void Compute()
        {
            base.Compute();
            var action = new Action(Input.GetAxis(EditorProps.HAxis),Input.GetAxis(EditorProps.VAxis), EditorProps.IsDiscrete);
            PerformAction(action);
        }
    }
}