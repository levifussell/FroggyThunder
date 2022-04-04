using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
    private static Mesh _cylinderMesh = null;
    private static Mesh cyclinderMesh
    {
        get
        {
            if(_cylinderMesh == null)
            {
                _cylinderMesh = Resources.Load<Mesh>("Models/Character_Models/Creature_Cylinder 1");
            }

            return _cylinderMesh;
        }
    }

    public static GameObject CreatePrimitiveCylinder()
    {
        GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        MeshFilter mf = gameObject.GetComponent<MeshFilter>();
        mf.mesh = cyclinderMesh;
        CapsuleCollider cc = gameObject.GetComponent<CapsuleCollider>();
        cc.radius /= 100.0f;
        cc.height /= 100.0f;
        cc.direction = 2;
        return gameObject;
    }
}
