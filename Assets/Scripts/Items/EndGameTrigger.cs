using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameTrigger : MonoBehaviour
{
    [SerializeField]
    CameraController m_cameraController = null;

    [SerializeField]
    PlayerDialogueManager m_dialogueManager = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
            return;

        Destroy(m_cameraController.followTransform.GetComponent<CharacterController>());
        m_cameraController.rotationModeEnabled = true;

        m_dialogueManager.PlayCustomDialogue("La La La (The End) La La. *Ribbit*      \nLa La La *Ribbit* La La!");
    }
}
