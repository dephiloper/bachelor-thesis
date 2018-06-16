using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SubmitScript : MonoBehaviour
{
	public InputField PopulationSize;
	public InputField MutationRate;
	public InputField MaxLifeTime;
	public Button Submit;
	
	// Use this for initialization
	void Start ()
	{
		Submit.onClick.AddListener(OnSubmit);
	}

	private void OnSubmit()
	{
		int populationSize;
		int maxLifeTime;
		float mutationRate;

		int.TryParse(PopulationSize.text, out populationSize);
		int.TryParse(MaxLifeTime.text, out maxLifeTime);
		float.TryParse(MutationRate.text, out mutationRate);

		PlayerPrefs.SetInt("PopulationSize", populationSize);
		PlayerPrefs.SetInt("MaxLifeTime", maxLifeTime);
		PlayerPrefs.SetFloat("MutationRate", mutationRate);
		
		SceneManager.LoadScene("AnhancedScene");
	}
}
