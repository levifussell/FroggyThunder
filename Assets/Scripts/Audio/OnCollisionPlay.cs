using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollisionPlay : MonoBehaviour
{
    AudioSource m_audioSource;

    public static OnCollisionPlay Attach3DWithAudioClip(GameObject gameObject, AudioClip clip)
    {
        AudioSource ac = gameObject.GetComponent<AudioSource>();
        
        if(ac == null)
        {
            ac = gameObject.AddComponent<AudioSource>();
        }

        OnCollisionPlay op = gameObject.AddComponent<OnCollisionPlay>();
        ac.clip = clip;
        ac.spatialBlend = 1.0f;
        ac.maxDistance = 10.0f;
        ac.volume = 0.3f;
        ac.rolloffMode = AudioRolloffMode.Linear;

        return op;
    }

    private void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Character"))
            return;

        m_audioSource.pitch = Random.Range(0.8f, 1.2f);
        m_audioSource.volume = Random.Range(0.4f, 1.0f);
        m_audioSource.Play();
    }
}
