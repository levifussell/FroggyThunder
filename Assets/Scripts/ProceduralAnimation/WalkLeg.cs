using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralAnimation
{
    public class WalkLeg : MonoBehaviour
    {
        [SerializeField]
        float m_legRadius = 0.1f;

        Transform m_hipTransform;
        Transform m_footTransform;

        Quaternion m_rotOffset;

        public static WalkLeg Build(Transform parent, Transform hipTransform, Transform footTransform)
        {
            GameObject walkLegObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Destroy(walkLegObj.GetComponent<Collider>());
            walkLegObj.transform.parent = parent;
            walkLegObj.name = "Leg";
            
            WalkLeg walkLeg = walkLegObj.AddComponent<WalkLeg>();
            walkLeg.transform.localScale = Vector3.one * walkLeg.m_legRadius;
            walkLeg.m_hipTransform = hipTransform;
            walkLeg.m_footTransform = footTransform;

            walkLeg.m_rotOffset = Quaternion.AngleAxis(90.0f, Vector3.right);

            return walkLeg;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 diff = m_footTransform.position - m_hipTransform.position;
            transform.position = m_hipTransform.position + diff / 2.0f;
            transform.localScale = new Vector3(m_legRadius, diff.magnitude * 0.5f, m_legRadius);
            transform.rotation = Quaternion.LookRotation(diff) * m_rotOffset;
        }
    }
}
