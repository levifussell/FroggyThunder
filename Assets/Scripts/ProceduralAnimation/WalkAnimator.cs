using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralAnimation
{
    public class WalkAnimator : MonoBehaviour
    {
        [SerializeField]
        private WalkBody m_body = null;

        [SerializeField]
        private int m_numFeet = 2;

        [SerializeField]
        private float m_footRadius = 0.1f;

        [SerializeField]
        private float m_footSpeed = 5.0f;

        [SerializeField]
        private float m_footPlacementDistance = 0.1f;

        [SerializeField]
        private float m_minFootStrideDistance = 0.1f;

        [SerializeField]
        private float m_maxFootStrideDistance = 0.3f;

        [SerializeField]
        private float m_strideVelocityScale = 2.0f;

        [SerializeField]
        private float m_maxLegDistance = 1.0f;

        [SerializeField]
        private CharacterController m_characterController = null;

        private float m_maxProjectHeight = 10.0f;

        private WalkFoot[] m_feet;
        private WalkLeg[] m_leg;
        private Vector3[] m_feetOffset;

        int numActiveFeet
        {
            get
            {
                return m_feet.Where(x => x.isActive).Count();
            }
        }

        private void Awake()
        {
            /* Build Feet */

            m_feet = new WalkFoot[m_numFeet];
            m_feetOffset = new Vector3[m_numFeet];

            for (int i = 0; i < m_numFeet; ++i)
            {
                m_feet[i] = WalkFoot.Build(transform, m_footRadius, m_footSpeed);
            }

            for (int i = 0; i < m_numFeet; ++i)
            {
                float posAngle = ((2.0f * Mathf.PI) / m_numFeet) * i;
                m_feetOffset[i] = m_footPlacementDistance * new Vector3(Mathf.Cos(posAngle), 0.0f, Mathf.Sin(posAngle));
                m_feet[i].transform.position = this.transform.position + this.transform.rotation * m_feetOffset[i];
            }

            for (int i = 0; i < m_numFeet; ++i)
            {
                ProjectFootAtIndex(i, m_feet[i].transform.position, m_footRadius * 0.5f, m_maxProjectHeight);
                m_feet[i].ResetAt(m_feet[i].transform.position);
            }

            /* Build Legs */

            m_leg = new WalkLeg[m_numFeet];

            for(int i = 0; i < m_numFeet; ++i)
            {
                m_leg[i] = WalkLeg.Build(transform, m_body.transform, m_feet[i].footObjectPhy.transform);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (numActiveFeet == 0 && (CheckIfNoSupportFootHull(out float distFromSupport) || CheckIfAnyFootTooLong()))
            {
                int indexToProject = FindNextFootIndexToProject();

                distFromSupport = Mathf.Clamp(m_characterController.velocity.magnitude * m_strideVelocityScale, m_minFootStrideDistance, m_maxFootStrideDistance);

                Vector3 projectFromPos = this.transform.position +
                                            this.transform.rotation * m_feetOffset[indexToProject] +
                                            this.transform.forward * distFromSupport;
                ProjectFootAtIndex(indexToProject, projectFromPos, m_footRadius, m_maxProjectHeight);

                m_body.StartNewStep();
            }
        }

        #region custom methods

        private Vector2 GetLocalFootProjectionAtIndex(int footIndex)
        {
            Vector3 localFoot = (Quaternion.Inverse(transform.rotation) * (m_feet[footIndex].transform.position - transform.position));
            return new Vector2(localFoot.x, localFoot.z);
        }

        private void ProjectFootAtIndex(int footIndex, Vector3 projectFromPos, float footRadius, float maxProjectHeight)
        {
            if (Physics.SphereCast(new Ray(projectFromPos, Vector3.down), footRadius, out RaycastHit hit, maxProjectHeight, ~0, QueryTriggerInteraction.Ignore))
            {
                m_feet[footIndex].SetNewTargetPosition(hit.point);
            }
        }

        private int FindNextFootIndexToProject()
        {
            float furthestLocalFootProjection = float.MinValue;
            int furthestLocalFootProjectionIndex = -1;

            for (int i = 0; i < m_numFeet; ++i)
            {
                float localFootProjection = GetLocalFootProjectionAtIndex(i).sqrMagnitude;

                if (localFootProjection > furthestLocalFootProjection)
                {
                    furthestLocalFootProjection = localFootProjection;
                    furthestLocalFootProjectionIndex = i;
                }
            }

            return furthestLocalFootProjectionIndex;
        }

        private bool CheckIfAnyFootTooLong()
        {
            bool isTooLong = false;
            for(int i = 0; i < m_numFeet; ++i)
            {
                float localFootProjectionMag = GetLocalFootProjectionAtIndex(i).sqrMagnitude;
                if(localFootProjectionMag > m_maxLegDistance * m_maxLegDistance)
                {
                    isTooLong = true;
                    break;
                }
            }

            return isTooLong;
        }

        private bool CheckIfNoSupportFootHull(out float distFromSupport)
        {
            bool notSupported = true;
            distFromSupport = float.MaxValue;

            for (int i = 0; i < m_numFeet; ++i)
            {
                float localFootProjectionZ = GetLocalFootProjectionAtIndex(i).y;
                float localFootProjectionX = GetLocalFootProjectionAtIndex(i).x;

                if (localFootProjectionZ > 0.0f)
                {
                    notSupported = false;
                    break;
                }

                if(Mathf.Abs(localFootProjectionZ) < distFromSupport)
                {
                    distFromSupport = Mathf.Abs(localFootProjectionZ);
                }
            }

            return notSupported;
        }

        #endregion
    }
}
