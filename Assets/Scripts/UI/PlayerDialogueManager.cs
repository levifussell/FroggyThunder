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

    /* Objects to Track */

    [SerializeField]
    Transform m_monsterTransform = null;

    [SerializeField]
    CameraController m_cameraController = null;

    float m_startTimer = 0.0f;

    /* Scenario Flags */

    bool m_scenarioIntro1 = false;
    bool m_scenarioIntro2 = false;
    bool m_scenarioPlayerSeesMonster = false;
    bool m_scenarioPlayerSeesAltar = false;
    bool m_scenarioPlayerSeesBody = false;

    private void Update()
    {
        m_startTimer += Time.deltaTime;

        if (m_cameraController.followTransform == null)
            return;

        Transform m_playerTransform = m_cameraController.followTransform;

        /* Check for scenarios */

        if(!m_scenarioIntro1 && m_startTimer > 1.0f)
        {
            m_dialogueUI.BeginNewDialogue(INSTRUCTION_DIALOGUE_OPTIONS[0]);
            m_scenarioIntro1 = true;
        }
        else if(!m_scenarioIntro2 && m_startTimer > 7.0f)
        {
            m_dialogueUI.BeginNewDialogue(INSTRUCTION_DIALOGUE_OPTIONS[1]);
            m_scenarioIntro2 = true;
        }

        if (Physics.SphereCast(new Ray(m_playerTransform.position, m_playerTransform.forward), 0.4f, out RaycastHit hit, 10.0f, ~0, QueryTriggerInteraction.Ignore))
        {
            if (!m_scenarioPlayerSeesMonster)
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Monster"))
                {
                    m_dialogueUI.BeginNewDialogue(INSTRUCTION_DIALOGUE_OPTIONS[2]);
                    m_scenarioPlayerSeesMonster = true;
                }
            }

            if (!m_scenarioPlayerSeesAltar)
            {
                if (hit.collider.gameObject.CompareTag("Altar"))
                {
                    m_dialogueUI.BeginNewDialogue(INSTRUCTION_DIALOGUE_OPTIONS[4]);
                    m_scenarioPlayerSeesAltar = true;
                }
            }

            if (!m_scenarioPlayerSeesBody)
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Character"))
                {
                    m_dialogueUI.BeginNewDialogue(INSTRUCTION_DIALOGUE_OPTIONS[3]);
                    m_scenarioPlayerSeesBody = true;
                }
            }
        }

    }

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        //StartDialogue();
    }

    void StartDialogue()
    {
        StopAllCoroutines();
        StartCoroutine(RunNewDialogue());
    }

    IEnumerator RunNewDialogue()
    {
        yield return new WaitForSeconds(Random.Range(m_minTimeToNextDialogueSeconds, m_maxTimeToNextDialogueSeconds));
        m_dialogueUI.BeginNewDialogue(RANDOM_DIALOGUE_OPTIONS[Random.Range(0, RANDOM_DIALOGUE_OPTIONS.Length)]);
        StartDialogue();
    }

    static string[] INSTRUCTION_DIALOGUE_OPTIONS =
    {
        "Dang, the lights are out.",
        "I need to find a way out of here...",
        "What in the f*uit fly is that thing!",
        "Is that MY body??",
        "Oh geez...that's not an altar. Tell me that's not an altar.",
    };

    static string[] RANDOM_DIALOGUE_OPTIONS = 
    {
        "      AHHHHH!      \n ...sorry, I just had to scream.",
        "Oh please, oh please, get me out of here.",
        "This place creeps me out.",
    };
}
