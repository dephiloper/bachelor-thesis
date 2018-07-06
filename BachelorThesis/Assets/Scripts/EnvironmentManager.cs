using System.Collections.Generic;
using System.Linq;
using Train;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public static EnvironmentManager Instance;
    public GameObject ObstaclePrefab;
    public GameObject CollectablePrefab;
    
    private GameObject[] _obstacles = new GameObject[20];
    private GameObject[] _collectables = new GameObject[20];
    private Mesh _mesh;

    private void Awake()
    {
        if (!Instance)
            Instance = this;
    }

    private void Start()
    {
        _mesh = GetComponent<MeshFilter>().mesh;
        if (!TrainManager.Instance) {
            InstantiateEnvironmentalesOnMesh(ObstaclePrefab, ref _obstacles, 1.5f);
            InstantiateEnvironmentalesOnMesh(CollectablePrefab, ref _collectables, 1.5f);
        }
    }

    /// <summary>
    /// https://forum.unity.com/threads/random-instantiate-on-surface-of-mesh.11153/#post-78718
    /// </summary>
    private void InstantiateEnvironmentalesOnMesh(GameObject prefab, ref GameObject[] environmentals, float minDistance)
    {
        for (var i = 0; i < environmentals.Length; i++)
        {
            var randTriPoint = GetRandomPosition();
            
            // when one of the previously placed obstacles is to close on the current on, retry
            if (_obstacles.Concat(_collectables).Any(x =>
                x != null && Vector3.Distance(x.transform.position, randTriPoint) < minDistance))
            {
                i--;
                continue;
            }
            
            if (environmentals[i] == null) {
                environmentals[i] = Instantiate(prefab, randTriPoint, Quaternion.identity);
                environmentals[i].transform.parent = transform;
            }
            else
                environmentals[i].transform.position = randTriPoint;

        }
    }

    private Vector3 GetRandomPosition()
    {
        // get random triangle
        var randomTriangle = Random.Range(0, _mesh.triangles.Length / 3) * 3;
            
        // get points representing random triangle and translate them into world points
        var triA = transform.TransformPoint(_mesh.vertices[_mesh.triangles[randomTriangle]]);
        var triB = transform.TransformPoint(_mesh.vertices[_mesh.triangles[randomTriangle+1]]);
        var triC = transform.TransformPoint(_mesh.vertices[_mesh.triangles[randomTriangle+2]]);
            
        // get random offsets to allow variance when getting a point
        var randA = Random.value;
        var randB = Random.value;
        var randC = Random.value;
            
        // calculate random point by normalising the result with the sum of all random values
        return (randA * triA + randB * triB + randC * triC) / (randA + randB + randC);    
    }
}