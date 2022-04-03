using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField]
    float m_accelMag = 10.0f;

    [SerializeField]
    float m_velDecayRate = 0.9f;

    [SerializeField]
    float m_rotSpeed = 1.0f;

    [SerializeField]
    Transform m_phyRoot = null;

    [SerializeField]
    float m_phyRootThreshold = 0.1f;

    [SerializeField]
    float m_phyRootStrength = 10.0f;

    Vector3 m_controlVelocity;

    public Vector3 velocity { get => m_controlVelocity; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 accel = Vector3.zero;
        float verticalRotation = 0.0f;

        if(Input.GetKey(KeyCode.W))
        {
            accel += transform.forward;

            if(Input.GetKey(KeyCode.A))
            {
                verticalRotation -= m_rotSpeed;
            }

            if(Input.GetKey(KeyCode.D))
            {
                verticalRotation += m_rotSpeed;
            }
        }

        /* Check if too far from the phy root */

        Vector3 phyRootDiff = m_phyRoot.position - transform.position;
        if(phyRootDiff.sqrMagnitude >= m_phyRootThreshold * m_phyRootThreshold)
        {
            accel += phyRootDiff * m_phyRootStrength;
        }


        /* Apply motion */

        accel = accel.normalized * m_accelMag;
        accel.y = 0.0f;

        m_controlVelocity = m_velDecayRate * m_controlVelocity + accel * Time.deltaTime;
        transform.position += m_controlVelocity * Time.deltaTime;
        transform.rotation *= Quaternion.AngleAxis(verticalRotation * Time.deltaTime, Vector3.up);
    }
}
