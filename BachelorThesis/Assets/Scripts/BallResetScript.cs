using UnityEngine;

public class BallResetScript : MonoBehaviour
{
    public GameObject Ball;
    
    private SteamVR_Controller.Device TrackedDevice => SteamVR_Controller.Input((int) _trackedObj.index);
    private SteamVR_TrackedController _controller;
    private SteamVR_TrackedObject _trackedObj;

    private void Awake()
    {
        _trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    private void OnEnable()
    {
        _controller = GetComponent<SteamVR_TrackedController>();
        _controller.TriggerClicked += ControllerOnTriggerClicked;
        _controller.TriggerUnclicked += ControllerOnTriggerUnclicked;
    }

    private void OnDisable()
    {
        _controller.TriggerClicked -= ControllerOnTriggerClicked;
    }

    private void ControllerOnTriggerClicked(object sender, ClickedEventArgs e)
    {
        Ball.GetComponent<Rigidbody>().useGravity = false;
        Ball.GetComponent<Rigidbody>().isKinematic = true;
        Ball.transform.parent = transform;
        Ball.transform.position = transform.position + new Vector3(0, 0.1f, 0);
    }

    private void ControllerOnTriggerUnclicked(object sender, ClickedEventArgs e)
    {
        Ball.transform.parent = null;
        var ballRigidBody = Ball.GetComponent<Rigidbody>();
        ballRigidBody.useGravity = true;
        Ball.GetComponent<Rigidbody>().isKinematic = false;
        ballRigidBody.velocity = TrackedDevice.velocity;
        ballRigidBody.angularVelocity = TrackedDevice.angularVelocity;
    }
}