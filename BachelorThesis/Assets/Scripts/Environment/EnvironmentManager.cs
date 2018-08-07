using System.Collections.Generic;
using System.Linq;
using Train;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Environment
{
    public class EnvironmentManager : MonoBehaviour
    {
        public static EnvironmentManager Instance;
        public GameObject[] ObstaclesPrefab;
        public GameObject[] CollectablesPrefab;
        public GameObject SectionsPefab;
        public SectionBehavior[] Sections;
        public float EnvironmentalSpace = 1.5f;
        public SpawnType ObstacleSpawnType;
        public SpawnType CollectableSpawnType;
        public int TrackNumber = 0;
        

        private Mesh _mesh;
        private readonly List<Vector3> _occupiedPositions = new List<Vector3>();

        private void Awake()
        {
            if (Instance) return;
            Instance = this;
            Sections = SectionsPefab.GetComponentsInChildren<SectionBehavior>();
        }

        private void Start()
        {
            _mesh = GetComponent<MeshFilter>().mesh;
            if (TrainManager.Instance) return;
            SpawnEnvironmentals();
        }

        public void SpawnEnvironmentals()
        {
            ChangeSpawnType(ObstaclesPrefab, ObstacleSpawnType);
            ChangeSpawnType(CollectablesPrefab, CollectableSpawnType);
            _occupiedPositions.Clear();
        }

        private void ChangeSpawnType(GameObject[] groupsPrefab, SpawnType spawnType)
        {
            if (spawnType == SpawnType.Ignore) return;
            
            foreach (var groupPrefab in groupsPrefab)
                groupPrefab.SetActive(false);
            
            if (spawnType == SpawnType.None) return;

            var group = groupsPrefab[TrackNumber];

            if (spawnType == SpawnType.PseudoDynamic)
            {
                TrackNumber = Random.Range(0, 3);
                group = groupsPrefab[TrackNumber];
            }

            group.SetActive(true);
            
            // if active spawntype, enable also all child environmentals (disabled collectables)
            foreach (var child in group.transform)
            {
                var childTransform = (Transform) child;
                childTransform.gameObject.SetActive(true);
                
                // if static add to occupiedPositions
                if (spawnType == SpawnType.Static)
                    _occupiedPositions.Add(childTransform.position);
            }

            // if dynamic spawntype, reposition environmentals 
            if (spawnType == SpawnType.Dynamic)
                InstantiateEnvironmentalesOnMesh(group, EnvironmentalSpace);
        }

        /// <summary>
        /// https://forum.unity.com/threads/random-instantiate-on-surface-of-mesh.11153/#post-78718
        /// </summary>
        private void InstantiateEnvironmentalesOnMesh(GameObject groupPrefab, float minSpace)
        {
            foreach (var environmental in groupPrefab.transform)
            {
                Vector3 randPos;

                do // when one of the previously placed obstacles is to close on the current on, retry
                    randPos = GetRandomPosition();
                while (_occupiedPositions.Any(x => Vector3.Distance(x, randPos) < minSpace));
                
                // after getting random pos, add this to occupied positions
                _occupiedPositions.Add(randPos);

                var envTransform = (Transform) environmental;
                envTransform.position = randPos;
            }
        }

        private Vector3 GetRandomPosition()
        {
            // get random triangle
            var randomTriangle = Random.Range(0, _mesh.triangles.Length / 3) * 3;

            // get points representing random triangle and translate them into world points
            var triA = transform.TransformPoint(_mesh.vertices[_mesh.triangles[randomTriangle]]);
            var triB = transform.TransformPoint(_mesh.vertices[_mesh.triangles[randomTriangle + 1]]);
            var triC = transform.TransformPoint(_mesh.vertices[_mesh.triangles[randomTriangle + 2]]);

            // get random offsets to allow variance when getting a point
            var randA = Random.value;
            var randB = Random.value;
            var randC = Random.value;

            // calculate random point by normalising the result with the sum of all random values
            return (randA * triA + randB * triB + randC * triC) / (randA + randB + randC);
        }
    }

    public enum SpawnType
    {
        None,
        Static,
        PseudoDynamic,
        Dynamic,
        Ignore
    }
}