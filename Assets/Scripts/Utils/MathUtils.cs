using UnityEngine;

public class MathUtils
{
    public static float FixFloat(float _float)
    {
        string formattedFloat = _float.ToString("F2");
        return float.Parse(formattedFloat);
    }

    public static Vector3 FixVector3(Vector3 _vector3)
    {
        return new Vector3(
            MathUtils.FixFloat(_vector3.x),
            MathUtils.FixFloat(_vector3.y),
            MathUtils.FixFloat(_vector3.z)
        );
    }
}