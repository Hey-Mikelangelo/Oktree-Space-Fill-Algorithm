using UnityEngine;

public static class Utils
{
    public static Quaternion GetRandomRotation()
    {
        return Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
    }

    public static Vector3 GetRandomUniformScale(float min, float max)
    {
        float randomScaleValue = Random.Range(min, max);
        return new Vector3(randomScaleValue, randomScaleValue, randomScaleValue);
    }

    public static float GetBiggestVector3Component(Vector3 vector3)
    {
        if(vector3.x > vector3.y)
        {
            if(vector3.x > vector3.z)
            {
                return vector3.x;
            }
            else
            {
                return vector3.z;
            }
        }
        else
        {
            if(vector3.y > vector3.z)
            {
                return vector3.y;
            }
            else
            {
                return vector3.z;
            }
        }
    }

    public static Bounds GetRotatedMeshBounds(MeshFilter meshFilter)
    {
        Transform myTransform = meshFilter.transform;
        Mesh mesh = meshFilter.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        if (vertices.Length <= 0)
        {
            Debug.LogError("CalculateBoundingBox: mesh doesn't have vertices");
            return new Bounds(meshFilter.transform.position, Vector3.one);
        }
        Vector3 min, max;
        min = max = myTransform.TransformPoint(vertices[0]);
        for (int i = 1; i < vertices.Length; i++)
        {
            Vector3 V = myTransform.TransformPoint(vertices[i]);
            if (V.x > max.x)
                max.x = V.x;
            if (V.x < min.x)
                min.x = V.x;

            if (V.y > max.y)
                max.y = V.y;
            if (V.y < min.y)
                min.y = V.y;

            if (V.z > max.z)
                max.z = V.z;
            if (V.z < min.z)
                min.z = V.z;
        }
        Bounds bounds = new Bounds();
        bounds.SetMinMax(min, max);
        return bounds;
    }
}

