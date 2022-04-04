using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneTimeDialogueTrigger : MonoBehaviour
{
    [SerializeField]
    string m_triggerDialogue = "HI!";

    [SerializeField]
    PlayerDialogueManager m_dialogueManager = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
            return;

        m_dialogueManager.PlayCustomDialogue(m_triggerDialogue);
        Destroy(this.gameObject);
    }
}
