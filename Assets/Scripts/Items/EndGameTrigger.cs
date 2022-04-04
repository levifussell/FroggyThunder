using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameTrigger : MonoBehaviour
{
    [SerializeField]
    CameraController m_cameraController = null;

    private void OnTriggerEnter(Collider other)
    {
        Destroy(m_cameraController.followTransform.GetComponent<CharacterController>());
        m_cameraController.rotationModeEnabled = true;
    }
}
