using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject SeedPrefab;
    public int SeedCount = 100;
    
    public static GameManager Instance;
    public GameObject[] Seeds { get; private set; }
    

    private const int BorderThreshold = 10;

    private void Awake()
    {
        if (!Instance)
            Instance = this;
    }

    void Start()
    {
        InvokeRepeating(nameof(SpawnSeed), 0.0f, 5f);
        Seeds = new GameObject[SeedCount];
    }
    
    private void SpawnSeed()
    {
        for (var i = 0; i < Seeds.Length; i++)
        {
            if (!Seeds[i])
                Seeds[i] = Instantiate(SeedPrefab);

            var randomLocation = new Vector3(Random.value * (Screen.width - BorderThreshold),
                Random.value * (Screen.height - BorderThreshold), 0f);
            randomLocation = Camera.main.ScreenToWorldPoint(randomLocation);
            randomLocation.z = 0;
            Seeds[i].transform.position = randomLocation;
        }
    }
}