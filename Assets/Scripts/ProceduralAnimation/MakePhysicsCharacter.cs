using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralAnimation
{
    public static class MakePhysicsCharacter
    {
        public static void From(Transform baseObj)
        {
            Rigidbody[] bodies = baseObj.GetComponentsInChildren<Rigidbody>();

            foreach(Rigidbody body in bodies)
            {
                GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                MeshFilter mf = g.AddComponent<MeshFilter>();
                mf.mesh = g.GetComponent<MeshFilter>().mesh;

                Rigidbody rb = g.AddComponent<Rigidbody>();
                ConfigurableJoint joint = g.AddComponent<ConfigurableJoint>();
                joint.SetPdParamters(1000.0f, 30.0f, 1000.0f, 30.0f, 1000.0f);
            }
        }
    }
}
