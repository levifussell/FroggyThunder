using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public struct CameraSettings
    {
        public float pitch;
        public float distance;
    }

    [SerializeField]
    public Transform followTransform = null;

    [SerializeField]
    float m_pitch = 30.0f;

    [SerializeField]
    float m_distance = 2.0f;

    public bool rotationModeEnabled = false;
    public float rotationTimer = 0.0f;
    public float rotationSpeed = 10.0f;
        
    CameraSettings m_startSettings;

    float pitchRad { get => m_pitch * Mathf.Deg2Rad; }

    private void Awake()
    {
        m_startSettings.pitch = m_pitch;
        m_startSettings.distance = m_distance;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forwardDir = Vector3.forward;

        if(followTransform != null)
        {
            forwardDir = followTransform.forward;
        }

        if (rotationModeEnabled)
        {
            rotationTimer += Time.deltaTime;
            forwardDir = Quaternion.AngleAxis(rotationTimer * rotationSpeed, Vector3.up) * forwardDir;
        }

        transform.rotation = Quaternion.AngleAxis(m_pitch, followTransform.right) * Quaternion.LookRotation(forwardDir, Vector3.up);
        transform.position = followTransform.position + new Vector3(0.0f, Mathf.Sin(Mathf.Deg2Rad*pitchRad), 0.0f) * m_distance - transform.forward * m_distance;
    }

    public void SetSettings(CameraSettings settings, float timeSeconds)
    {
        StopAllCoroutines();
        StartCoroutine(TransitionSettings(settings, timeSeconds));
    }

    public void RevertToOriginalSettings()
    {
        SetSettings(m_startSettings, 0.3f);
    }

    IEnumerator TransitionSettings(CameraSettings newSettings, float timeSeconds)
    {
        float pitchRate = (newSettings.pitch - m_pitch) / timeSeconds;
        float distRate = (newSettings.distance - m_distance) / timeSeconds;

        while(Mathf.Abs(m_pitch - newSettings.pitch) > 1e-1f)
        {
            m_pitch += pitchRate * Time.fixedDeltaTime;
            m_distance += distRate * Time.fixedDeltaTime;

            //Debug.Log(Mathf.Abs(m_pitch - newSettings.pitch));

            yield return new WaitForFixedUpdate();
        }

        m_pitch = newSettings.pitch;
        m_distance = newSettings.distance;
    }
}
