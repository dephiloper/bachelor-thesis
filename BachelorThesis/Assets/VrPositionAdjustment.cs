using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrPositionAdjustment : MonoBehaviour {
	
	public Transform VrCamera;
	private SteamVR_TrackedController _controller;
	private bool _moveLeft;
	private bool _moveRight;
	private bool _moveUp;
	private bool _moveDown;
	
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
		_moveLeft = false;
		_moveRight = false;
		_moveUp = false;
		_moveDown = false;
	}

	private void HandlePadClicked(object sender, ClickedEventArgs e)
	{		
		HandlePadUnclicked(sender, e);
		
		print(e.padX);
		
		if (e.padX > 0.6)
			_moveLeft = true;
		else if (e.padX < -0.6)
			_moveRight = true;
		if (e.padY > 0.6)
			_moveUp = true;
		if (e.padY < -0.6)
			_moveDown = true;
	}

	// Update is called once per frame
	void Update () {
		if (GameManager.Instance.StartRace) return;
		
		if (_controller.padPressed)
		{
			if (_moveLeft)
				VrCamera.position += new Vector3(0.005f, 0, 0);
			if (_moveRight)
				VrCamera.position -= new Vector3(0.005f, 0, 0);
			if (_moveUp)
				VrCamera.position += new Vector3(0, 0.005f, 0);
			if (_moveDown)
				VrCamera.position -= new Vector3(0, 0.005f, 0);
		}
	}
}
