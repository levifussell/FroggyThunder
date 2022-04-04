using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmCollider : MonoBehaviour
{
    public Action onCollide = null;

    public bool detectGrabbing = false;
    public bool isGrabbing = false;

    private Rigidbody m_currentGrabbedRigidbody = null;
    private ConfigurableJoint m_currentGrabJoint = null;

    private void OnCollisionEnter(Collision collision)
    {
        if(detectGrabbing && !isGrabbing && collision.gameObject.CompareTag("Grabbable"))
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            m_currentGrabbedRigidbody = rb;

            ConfigurableJoint grabJoint = gameObject.AddComponent<ConfigurableJoint>();
            grabJoint.SetPdParamters(100.0f, 2.0f, 100.0f, 2.0f, 20.0f);
            grabJoint.connectedBody = rb;
            grabJoint.connectedMassScale = 30.0f;

            m_currentGrabbedRigidbody = rb;
            m_currentGrabJoint = grabJoint;

            Grabbable grab = collision.gameObject.AddComponent<Grabbable>();
            grab.onDestroy += DropGrabbedObject;

            isGrabbing = true;
        }

        onCollide?.Invoke();
    }

    public void DropGrabbedObject()
    {
        if(m_currentGrabJoint != null)
        {
            Destroy(m_currentGrabJoint);
        }

        m_currentGrabbedRigidbody = null;
        m_currentGrabJoint = null;

        isGrabbing = false;
    }

    private void OnDestroy()
    {
        DropGrabbedObject();
    }
}
