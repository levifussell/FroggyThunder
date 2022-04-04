using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconAnimation : MonoBehaviour
{
    [SerializeField]
    float m_maxAngle = 10.0f;

    [SerializeField]
    float m_frequency = 4.0f;

    RectTransform m_rectTransform;


    private void Awake()
    {
        m_rectTransform = GetComponent<RectTransform>();        
    }

    private void Update()
    {
        float angle = Mathf.Sin(Time.time * m_frequency) * m_maxAngle;
        m_rectTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
