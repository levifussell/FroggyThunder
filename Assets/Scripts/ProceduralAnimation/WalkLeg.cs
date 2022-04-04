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

        public static WalkLeg Build(Transform parent, Transform hipTransform, Transform footTransform, float legRadius, Material material)
        {
            //GameObject walkLegObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            GameObject walkLegObj = GameObjectExtensions.CreatePrimitiveCylinder();
            //Destroy(walkLegObj.GetComponent<Collider>());
            walkLegObj.transform.parent = parent;
            walkLegObj.name = "Leg";
            Rigidbody rb = walkLegObj.AddComponent<Rigidbody>();
            rb.isKinematic = true;

            if(material != null)
            {
                MeshRenderer mr = walkLegObj.GetComponent<MeshRenderer>();
                mr.sharedMaterial = material;
            }
            
            WalkLeg walkLeg = walkLegObj.AddComponent<WalkLeg>();
            walkLeg.m_legRadius = legRadius;
            walkLeg.transform.localScale = Vector3.one * walkLeg.m_legRadius;
            walkLeg.m_hipTransform = hipTransform;
            walkLeg.m_footTransform = footTransform;

            walkLeg.m_rotOffset = Quaternion.identity; // Quaternion.AngleAxis(90.0f, Vector3.right);

            walkLeg.SetLegPose();

            return walkLeg;
        }

        // Update is called once per frame
        void Update()
        {
            SetLegPose();
        }

        public void SetLegPose()
        {
            if(m_hipTransform == null || m_footTransform == null)
            {
                Destroy(this.gameObject);
                return;
            }

            Vector3 diff = m_footTransform.position - m_hipTransform.position;
            transform.position = m_hipTransform.position + diff / 2.0f;
            //transform.localScale = new Vector3(m_legRadius, diff.magnitude * 0.5f, m_legRadius) * 100.0f;
            transform.localScale = new Vector3(m_legRadius * 0.5f, m_legRadius * 0.5f, diff.magnitude * 0.5f) * 100.0f;
            transform.rotation = Quaternion.LookRotation(diff) * m_rotOffset;
        }
    }
}
