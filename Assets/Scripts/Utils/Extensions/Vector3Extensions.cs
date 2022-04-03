using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 Clamp(this Vector3 instance, float min, float max)
    {
        Vector3 clamped = new Vector3(
            Mathf.Clamp(instance.x, min, max),
            Mathf.Clamp(instance.y, min, max),
            Mathf.Clamp(instance.z, min, max)
            );
        return clamped;
    }

    public static Vector3Int Clamp(this Vector3Int instance, int min, int max)
    {
        Vector3Int clamped = new Vector3Int(
            Mathf.Clamp(instance.x, min, max),
            Mathf.Clamp(instance.y, min, max),
            Mathf.Clamp(instance.z, min, max)
            );
        return clamped;
    }

    public static Vector3 Mul(this Vector3 instance, Vector3 other)
    {
        return new Vector3(
            instance.x * other.x,
            instance.y * other.y,
            instance.z * other.z
            );
    }
    public static Vector3 Mul(this Vector3 instance, float s)
    {
        return new Vector3(
            instance.x * s,
            instance.y * s,
            instance.z * s
            );
    }
    public static Vector3 Div(this Vector3 instance, Vector3 other)
    {
        return new Vector3(
            instance.x / other.x,
            instance.y / other.y,
            instance.z / other.z
            );
    }

    public static Vector3 Sub(this Vector3 instance, float other)
    {
        return new Vector3(
            instance.x - other,
            instance.y - other,
            instance.z - other
            );
    }

    public static Vector3 Min(Vector3 a, Vector3 b)
    {
        return new Vector3(
            Mathf.Min(a.x, b.x),
            Mathf.Min(a.y, b.y),
            Mathf.Min(a.z, b.z)
            );
    }
    public static Vector3 Max(Vector3 a, Vector3 b)
    {
        return new Vector3(
            Mathf.Max(a.x, b.x),
            Mathf.Max(a.y, b.y),
            Mathf.Max(a.z, b.z)
            );
    }

    public static Vector3 RandomSphere(float radius = 1.0f)
    {
        Vector3 sphereVec = new Vector3(
            Random.Range(-1.0f, 1.0f),
            Random.Range(-1.0f, 1.0f),
            Random.Range(-1.0f, 1.0f));
        return sphereVec.normalized * radius;
    }
    public static Vector3 RandomUnitCube()
    {
        Vector3 cubeVec = new Vector3(
            Random.Range(-1.0f, 1.0f),
            Random.Range(-1.0f, 1.0f),
            Random.Range(-1.0f, 1.0f));
        return cubeVec;
    }
    public static Vector3 RandomCube(Vector3 halfRange)
    {
        Vector3 cubeVec = new Vector3(
            Random.Range(-halfRange.x, halfRange.x),
            Random.Range(-halfRange.y, halfRange.y),
            Random.Range(-halfRange.z, halfRange.z));
        return cubeVec;
    }

    public static float MinElement(this Vector3 a)
    {
        return Mathf.Min(a.x, a.y, a.z);
    }

    public static float MaxElement(this Vector3 a)
    {
        return Mathf.Max(a.x, a.y, a.z);
    }

    public static Vector3 SmoothStep(Vector3 from, Vector3 to, float t)
    {
        return new Vector3(
            Mathf.SmoothStep(from.x, to.x, t),
            Mathf.SmoothStep(from.y, to.y, t),
            Mathf.SmoothStep(from.z, to.z, t)
            );
    }
}
