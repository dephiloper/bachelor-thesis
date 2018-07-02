﻿using UnityEngine;

public static class ExtensionMethods
{
    public static Vector2 ToVector2(this Vector3 v3)
    {
        return new Vector2(v3.x, v3.z);
    }
}