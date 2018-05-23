using UnityEngine;

public class BloopScript : MonoBehaviour
{
	public int BloopNumber = 0;
	
	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.CompareTag("Separator"))
		{
			transform.parent.GetComponent<GeneticAlgorithm>().Population[BloopNumber].Punishment /= 2;
		}
	}
}
