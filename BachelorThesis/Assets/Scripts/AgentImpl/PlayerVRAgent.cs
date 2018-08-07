using Extensions;
using UnityEngine;

namespace AgentImpl
{
    public class PlayerVrAgent : PlayerAgent
    {
        [Header(nameof(PlayerVrAgent))]
        public GameObject SteeringWheel;

        protected override void Compute()
        {
            base.Compute();
            SteeringWheel.transform.rotation = Quaternion.Euler(SteeringWheel.transform.eulerAngles.x, 
                SteeringWheel.transform.eulerAngles.y, HAxisValue.Map(-1f, 1f, 110f, -110f));
        }
    }
}