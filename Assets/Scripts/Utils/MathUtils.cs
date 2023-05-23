using System.Collections.Generic;
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

    public static List<Vector2Int> GenerateRandomVectors(int numberOfVectors)
    {
        List<Vector2Int> generatedVectors = new List<Vector2Int>();

        for (int i = 0; i < numberOfVectors; i++)
        {
            Vector2Int newVector;

            do
            {
                newVector = new Vector2Int(Random.Range(0, GameConfig.Rows), Random.Range(0, GameConfig.Cols));
            } while (generatedVectors.Contains(newVector));

            generatedVectors.Add(newVector);
        }

        return generatedVectors;
    }

}