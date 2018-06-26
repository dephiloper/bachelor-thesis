using UnityEngine;

namespace Agent
{
    public class PlayerAgent : BaseAgent
    {
        public PlayerAgent(AgentScript agentScript) : base(agentScript)
        {
        }

        public override void Compute()
        {
            base.Compute();
            var action = new Action(
                    Input.GetAxisRaw(EditorProps.HAxis), 
                    Input.GetAxisRaw(EditorProps.VAxis)
                );
            PerformAction(action);
        }
    }
}