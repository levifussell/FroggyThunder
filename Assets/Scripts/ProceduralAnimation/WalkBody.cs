using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathUtils;
using MathUtils.Editor;

namespace ProceduralAnimation
{
    public class WalkBody : MonoBehaviour
    {
        [SerializeField]
        private float m_bodyStepHeight = 0.5f;

        [SerializeField]
        private float m_bodyStepSpeed = 1.0f;

        BezierCurve3 m_stepCurve;
        float m_stepCurveTimer;

        private void Awake()
        {
            m_stepCurve = new BezierCurve3(transform.localPosition, transform.localPosition + Vector3.up * m_bodyStepHeight, transform.localPosition);
        }

        // Update is called once per frame
        void Update()
        {
            m_stepCurveTimer = Mathf.Clamp01(m_stepCurveTimer + m_bodyStepSpeed * Time.deltaTime);
            transform.localPosition = m_stepCurve.Evaluate(m_stepCurveTimer);
        }
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

#if UNITY_EDITOR
            if(m_stepCurve != null)
                E_BezierCurves.EditorSceneDrawBezierCurve3(m_stepCurve);
#endif
        }

        public void StartNewStep()
        {
            m_stepCurveTimer = 0.0f;
        }
    }
}
