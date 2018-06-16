using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.Ants.Length > 0)
        {
            var bestAntIndex = GameManager.Instance.BestAntIndex();
            var bestAntPosition = GameManager.Instance.Ants[bestAntIndex].transform.position;
            transform.position = Vector3.Lerp(transform.position,
                new Vector3(bestAntPosition.x, bestAntPosition.y, transform.position.z), 0.1f);
        }
    }
}