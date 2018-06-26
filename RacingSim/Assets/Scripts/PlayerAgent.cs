using UnityEngine;

public class PlayerAgent : Agent
{
    private readonly string _horizontalInput;
    private readonly string _verticalInput;
    
    public PlayerAgent(Transform agentTransform, int playerId) : base(agentTransform)
    {
        _horizontalInput = $"HorizontalP{playerId}";
        _verticalInput = $"VerticalP{playerId}";
    }

    public override void Compute()
    {
        base.Compute();
        var action = new Action(Input.GetAxisRaw(_horizontalInput), Input.GetAxisRaw(_verticalInput));
        PerformAction(action);
    }
}