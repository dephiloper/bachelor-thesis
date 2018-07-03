using UnityEngine;

public static class ExtensionMethods
{
    public static Vector2 ToVector2(this Vector3 v3) => new Vector2(v3.x, v3.z);
    public static Vector3 ToVector3(this Vector2 v2) => new Vector3(v2.x, 0, v2.y);
}