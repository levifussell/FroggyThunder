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

    //[SerializeField]
    //public Transform m_monsterTransform = null;

    [SerializeField]
    CameraController m_cameraController = null;

    [SerializeField]
    PlayerSpawner m_playerSpawner = null;

    float m_startTimer = 0.0f;

    /* Scenario Flags */

    bool m_scenarioIntro1 = false;
    bool m_scenarioIntro2 = false;
    bool m_scenarioGrabInstruction = false;
    bool m_scenarioGrabInstruction2 = false;
    bool m_scenarioPlayerSeesMonster = false;
    bool m_scenarioPlayerSeesAltar = false;
    bool m_scenarioPlayerSeesBody = false;
    bool m_scenarioFirstDeath = false;
    bool m_scenarioMonsterSpawn = false;

    private void Update()
    {
        if (m_cameraController.followTransform == null)
            return;

        if(m_startTimer == 0)
        {
            m_cameraController.followTransform.GetComponent<CharacterController>().enabled = false;
        }

        m_startTimer += Time.deltaTime;

        Transform m_playerTransform = m_cameraController.followTransform;

        /* Check for scenarios */

        if(!m_scenarioIntro1 && m_startTimer > 2.0f)
        {
            m_dialogueUI.BeginNewDialogue(INSTRUCTION_DIALOGUE_OPTIONS[0]);
            m_scenarioIntro1 = true;
        }
        else if(!m_scenarioIntro2 && m_startTimer > 10.0f)
        {
            m_dialogueUI.BeginNewDialogue(INSTRUCTION_DIALOGUE_OPTIONS[1]);
            m_scenarioIntro2 = true;
        }
        else if(!m_scenarioGrabInstruction && m_startTimer > 17.0f)
        {
            m_dialogueUI.BeginNewDialogue(INSTRUCTION_DIALOGUE_OPTIONS[5]);
            m_scenarioGrabInstruction = true;
        }
        else if(!m_scenarioGrabInstruction2 && m_startTimer > 23.0f)
        {
            m_dialogueUI.BeginNewDialogue(INSTRUCTION_DIALOGUE_OPTIONS[6]);
            m_scenarioGrabInstruction2 = true;

            m_cameraController.followTransform.GetComponent<CharacterController>().enabled = true;
        }

        if (Physics.SphereCast(new Ray(m_playerTransform.position, m_playerTransform.forward), 1.0f, out RaycastHit hit, 10.0f, ~0, QueryTriggerInteraction.Ignore))
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
                if (hit.collider.gameObject.CompareTag("Door"))
                {
                    m_dialogueUI.BeginNewDialogue(INSTRUCTION_DIALOGUE_OPTIONS[4]);
                    m_scenarioPlayerSeesAltar = true;
                }
            }

            if (!m_scenarioPlayerSeesBody && m_playerTransform.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Character"))
                {
                    m_dialogueUI.BeginNewDialogue(INSTRUCTION_DIALOGUE_OPTIONS[3]);
                    m_scenarioPlayerSeesBody = true;
                }
            }
        }

        if (!m_scenarioFirstDeath && m_playerSpawner.numSpawns > 1)
        {
            m_dialogueUI.BeginNewDialogue(INSTRUCTION_DIALOGUE_OPTIONS[7]);
            m_scenarioFirstDeath = true;
        }

        if (!m_scenarioMonsterSpawn && m_playerSpawner.monsterSpawn)
        {
            m_dialogueUI.BeginNewDialogue(INSTRUCTION_DIALOGUE_OPTIONS[8]);
            m_scenarioMonsterSpawn = true;
        }
    }

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        //StartDialogue();
    }

    public void PlayCustomDialogue(string dialogue)
    {
        m_dialogueUI.BeginNewDialogue(dialogue);
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
        "Oh no, the lights just went out in my Thunder Cave.",
        "I need to get out of here...",
        "What in the f*uit fly is that thing!",
        "Is that MY body??",
        "What's this altar... and this door? I don't remember putting those there.",
        "[Left click to grab + release]",
        "[WASD to move]",
        "Holy tadpoles. \n...what happened.\n Did I just die?!",
        "What was that noise? Hello?",
    };

    static string[] RANDOM_DIALOGUE_OPTIONS = 
    {
        "      AHHHHH!      \n ...sorry, I just had to scream.",
        "Oh please, oh please, get me out of here.",
        "This place creeps me out.",
    };
}
