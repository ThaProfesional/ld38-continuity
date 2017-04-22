using UnityEngine;

public static class Vector2Extension {
    // http://answers.unity3d.com/questions/661383/whats-the-most-efficient-way-to-rotate-a-vector2-o.html
    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;

        return new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);
    }
}
