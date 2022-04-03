using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Transform m_followTransform = null;

    [SerializeField]
    float m_pitch = 30.0f;

    [SerializeField]
    float m_distance = 2.0f;

    float pitchRad { get => m_pitch * Mathf.Deg2Rad; }

    // Update is called once per frame
    void Update()
    {
        transform.position = m_followTransform.position + new Vector3(0.0f, Mathf.Sin(pitchRad), Mathf.Cos(pitchRad)) * m_distance;
        transform.rotation = Quaternion.LookRotation(m_followTransform.position - transform.position, Vector3.up);
    }
}
