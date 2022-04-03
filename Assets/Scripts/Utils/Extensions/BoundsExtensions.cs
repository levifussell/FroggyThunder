using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoundsExtensions 
{
    public static Vector3[] ComputeAndGetCorners(this Bounds instance)
    {
        Vector3[] corners = new Vector3[]
        {
            new Vector3(instance.min.x, instance.min.y, instance.min.z),
            new Vector3(instance.min.x, instance.min.y, instance.max.z),
            new Vector3(instance.min.x, instance.max.y, instance.min.z),
            new Vector3(instance.max.x, instance.min.y, instance.min.z),
            new Vector3(instance.max.x, instance.max.y, instance.max.z),
            new Vector3(instance.max.x, instance.max.y, instance.min.z),
            new Vector3(instance.max.x, instance.min.y, instance.max.z),
            new Vector3(instance.min.x, instance.max.y, instance.max.z),
        };

        return corners;
    }

    public static Bounds GetFullObjectBounds(GameObject gameObject)
    {
        Collider[] cs = gameObject.GetComponentsInChildren<Collider>().Where((x) => !x.isTrigger).ToArray();
        if (cs.Length == 0)
        {
            Debug.LogError("Tried to get bounds of object without any colliders.");
            return new Bounds();
        }
        Bounds fullBounds = cs[0].bounds;
        foreach(Collider c in cs)
        {
            fullBounds.Encapsulate(c.bounds);
        }

        return fullBounds;
    }

    public static Bounds GetFullObjectBoundsLocal(GameObject gameObject)
    {
        Quaternion rot = gameObject.transform.rotation;
        gameObject.transform.rotation = Quaternion.identity;
        Collider[] cs = gameObject.GetComponentsInChildren<Collider>().Where((x) => !x.isTrigger).ToArray();
        if (cs.Length == 0)
        {
            Debug.LogError("Tried to get bounds of object without any colliders.");
            return new Bounds();
        }
        Bounds fullBounds = cs[0].bounds;
        foreach(Collider c in cs)
        {
            fullBounds.Encapsulate(c.bounds);
        }
        gameObject.transform.rotation = rot;

        return fullBounds;
    }

    public static Bounds GetFullObjectBoxBoundsLocal(GameObject gameObject)
    {
        BoxCollider[] cs = gameObject.GetComponentsInChildren<BoxCollider>().Where((x) => !x.isTrigger).ToArray();
        if (cs.Length == 0)
        {
            Debug.LogError("Tried to get bounds of object without any colliders.");
            return new Bounds();
        }
        Bounds fullBounds = new Bounds(gameObject.transform.position, cs[0].size.Mul(gameObject.transform.localScale));
        foreach(BoxCollider c in cs)
        {
            Bounds bound = new Bounds(gameObject.transform.position, c.size.Mul(gameObject.transform.localScale));
            fullBounds.Encapsulate(bound);
        }

        return fullBounds;
    }
}
