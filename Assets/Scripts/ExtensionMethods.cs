using UnityEngine;

public static class ExtensionMethods
{
    public static Vector3 GetRandomPoint(this Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z));
    }

    public static Vector3 GetWorldScale(this Transform targetTransform)
    {
        Vector3 totalScale = targetTransform.localScale;
        if (targetTransform.parent == null)
        {
            return totalScale;
        }
        else
        {
            Vector3 parentWorldScale = GetWorldScale(targetTransform.parent);
            totalScale.Scale(parentWorldScale);
            return totalScale;
        }
    }


}

