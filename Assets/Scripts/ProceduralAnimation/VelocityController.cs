using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityController : MonoBehaviour
{
    [SerializeField]
    public Transform targetTransform;

    private Rigidbody m_rigidBody;

    private void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody>();

        if (m_rigidBody == null)
            m_rigidBody = gameObject.AddComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 diff = targetTransform.position - m_rigidBody.position;
        m_rigidBody.velocity = diff / Time.fixedDeltaTime;
    }
}
