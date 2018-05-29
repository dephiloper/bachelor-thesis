using UnityEngine;

public class BatSelectionScript : MonoBehaviour
{
	public GameObject Ball;
	private SteamVR_TrackedController _controller;
	
	private SteamVR_TrackedObject trackedObj;

	private SteamVR_Controller.Device Controller => SteamVR_Controller.Input((int)trackedObj.index);

	private void Awake()
	{
		trackedObj = GetComponent<SteamVR_TrackedObject>();
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
		Destroy(Ball.GetComponent<Rigidbody>());
		Ball.transform.parent = transform;
		Ball.transform.position = transform.position + new Vector3(0, 0.1f, 0);
	}

	private void ControllerOnTriggerUnclicked(object sender, ClickedEventArgs e)
	{
		Ball.transform.parent = null;
		var ballRigidBody = Ball.AddComponent<Rigidbody>();
		ballRigidBody.velocity = Controller.velocity;
		ballRigidBody.angularVelocity = Controller.angularVelocity;
	}

	private void Update()
	{
	}
}
