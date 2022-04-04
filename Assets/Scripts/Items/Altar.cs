using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Altar : MonoBehaviour
{
    [SerializeField]
    Vector3 m_localSacrificePosition = Vector3.zero;

    [SerializeField]
    ParticleSystem m_sacrificeParticles = null;

    [SerializeField]
    Light m_sacrificeLight = null;

    [SerializeField]
    GameObject m_altarTop = null;

    [SerializeField]
    Door m_doorToOpen = null;

    Flashlight m_previousFlash;

    Queue<CharacterKiller> m_sacrificeList = new Queue<CharacterKiller>();

    Material m_altarTopMaterial = null;

    AudioSource m_audioSource = null;

    bool m_isSacrificing = false;

    private void Awake()
    {
        m_sacrificeParticles.Stop();
        m_altarTopMaterial = m_altarTop.GetComponent<MeshRenderer>().material;

        m_audioSource = GetComponent<AudioSource>();
        m_audioSource.volume = 0.1f;

        Color c = m_altarTopMaterial.color;
        c = new Color(m_altarTopMaterial.color.r, m_altarTopMaterial.color.g, m_altarTopMaterial.color.b, 0.0f);
        m_altarTopMaterial.color = c;

        m_sacrificeLight.intensity = 0.0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == null)
            return;

        if(other.gameObject.layer == LayerMask.NameToLayer("Character"))
        {
            CharacterKiller ck = other.gameObject.GetComponentInParent<CharacterKiller>();
            if(ck != null && !m_sacrificeList.Contains(ck))
            {
                m_sacrificeList.Enqueue(ck);
            }
        }
    }

    private void Update()
    {
        if (m_isSacrificing)
            return;

        if(m_sacrificeList.Count > 0)
        {
            CharacterKiller ck = m_sacrificeList.Dequeue();
            
            if(ck != null)
            {
                Grabbable gb = ck.GetComponentInChildren<Grabbable>();
                if (gb != null)
                    Destroy(gb);

                BeginSacrificeEffect();
                if(ck.Sacrifice(this.transform.TransformPoint(m_localSacrificePosition), transform.forward))
                {
                    ck.onSacrifice -= EndSacrificeEffect;
                    ck.onSacrifice += EndSacrificeEffect;
                }
                else if(m_isSacrificing)
                {
                    EndSacrificeEffect();
                }
            }
        }
    }

    void BeginSacrificeEffect()
    {
        m_isSacrificing = true;
        StopAllCoroutines();
        StartCoroutine(LightTurnOn());
        StartCoroutine(AudioUp());
        StartCoroutine(RunBeginSacrificeEffect());
    }

    void EndSacrificeEffect()
    {
        m_isSacrificing = false;
        m_doorToOpen.AddNewSacrifice();
        StopAllCoroutines();
        StartCoroutine(LightTurnOff());
        StartCoroutine(AudioDown());
        StartCoroutine(RunEndSacrificeEffect());
    }

    IEnumerator RunBeginSacrificeEffect()
    {
        m_sacrificeParticles.Play();

        Color c = m_altarTopMaterial.color;

        // turn off flashlight.
        m_previousFlash = FindObjectsOfType<Flashlight>().Where(x => x.isOn).ToArray()[0];
        m_previousFlash.TurnOff();

        while (c.a < 1.0f)
        {
            c = new Color(c.r, c.g, c.b, c.a + 1.0f * Time.fixedDeltaTime);
            m_altarTopMaterial.color = c;

            yield return new WaitForFixedUpdate();
        }

        c = new Color(c.r, c.g, c.b, 1.0f);
        m_altarTopMaterial.color = c;
    }

    IEnumerator RunEndSacrificeEffect()
    {
        m_sacrificeParticles.Stop();

        Color c = m_altarTopMaterial.color;

        // turn on flashlight.
        if(m_previousFlash != null)
            m_previousFlash.TurnOn();

        while (c.a > 0.0f)
        {
            c = new Color(c.r, c.g, c.b, c.a - 1.0f * Time.fixedDeltaTime);
            m_altarTopMaterial.color = c;

            yield return new WaitForFixedUpdate();
        }

        c = new Color(c.r, c.g, c.b, 0.0f);
        m_altarTopMaterial.color = c;
    }

    IEnumerator LightTurnOn()
    {
        while(m_sacrificeLight.intensity < 15.0f)
        {
            m_sacrificeLight.intensity += 10.0f * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        m_sacrificeLight.intensity = 15.0f;
    }

    IEnumerator LightTurnOff()
    {
        while(m_sacrificeLight.intensity > 0.0f)
        {
            m_sacrificeLight.intensity -= 10.0f * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        m_sacrificeLight.intensity = 0.0f;
    }

    IEnumerator AudioUp()
    {
        while(m_audioSource.volume < 1.0f)
        {
            m_audioSource.volume += 10.0f * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        m_audioSource.volume = 1.0f;
    }

    IEnumerator AudioDown()
    {
        while(m_audioSource.volume > 0.1f)
        {
            m_audioSource.volume -= 10.0f * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        m_audioSource.volume = 0.1f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawCube(this.transform.TransformPoint(m_localSacrificePosition), Vector3.one * 0.5f);
    }
}
