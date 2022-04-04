using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField]
    GameObject m_doorVisual = null;

    [SerializeField]
    int m_numSacrificesToOpen = 10;

    [SerializeField]
    Vector3 m_endPositionLocal = Vector3.zero;

    [SerializeField]
    float m_shakeScale = 1.0f;

    [SerializeField]
    float m_moveSpeed = 1.0f;

    [SerializeField]
    bool m_debugManualAdd = false;

    int m_numSacrifices = 0;

    Vector3 m_startPositionGlobal;
    Vector3 m_endPositionGlobal;

    AudioSource m_audioSource = null;


    private void Awake()
    {
        m_startPositionGlobal = transform.position;
        m_endPositionGlobal = transform.TransformPoint(m_endPositionLocal);

        m_audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(m_debugManualAdd)
        {
            AddNewSacrifice();
            m_debugManualAdd = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(m_endPositionGlobal, 0.5f * Vector3.one);
    }

    public void AddNewSacrifice()
    {
        m_numSacrifices = Mathf.Min(m_numSacrifices + 1, m_numSacrificesToOpen);
        Vector3 targetPos = m_startPositionGlobal + (m_endPositionGlobal - m_startPositionGlobal) * ((float)m_numSacrifices / (float)m_numSacrificesToOpen);
        StartCoroutine(MoveToTarget(targetPos));
    }

    IEnumerator MoveToTarget(Vector3 target)
    {
        m_audioSource.loop = true;
        m_audioSource.Play();

        Vector3 diff;
        do
        {
            diff = target - transform.position;
            transform.position += diff.normalized * Mathf.Min(diff.magnitude, Time.deltaTime * m_moveSpeed);
            m_doorVisual.transform.position = transform.position + Vector3Extensions.RandomSphere(m_shakeScale);
            yield return new WaitForEndOfFrame();

        } while (diff.sqrMagnitude > 1e-2f);

        m_audioSource.Stop();
        m_doorVisual.transform.position = transform.position;
    }
}
