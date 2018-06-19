using UnityEngine;

public class BloopScript : MonoBehaviour
{
	
	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.CompareTag("Separator"))
			Destroy(transform.GetComponent<Rigidbody2D>());
	}

	private void OnBecameInvisible() => Destroy(transform.GetComponent<Rigidbody2D>());
}
