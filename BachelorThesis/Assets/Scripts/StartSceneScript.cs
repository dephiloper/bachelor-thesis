using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;

public class StartSceneScript : MonoBehaviour
{
	public Button BtnStart;
	public Button BtnVr;
	public Toggle EnableCollectables;
	public Toggle EnableIntercollision;
	
	// Use this for initialization
	void Start () {
		BtnStart.onClick.AddListener(() =>
		{
			if (!EnableIntercollision.isOn)
				Physics.IgnoreLayerCollision(10, 10);
			
			SceneManager.LoadScene(EnableCollectables.isOn ? "PlayScene" : "PlaySceneNoEnvs");
		});
		
		BtnVr.onClick.AddListener(() =>
		{
			if (!EnableIntercollision.isOn)
				Physics.IgnoreLayerCollision(10, 10);
			
			StartCoroutine(LoadDevice("OpenVR"));
		});
	}

	private IEnumerator LoadDevice(string newDevice)
	{
		XRSettings.LoadDeviceByName(newDevice);
		yield return null;
		XRSettings.enabled = true;
		SceneManager.LoadScene(EnableCollectables.isOn ? "PlaySceneVr" : "PlaySceneVrNoEnvs");
	}
}