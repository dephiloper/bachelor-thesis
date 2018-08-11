using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrRotationAdjustment : MonoBehaviour {

	public Transform VrCamera;
	private SteamVR_TrackedController _controller;
	private bool _moveForward;
	private bool _moveBackward;
	private bool _rotateLeft;
	private bool _rotateRight;
	
	// Use this for initialization
	void Start () {
		_controller = GetComponent<SteamVR_TrackedController>();
		_controller.PadClicked += HandlePadClicked;
		_controller.PadUnclicked += HandlePadUnclicked;
		_controller.TriggerClicked += TriggerClicked;
	}

	private void TriggerClicked(object sender, ClickedEventArgs e)
	{
		GameManager.Instance.FinishSetup();
	}

	private void HandlePadUnclicked(object sender, ClickedEventArgs e)
	{
		_moveForward = false;
		_moveBackward = false;
		_rotateLeft = false;
		_rotateRight = false;
	}

	private void HandlePadClicked(object sender, ClickedEventArgs e)
	{
		HandlePadUnclicked(sender, e);
		
		if (e.padX > 0.6)
			_rotateLeft = true;
		if (e.padX < -0.6)
			_rotateRight = true;
		if (e.padY > 0.6)
			_moveForward = true;
		if (e.padY < -0.6)
			_moveBackward = true;
	}

	// Update is called once per frame
	void Update () {
		if (GameManager.Instance.StartRace) return;

		if (_controller.padPressed) {
		
		if (_moveForward)
			VrCamera.position += new Vector3(0, 0, 0.005f);
		if (_moveBackward)
			VrCamera.position -= new Vector3(0, 0, 0.005f);
		if (_rotateLeft)
			VrCamera.Rotate(0,0.5f,0);
		if (_rotateRight)
			VrCamera.Rotate(0,-0.5f,0);
			
		}
	}
}
