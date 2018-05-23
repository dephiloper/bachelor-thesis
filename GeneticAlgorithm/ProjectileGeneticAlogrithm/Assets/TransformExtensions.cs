using UnityEngine;

public static class TransformExtensions
{
    public static Vector2 GetPosition2D(this Transform transform)
    {
        return new Vector2(transform.position.x, transform.position.y);
    }
}