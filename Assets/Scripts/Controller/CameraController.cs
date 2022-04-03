using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    public Transform followTransform = null;

    [SerializeField]
    float m_pitch = 30.0f;

    [SerializeField]
    float m_distance = 2.0f;

    float pitchRad { get => m_pitch * Mathf.Deg2Rad; }

    // Update is called once per frame
    void Update()
    {
        if(followTransform != null)
        {
            transform.rotation = Quaternion.AngleAxis(m_pitch, followTransform.right) * Quaternion.LookRotation(followTransform.forward, Vector3.up);
            transform.position = followTransform.position + new Vector3(0.0f, Mathf.Sin(pitchRad), 0.0f) * m_distance - transform.forward * m_distance;
        }
    }
}
