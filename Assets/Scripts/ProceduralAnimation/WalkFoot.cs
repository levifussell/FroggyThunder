using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathUtils;
using MathUtils.Editor;

namespace ProceduralAnimation
{
    public class WalkFoot : MonoBehaviour
    {
        [SerializeField]
        float m_speedToTarget = 1.0f;

        [SerializeField]
        public float m_stepHeightMin = 1.0f;

        [SerializeField]
        public float m_stepHeightMax = 1.0f;

        public float speedToTarget { get => m_speedToTarget; set => m_speedToTarget = value; }

        Vector3 m_targetPrevious;
        Vector3 m_target;

        BezierCurve3 m_stepCurve;
        float m_bezierStepTime = 0.0f;

        GameObject m_footObjectKin;
        GameObject m_footObjectPhy;
        Rigidbody m_footObjectRb;

        public Vector3 target { get => m_target; }

        public GameObject footObjectPhy { get => m_footObjectPhy; }
        public Rigidbody footObjectRb { get => m_footObjectRb; }

        public bool isActive
        {
            get
            {
                return m_footObjectRb.velocity.sqrMagnitude > 1e-2f || m_bezierStepTime < 1.0f;
            }
        }

        public static WalkFoot Build(Transform parent, float m_footRadius, float m_footSpeed, float m_footStepHeightMin, float m_footStepHeightMax)
        {
            GameObject footObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Destroy(footObject.GetComponent<Collider>());
            footObject.transform.parent = parent;
            footObject.name = "Foot";

            Rigidbody footRb = footObject.AddComponent<Rigidbody>();
            footRb.isKinematic = true;

            footObject.transform.localScale = m_footRadius * Vector3.one;
            WalkFoot foot = footObject.AddComponent<WalkFoot>();
            foot.speedToTarget = m_footSpeed;
            foot.m_stepHeightMin = m_footStepHeightMin;
            foot.m_stepHeightMax = m_footStepHeightMax;

            GameObject footObjectPhy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            footObjectPhy.transform.localScale = m_footRadius * Vector3.one;
            //ConfigurableJoint joint = footObjectPhy.AddComponent<ConfigurableJoint>();
            //joint.connectedBody = footRb;
            //joint.SetPdParamters(1000.0f, 1.0f, 1000.0f, 1.0f, 100.0f);

            VelocityController vel = footObjectPhy.AddComponent<VelocityController>();
            vel.targetTransform = footRb.transform;

            foot.m_footObjectKin = footObject;
            foot.m_footObjectPhy = footObjectPhy;
            foot.m_footObjectRb = footObjectPhy.GetComponent<Rigidbody>();

            return foot;
        }

        private void Start()
        {
            ResetAt(transform.position);
        }

        private void Update()
        {
            m_bezierStepTime = Mathf.Clamp01(m_bezierStepTime + Time.deltaTime * m_speedToTarget);
            transform.position = m_stepCurve.Evaluate(m_bezierStepTime);
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            E_BezierCurves.EditorSceneDrawBezierCurve3(m_stepCurve);
        }

        #region methods

        public void SetNewTargetPosition(Vector3 pos)
        {
            m_targetPrevious = m_target;
            m_target = pos;
            Vector3 midPoint = (m_targetPrevious + (m_target - m_targetPrevious) / 2.0f) + Vector3.up * Random.Range(m_stepHeightMin, m_stepHeightMax);
            m_stepCurve = new BezierCurve3(m_target, midPoint, m_targetPrevious);
            m_bezierStepTime = 0.0f;
        }

        public void ResetAt(Vector3 pos)
        {
            m_targetPrevious = pos;
            m_target = pos;
            SetNewTargetPosition(pos);
            transform.position = pos;
        }

        #endregion
    }
}
