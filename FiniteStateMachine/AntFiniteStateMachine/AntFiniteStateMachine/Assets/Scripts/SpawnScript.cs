using UnityEngine;
using Random = System.Random;

public class SpawnScript : MonoBehaviour
{
	public GameObject[] AntPrefab;
	public GameObject LeafPrefab;
	public GameObject NestPrefab;
	public int AntPopulationCount;
	public int LeafCount;

	private readonly Random _random = new Random();
	
	void Start ()
	{
		for (var i = 0; i < AntPopulationCount; i++)
		{
			Instantiate(AntPrefab[_random.Next(0, AntPrefab.Length)]);
		}

		for (var i = 0; i < LeafCount; i++)
		{
			var leaf = Instantiate(LeafPrefab);
			leaf.transform.position = new Vector3((float)_random.NextDouble() - 0.5f, (float)_random.NextDouble() - 0.5f) * 10;
		}
		
		var nest = Instantiate(NestPrefab);
		nest.transform.position = new Vector3((float)_random.NextDouble() - 0.5f, (float)_random.NextDouble() - 0.5f) * 10;
	}
}
