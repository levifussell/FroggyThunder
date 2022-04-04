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
    public Action onSacrifice = null;

    bool m_isDead = false;
    bool m_isSacrificed = false;

    int m_monsterLayer;

    /* References From Kill */

    List<GameObject> m_gameObjectsFromKill;

    /* Static Params */

    static string FROG_SKIN_MATERIAL = "CharacterArm";
    static string FROG_SKIN_MATERIAL2 = "Material.003";
    static Color FROG_DEATH_COLOR = new Color(255 / 255.0f, 253 / 255.0f, 196 / 255.0f); 

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
        if(!m_isDead && collision.gameObject.layer == m_monsterLayer)
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

        m_gameObjectsFromKill = new List<GameObject>();

        /* Play Death Audio */

        AudioSource acPlayer = m_playerTransform.GetComponentInChildren<AudioSource>();
        acPlayer.Play();

        /* Turn into skeleton */

        foreach(MeshRenderer mr in m_playerTransform.GetComponentsInChildren<MeshRenderer>())
        {
            foreach(Material m in mr.materials)
            {
                if (m.name.Contains(FROG_SKIN_MATERIAL) || m.name.Contains(FROG_SKIN_MATERIAL2))
                    m.color = FROG_DEATH_COLOR;
            }
        }

        /* Build Arms */

        ArmBuilder ac = m_playerTransform.GetComponentInChildren<ArmBuilder>();
        foreach(GameObject h in ac.physHands)
        {
            ConfigurableJoint joint = ac.m_phyShoulder.gameObject.AddComponent<ConfigurableJoint>();
            joint.SetPositionJointMotions(ConfigurableJointMotion.Locked);
            joint.SetPdParamters(200.0f, 2.0f, 200.0f, 2.0f, 100.0f);
            joint.connectedBody = h.GetComponent<Rigidbody>();
            Destroy(h.GetComponent<VelocityController>());

            MeshRenderer mr = h.GetComponent<MeshRenderer>();
            mr.material.color = Color.white;

            m_gameObjectsFromKill.Add(ac.m_phyShoulder.gameObject);
            m_gameObjectsFromKill.Add(h);
        }

        /* Remove Flashlight */

        //Destroy(ac.flashlightObj.gameObject);
        ac.flashlightObj.Drop();
        ac.flashlightObj.TurnOff();

        /* Build Feet */

        WalkAnimator wa = m_playerTransform.GetComponentInChildren<WalkAnimator>();
        foreach(WalkFoot f in wa.feet)
        {
            ConfigurableJoint joint = wa.m_body.gameObject.AddComponent<ConfigurableJoint>();
            joint.SetPositionJointMotions(ConfigurableJointMotion.Locked);
            joint.SetPdParamters(200.0f, 2.0f, 200.0f, 2.0f, 100.0f);
            joint.connectedBody = f.footObjectPhy.GetComponent<Rigidbody>();
            Destroy(f.footObjectPhy.GetComponent<VelocityController>());
            Destroy(f.footObjectPhy.GetComponent<OnCollisionPlay>());

            m_gameObjectsFromKill.Add(f.footObjectPhy);
        }

        Destroy(m_playerTransform.GetComponentInChildren<ArmController>());
        Destroy(m_playerTransform.GetComponentInChildren<WalkAnimator>());
        Destroy(m_playerTransform.GetComponentInChildren<CharacterController>());
        Destroy(m_playerTransform.GetComponentInChildren<WalkBody>());

        /* Destroy sounds */

        foreach(OnCollisionPlay op in GetComponentsInChildren<OnCollisionPlay>())
        {
            Destroy(op);
        }

        /* De-parent all hands */

        foreach (Rigidbody r in allRigidbodies)
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
                r.drag = 1.0f;
                r.angularDrag = 1.0f;
                r.velocity *= 0.0f;
                r.angularVelocity *= 0.0f;
            }
        }

        foreach(Transform t in m_playerTransform.GetComponentsInChildren<Transform>())
        {
            t.gameObject.layer = LayerMask.NameToLayer("Character");
        }

        /* Destroy child collision calls */

        foreach(ChildCollisionCheck cc in m_playerTransform.GetComponentsInChildren<ChildCollisionCheck>())
        {
            Destroy(cc);
        }

        /* Invoke Kill Events */

        onKill?.Invoke();
        onKill = null;

        ///* Destroy the Kill call */

        //Destroy(this);
    }

    public bool Sacrifice(Vector3 sacrificePosition, Vector3 altarRight)
    {
        if (m_isSacrificed)
            return false;

        if (!m_isDead)
        {
            Debug.LogError("Cannot sacrifice if not dead.");
            return false;
        }

        foreach (Transform t in m_playerTransform.GetComponentsInChildren<Transform>())
        {
            m_gameObjectsFromKill.Add(t.gameObject);
        }

        m_isSacrificed = true;

        StartCoroutine(DoSacrifice(sacrificePosition, altarRight));

        return true;
    }

    IEnumerator DoSacrifice(Vector3 sacrificePosition, Vector3 altarRight)
    {
        float totalSacrificeTime = 0.0f;
        float MAX_SACRIFICE_TIME = 8.0f;

        /* Disable Gravity */

        foreach (GameObject g in m_gameObjectsFromKill)
        {
            Rigidbody rb = g.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                rb.velocity *= 0.0f;
                rb.angularDrag = 10.0f;
                rb.drag = 1.0f;
                //rb.freezeRotation = true;
            }
        }

        /* Make Rigid */

        foreach (GameObject g in m_gameObjectsFromKill)
        {
            ConfigurableJoint[] cn = g.GetComponentsInChildren<ConfigurableJoint>();
            if (cn != null)
            {
                foreach (ConfigurableJoint c in cn)
                    //c.SetPdParamters(100.0f, 1.0f, 100.0f, 1.0f, 100.0f);
                    c.SetAllJointMotions(ConfigurableJointMotion.Locked);
            }
        }

        /* Float Upwards Stage */

        Rigidbody bodyRb = m_playerTransform.GetComponentInChildren<ArmBuilder>().GetComponent<Rigidbody>();
        Destroy(bodyRb.GetComponent<Collider>());

        //float timeFloatUpward = 4.0f;
        float floatSpeed = 0.02f;
        Vector3 diff = Vector3.zero;
        do
        {
            diff = sacrificePosition - bodyRb.position;
            bodyRb.velocity = (diff * floatSpeed) / Time.fixedDeltaTime;
            bodyRb.angularVelocity = Vector3.zero;

            yield return new WaitForFixedUpdate();

            //Debug.Log("Floating " + diff.magnitude);

            totalSacrificeTime += Time.fixedDeltaTime;
            if (totalSacrificeTime > MAX_SACRIFICE_TIME)
                break;

        } while (diff.magnitude > 1.0f);

        /* Rotate Stage */

        bodyRb.isKinematic = true;

        foreach (GameObject g in m_gameObjectsFromKill)
        {
            ConfigurableJoint[] cn = g.GetComponentsInChildren<ConfigurableJoint>();
            if (cn != null)
            {
                foreach (ConfigurableJoint c in cn)
                    c.SetAllJointMotions(ConfigurableJointMotion.Locked);
            }
        }

        foreach (GameObject g in m_gameObjectsFromKill)
        {
            Rigidbody rb = g.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.drag = 0.1f;
                rb.angularDrag = 0.1f;
                //rb.freezeRotation = false;
            }
        }

        Quaternion targetRotation = Quaternion.LookRotation(Vector3.up, altarRight);
        Quaternion rotDiff;
        float angle = 0.0f;
        do
        {
            rotDiff = Quaternion.Inverse(bodyRb.rotation) * targetRotation;
            rotDiff.ToAngleAxis(out angle, out Vector3 axis);
            bodyRb.rotation *= Quaternion.AngleAxis(angle * 1.0f * Time.fixedDeltaTime, axis);

            yield return new WaitForFixedUpdate();

            //Debug.Log("Rotatine " + angle);

            totalSacrificeTime += Time.fixedDeltaTime;
            if (totalSacrificeTime > MAX_SACRIFICE_TIME)
                break;

        } while (angle > 1.0f);

        /* Destroy Stage */

        foreach (GameObject g in m_gameObjectsFromKill)
        {
            Rigidbody rb = g.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }

            Collider c = g.GetComponent<Collider>();
            if (c != null)
            {
                Destroy(c);
            }

            if (g.name.Equals("Leg"))
            {
                Destroy(g);
            }
        }

        float explodeTimer = 2.0f;
        float scaleRate = 1.0f / (explodeTimer * 10.0f);

        while (explodeTimer > 0.0f)
        {
            foreach (GameObject g in m_gameObjectsFromKill)
            {
                if (g == null)
                    continue;

                Vector3 dToBody = bodyRb.transform.position - g.transform.position;

                g.transform.position += -dToBody.normalized * Time.fixedDeltaTime * 1.0f;
                g.transform.localScale -= Vector3.one * scaleRate * Time.fixedDeltaTime;
                g.transform.localScale = Vector3Extensions.Clamp(g.transform.localScale, 0, 1);
            }

            yield return new WaitForFixedUpdate();
            explodeTimer -= Time.fixedDeltaTime;
            //Debug.Log("Explode time " + explodeTimer);
        }

        onSacrifice?.Invoke();

        /* Cleanup */

        foreach (GameObject g in m_gameObjectsFromKill)
        {
            if(g != null)
            {
                Destroy(g);
            }    
        }

        Destroy(this.gameObject);

        /* Destroy the Kill call */

        Destroy(this);
    }

}
