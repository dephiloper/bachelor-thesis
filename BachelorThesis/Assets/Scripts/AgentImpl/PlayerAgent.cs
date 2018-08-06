using AgentData;
using AgentData.Actions;
using UnityEngine;

namespace AgentImpl
{
    public class PlayerAgent : Agent
    {
        [Header(nameof(PlayerAgent))]
        public bool IsDiscrete;
        public bool Record;
        [HideInInspector]
        public string VAxis;
        [HideInInspector]
        public string HAxis;
        [HideInInspector]
        public int VAxisIndex;
        [HideInInspector]
        public int HAxisIndex;


        private RecordManager _recordManager;

        private void Start()
        {
            if (Record)
                _recordManager = new RecordManager("decision.txt");
        }

        protected override void Compute()
        {
            if (!GameManager.Instance.StartRace) return;
            
            base.Compute();
            var hIput = IsDiscrete ? Input.GetAxisRaw(HAxis) : Input.GetAxis(HAxis);
            var vIput = IsDiscrete ? Input.GetAxisRaw(VAxis) : Input.GetAxis(VAxis);
            var action = new PlayerAction(hIput, vIput, IsDiscrete);
            PerformAction(action);
            _recordManager?.SaveDecision(Percept, action);
        }
        
        public override void SetValues(Agent agent)
        {
            base.SetValues(agent);
            var playerAgent = agent as PlayerAgent;
            if (playerAgent == null) return;
            
            IsDiscrete = playerAgent.IsDiscrete;
            Record = playerAgent.Record;
            IsDiscrete = playerAgent.IsDiscrete;
            VAxis = playerAgent.VAxis;
            HAxis = playerAgent.HAxis;
            VAxisIndex = playerAgent.VAxisIndex;
            HAxisIndex = playerAgent.HAxisIndex;
        }
    }
}