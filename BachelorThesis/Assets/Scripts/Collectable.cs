using Agent;
using UnityEngine;

public class Collectable : MonoBehaviour
{
	private const float RotationSpeed = 50;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.Rotate(Vector3.up * (RotationSpeed * Time.deltaTime));
	}

	private void OnTriggerEnter(Collider other)
	{
		var agentScript = other.gameObject.GetComponent<AgentScript>();
		if (agentScript)
			Destroy(gameObject);
	}
}