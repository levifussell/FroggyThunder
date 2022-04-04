using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechPopup : MonoBehaviour
{
    [SerializeField]
    float m_speedPopup = 1.0f;

    [SerializeField]
    bool m_debugEnable = false;

    RectTransform m_rectTransform;

    Vector3 m_enterPosition;
    Vector3 m_exitPosition;

    bool m_isPopupEnter = false;

    public Action onPopupEnter = null;

    private void Start()
    {
        m_rectTransform = GetComponent<RectTransform>();

        m_enterPosition = new Vector3(m_rectTransform.position.x, m_rectTransform.sizeDelta.y * m_rectTransform.localScale.y, m_rectTransform.position.z);
        m_exitPosition = new Vector3(m_rectTransform.position.x, -m_rectTransform.sizeDelta.y * m_rectTransform.localScale.y, m_rectTransform.position.z);

        m_rectTransform.position = m_exitPosition;
    }

    private void Update()
    {
        if(m_debugEnable)
        {
            if(m_isPopupEnter)
                EndPopup();
            else
                BeginPopup();

            m_debugEnable = false;
        }
    }

    public void BeginPopup()
    {
        m_isPopupEnter = true;
        StopAllCoroutines();
        StartCoroutine(MovePopup(m_enterPosition, true));
    }

    public void EndPopup()
    {
        m_isPopupEnter = false;
        StopAllCoroutines();
        StartCoroutine(MovePopup(m_exitPosition, false));
    }

    IEnumerator MovePopup(Vector3 target, bool isEnter)
    {
        Vector3 diff;
        do
        {
            diff = target - m_rectTransform.position;
            m_rectTransform.position += diff * Mathf.Min(1.0f, Time.fixedDeltaTime * m_speedPopup);
            yield return new WaitForFixedUpdate();

        } while (diff.magnitude > 1e-1);

        if(isEnter)
        {
            onPopupEnter?.Invoke();
        }
    }
}
