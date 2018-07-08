using System.Linq;
using Train;
using UnityEngine;

namespace Environment
{
    public class EnvironmentManager : MonoBehaviour
    {
        public static EnvironmentManager Instance;
        public GameObject ObstaclePrefab;
        public GameObject CollectablePrefab;
        public int ObstacleCount = 20;
        public float ObstacleMinDist = 1.5f;
        public int CollectableCount = 20;
        public float CollectableMinDist = 1.5f;

        private GameObject[] _obstacles;
        private GameObject[] _collectables;
        private Mesh _mesh;

        
        private void Awake()
        {
            if (Instance) return;
            Instance = this;
        }

        private void Start()
        {
            _mesh = GetComponent<MeshFilter>().mesh;
            _obstacles = new GameObject[ObstacleCount];
            _collectables = new GameObject[CollectableCount];
            if (TrainManager.Instance) return;
            SpawnEnvironmentals();
        }
        
        public void SpawnEnvironmentals()
        {
            InstantiateEnvironmentalesOnMesh(ObstaclePrefab, ref _obstacles, ObstacleMinDist);
            InstantiateEnvironmentalesOnMesh(CollectablePrefab, ref _collectables, CollectableMinDist);
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
}