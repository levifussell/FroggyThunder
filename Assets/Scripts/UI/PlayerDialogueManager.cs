using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDialogueManager : MonoBehaviour
{
    [SerializeField]
    DialogueUI m_dialogueUI = null;

    [SerializeField]
    float m_minTimeToNextDialogueSeconds = 10.0f;

    [SerializeField]
    float m_maxTimeToNextDialogueSeconds = 40.0f;

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        StartDialogue();
    }

    void StartDialogue()
    {
        StopAllCoroutines();
        StartCoroutine(RunNewDialogue());
    }

    IEnumerator RunNewDialogue()
    {
        yield return new WaitForSeconds(Random.Range(m_minTimeToNextDialogueSeconds, m_maxTimeToNextDialogueSeconds));
        m_dialogueUI.BeginNewDialogue(DIALOGUE_OPTIONS[Random.Range(0, DIALOGUE_OPTIONS.Length)]);
        StartDialogue();
    }

    static string[] DIALOGUE_OPTIONS = 
    {
        "WELCOME, WELCOME.",
        "AHHHHH!",
        "Oh please, get me out of here.",
    };
}
