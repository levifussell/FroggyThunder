using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmCollider : MonoBehaviour
{
    public Action onCollide = null;


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Grabbable"))
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();

            ConfigurableJoint grabJoint = gameObject.AddComponent<ConfigurableJoint>();
            grabJoint.SetAllJointMotions(ConfigurableJointMotion.Locked);
            grabJoint.connectedBody = rb;
            grabJoint.connectedMassScale = 100.0f;
        }

        onCollide?.Invoke();
    }
}
