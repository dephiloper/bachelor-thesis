using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneScript : MonoBehaviour
{
	public Button BtnStart;
	public Button BtnVr;
	public Toggle EnableObstacles;
	
	// Use this for initialization
	void Start () {
		BtnStart.onClick.AddListener(() =>
		{
			SceneManager.LoadScene(EnableObstacles.isOn ? "PlaySceneObstacle" : "PlayScene");
		});
		
		BtnVr.onClick.AddListener(() =>
		{
			SceneManager.LoadScene(EnableObstacles.isOn ? "PlaySceneVrObstacle" : "PlaySceneVr");
		});
	}
	
}