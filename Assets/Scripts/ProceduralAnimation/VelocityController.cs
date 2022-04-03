using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityController : MonoBehaviour
{
    [SerializeField]
    public Transform targetTransform;
    bool wasTargetNull = true;

    private Rigidbody m_rigidBody;

    private void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody>();

        if (m_rigidBody == null)
            m_rigidBody = gameObject.AddComponent<Rigidbody>();

    }

    private void FixedUpdate()
    {
        if (m_rigidBody.isKinematic)
            return;

        if (wasTargetNull)
        {
            m_rigidBody.position = targetTransform.position;
            wasTargetNull = false;
        }
        else
        {
            Vector3 diff = targetTransform.position - m_rigidBody.position;
            if (diff.magnitude > 1.0f)
                m_rigidBody.position = targetTransform.position;
            else
                m_rigidBody.velocity = diff / Time.fixedDeltaTime;

            Quaternion rotDiff = targetTransform.rotation * Quaternion.Inverse(m_rigidBody.rotation);
            rotDiff.ToAngleAxis(out float angle, out Vector3 axis);
            m_rigidBody.angularVelocity = ((angle * Mathf.Deg2Rad) / Time.fixedDeltaTime) * axis.normalized;
        }
    }
}
