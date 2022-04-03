using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProceduralAnimation;

public class CharacterKiller : MonoBehaviour
{
    [SerializeField]
    Transform m_playerTransform = null;

    [SerializeField]
    bool m_manualKill = false;

    public Action onKill = null;

    bool m_isDead = false;

    int m_monsterLayer;

    private void Awake()
    {
        m_monsterLayer = LayerMask.NameToLayer("Monster");
    }

    private void Start()
    {
        foreach(Rigidbody rb in transform.GetComponentsInChildren<Rigidbody>())
        {
            ChildCollisionCheck cc = rb.gameObject.AddComponent<ChildCollisionCheck>();
            cc.onCollisionEnter += CollisionCheck;
        }
    }

    public void Update()
    {
        if(m_manualKill)
        {
            Kill();
            m_manualKill = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        CollisionCheck(collision);
    }

    void CollisionCheck(Collision collision)
    {
        if(collision.gameObject.layer == m_monsterLayer)
        {
            Kill();
        }
    }

    public void Kill()
    {
        if (m_isDead)
        {
            Debug.LogError("Multiple Kill Calls");
            return;
        }
        
        m_isDead = true;

        Rigidbody[] allRigidbodies = m_playerTransform.GetComponentsInChildren<Rigidbody>();

        /* Build Arms */

        ArmBuilder ac = m_playerTransform.GetComponentInChildren<ArmBuilder>();
        foreach(GameObject h in ac.physHands)
        {
            ConfigurableJoint joint = ac.m_phyShoulder.gameObject.AddComponent<ConfigurableJoint>();
            joint.SetAllJointMotions(ConfigurableJointMotion.Locked);
            joint.connectedBody = h.GetComponent<Rigidbody>();
            Destroy(h.GetComponent<VelocityController>());
        }

        /* Build Feet */

        WalkAnimator wa = m_playerTransform.GetComponentInChildren<WalkAnimator>();
        foreach(WalkFoot f in wa.feet)
        {
            ConfigurableJoint joint = wa.m_body.gameObject.AddComponent<ConfigurableJoint>();
            joint.SetAllJointMotions(ConfigurableJointMotion.Locked);
            joint.connectedBody = f.footObjectPhy.GetComponent<Rigidbody>();
            Destroy(f.footObjectPhy.GetComponent<VelocityController>());
        }

        Destroy(m_playerTransform.GetComponentInChildren<ArmController>());
        Destroy(m_playerTransform.GetComponentInChildren<WalkAnimator>());
        Destroy(m_playerTransform.GetComponentInChildren<CharacterController>());
        Destroy(m_playerTransform.GetComponentInChildren<WalkBody>());

        /* De-parent all hands */

        foreach(Rigidbody r in allRigidbodies)
        {
            if(r.name.Equals("Hand"))
            {
                Destroy(r.gameObject);
            }

            if(r.name.Equals("Foot"))
            {
                Destroy(r.gameObject);
            }
        }

        foreach(Rigidbody r in allRigidbodies)
        {
            if(r.gameObject != null)
            {
                r.isKinematic = false;
                r.useGravity = true;
                r.tag = "Grabbable";
                r.gameObject.layer = LayerMask.NameToLayer("Character");
            }
        }

        /* Destroy child collision calls */

        foreach(ChildCollisionCheck cc in m_playerTransform.GetComponentsInChildren<ChildCollisionCheck>())
        {
            Destroy(cc);
        }

        /* Invoke Kill Events */

        onKill?.Invoke();
        onKill = null;

        /* Destroy the Kill call */

        Destroy(this);
    }

}
