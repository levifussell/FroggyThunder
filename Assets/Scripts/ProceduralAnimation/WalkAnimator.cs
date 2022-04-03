using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralAnimation
{
    public class WalkAnimator : MonoBehaviour
    {
        [SerializeField]
        public WalkBody m_body = null;

        [SerializeField]
        private bool m_randomFootPlacement = false;

        [SerializeField]
        private int m_numFeet = 2;

        [SerializeField]
        private float m_footRadius = 0.1f;

        [SerializeField]
        private float m_footSpeed = 5.0f;

        [SerializeField]
        private float m_footStepHeightMin = 1.0f;

        [SerializeField]
        private float m_footStepHeightMax = 1.0f;

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

        private float m_maxProjectHeight
        {
            get { return m_footStepHeightMax * 4.0f; }
        }

        private int m_maxNumActiveFeet;

        private WalkFoot[] m_feet;
        private WalkLeg[] m_leg;
        private Vector3[] m_feetOffset;

        public WalkFoot[] feet { get => m_feet; set => m_feet = value; }

        private int m_footMask;

        int numActiveFeet
        {
            get
            {
                return m_feet.Where(x => x.isActive).Count();
            }
        }

        private void Awake()
        {
            m_maxNumActiveFeet = Mathf.Max(0, m_numFeet - 2);

            m_footMask = ~LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer));

            /* Build Feet */

            m_feet = new WalkFoot[m_numFeet];
            m_feetOffset = new Vector3[m_numFeet];

            for (int i = 0; i < m_numFeet; ++i)
            {
                m_feet[i] = WalkFoot.Build(transform, m_footRadius, m_footSpeed, m_footStepHeightMin, m_footStepHeightMax);
                m_feet[i].gameObject.layer = gameObject.layer;
                m_feet[i].footObjectPhy.layer = gameObject.layer;
            }

            for (int i = 0; i < m_numFeet; ++i)
            {
                float posAngle = ((2.0f * Mathf.PI) / m_numFeet) * i;
                m_feetOffset[i] = m_footPlacementDistance * new Vector3(Mathf.Cos(posAngle), 0.0f, Mathf.Sin(posAngle));
                m_feet[i].transform.position = this.transform.position + this.transform.rotation * m_feetOffset[i];
            }

            for (int i = 0; i < m_numFeet; ++i)
            {
                ProjectFootAtIndex(i, m_feet[i].transform.position, m_footRadius * 0.5f, 100.0f);
                m_feet[i].ResetAt(m_feet[i].target);
            }

            /* Build Legs */

            m_leg = new WalkLeg[m_numFeet];

            for(int i = 0; i < m_numFeet; ++i)
            {
                m_leg[i] = WalkLeg.Build(transform, m_body.transform, m_feet[i].footObjectPhy.transform, m_footRadius * 0.8f);
                m_leg[i].gameObject.layer = gameObject.layer;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (numActiveFeet <= m_maxNumActiveFeet)
            {
                HashSet<int> indicesToProject = GetIndicesOfFeetTooLong();

                if(CheckIfNoSupportFootHull(out float distFromSupport))
                {
                    indicesToProject.Add(FindNextFootIndexToProject());
                }

                foreach(int idx in indicesToProject)
                {
                    if(m_randomFootPlacement)
                        distFromSupport = Random.Range(m_minFootStrideDistance, m_maxFootStrideDistance);
                    else
                        distFromSupport = Mathf.Clamp(m_characterController.velocity.magnitude * m_strideVelocityScale, m_minFootStrideDistance, m_maxFootStrideDistance);

                    Vector3 projectFromPos = this.transform.position +
                                                this.transform.rotation * m_feetOffset[idx] +
                                                this.transform.forward * distFromSupport +
                                                Vector3.up * m_footStepHeightMax * 1.5f;
                    ProjectFootAtIndex(idx, projectFromPos, m_footRadius, m_maxProjectHeight);

                    if (numActiveFeet > m_maxNumActiveFeet)
                    {
                        break;
                    }

                }

                if(indicesToProject.Count > 0)
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
            if (Physics.SphereCast(new Ray(projectFromPos, Vector3.down), footRadius, out RaycastHit hit, maxProjectHeight, m_footMask, QueryTriggerInteraction.Ignore))
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

                if (!m_feet[i].isActive && localFootProjection > furthestLocalFootProjection)
                {
                    furthestLocalFootProjection = localFootProjection;
                    furthestLocalFootProjectionIndex = i;
                }
            }

            return furthestLocalFootProjectionIndex;
        }

        private HashSet<int> GetIndicesOfFeetTooLong()
        {
            HashSet<int> indices = new HashSet<int>();

            for(int i = 0; i < m_numFeet; ++i)
            {
                float localFootProjectionZ = GetLocalFootProjectionAtIndex(i).y;

                float localFootProjectionMag = GetLocalFootProjectionAtIndex(i).sqrMagnitude;

                if(localFootProjectionZ < 0.0f && localFootProjectionMag > m_maxLegDistance * m_maxLegDistance)
                {
                    indices.Add(i);
                }
            }

            return indices;
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
