using UnityEngine;

namespace Agent
{
    public class PlayerAgent : BaseAgent
    {
        public PlayerAgent(AgentScript agentScript) : base(agentScript) { }

        public override void Compute()
        {
            base.Compute();
            var action = new Action(Input.GetAxis(EditorProps.HAxis),Input.GetAxis(EditorProps.VAxis));
            PerformAction(action);
        }
    }
}