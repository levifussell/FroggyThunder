using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    [SerializeField]
    int m_numRayCasts = 16;

    [SerializeField]
    float m_viewRange = 30.0f;

    [SerializeField]
    float m_accelMag = 5.0f;

    [SerializeField]
    float m_velDecayRate = 0.9f;

    [SerializeField]
    Transform m_phyRoot = null;

    [SerializeField]
    float m_phyRootThreshold = 0.1f;

    [SerializeField]
    float m_phyRootStrength = 10.0f;

    [SerializeField]
    float m_backwardsBias = 0.2f;

    [SerializeField]
    float m_monsterPullFrequency = 1.0f;

    Vector3 m_controlVelocity;
    Vector3 m_lastWeightedDir;

    Vector3[] m_rayDirections;
    float[] m_rayHitDistances;

    int m_visionMask;

    float m_monsterPullTimer = 0.0f;

    private void Awake()
    {
        /* Create Vision Sensors */

        m_rayDirections = new Vector3[m_numRayCasts];
        m_rayHitDistances = new float[m_numRayCasts];

        for(int i = 0; i < m_rayDirections.Length; ++i)
        {
            float angle = (Mathf.PI) * ((float)i / m_rayDirections.Length);
            m_rayDirections[i] = new Vector3(Mathf.Cos(angle), 0.0f, Mathf.Sin(angle));
        }

        m_visionMask = ~LayerMask.GetMask("Monster");
    }

    // Update is called once per frame
    void Update()
    {
        /* Update Pull timer */

        m_monsterPullTimer += Time.deltaTime;

        /* Compute Vision Effects */

        Vector3 weightedDir = Vector3.zero;
        float distanceSum = 0.0f;

        for(int i = 0; i < m_numRayCasts; ++i)
        {
            Vector3 rayDir = transform.rotation * m_rayDirections[i];

            if(Physics.Raycast(new Ray(transform.position, rayDir), out RaycastHit hit, m_viewRange, m_visionMask, QueryTriggerInteraction.Ignore))
            {
                m_rayHitDistances[i] = hit.distance;
                weightedDir += hit.distance * rayDir;
                distanceSum += hit.distance;
            }
            else
            {
                m_rayHitDistances[i] = m_viewRange;
                weightedDir += m_viewRange * rayDir;
                distanceSum += m_viewRange;
            }
        }

        if(distanceSum > 0.0f)
            weightedDir /= distanceSum;

        /* Backwards Bias */

        weightedDir -= m_backwardsBias * transform.forward;
        weightedDir = weightedDir.normalized;

        m_lastWeightedDir = weightedDir;

        /* Forward motion */

        Vector3 accel = transform.forward * m_accelMag;
        accel.y = 0.0f;

        /* Check if too far from the phy root */

        Vector3 phyRootDiff = m_phyRoot.position - transform.position;
        if(phyRootDiff.sqrMagnitude >= m_phyRootThreshold * m_phyRootThreshold)
        {
            accel += phyRootDiff * m_phyRootStrength;
        }

        /* Rotation Motion */

        Quaternion.FromToRotation(transform.forward, weightedDir.normalized).ToAngleAxis(out float angle, out Vector3 axis);
        if (axis[1] < 0.0f)
            angle *= -1.0f;

        float angAccel = Mathf.Min(Mathf.Abs(angle), 10.0f * m_accelMag) * Mathf.Sign(angle);

        /* Compute Monster pull scale */

        float pullScale = Mathf.Sin(m_monsterPullTimer * m_monsterPullFrequency);
        if (pullScale < 0.0f)
            pullScale *= 0.1f;
        else if (pullScale > 0.5f)
            pullScale = 1.0f;

        /* Apply motion */

        m_controlVelocity = m_velDecayRate * m_controlVelocity + pullScale * accel * Time.deltaTime;
        transform.position += m_controlVelocity * Time.deltaTime;
        transform.rotation *= Quaternion.AngleAxis(angAccel * Time.deltaTime, Vector3.up);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        /* Draw Forward */

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);

        /* Weighted Dir */

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + m_lastWeightedDir.normalized);

        /* Draw Vision Sensors */

        for(int i = 0; i < m_numRayCasts; ++i)
        {
            Vector3 rayDir = transform.rotation * m_rayDirections[i];
            Gizmos.color = Color.Lerp(Color.red, Color.green, m_rayHitDistances[i] / m_viewRange);
            Gizmos.DrawLine(transform.position, transform.position + rayDir * m_viewRange);
        }
    }
}
