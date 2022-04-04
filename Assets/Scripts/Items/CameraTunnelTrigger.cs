using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProceduralAnimation;

public class CameraTunnelTrigger : MonoBehaviour
{
    [SerializeField]
    CameraController m_cameraController;

    List<GameObject> m_playerTriggers = new List<GameObject>();

    int m_playerMask;

    public static CameraController.CameraSettings tunnelCameraSettings
    {
        get
        {
            CameraController.CameraSettings set;
            set.distance = 6.0f;
            set.pitch = 3.0f;
            return set;
        }
    }

    private void Awake()
    {
        m_playerMask = LayerMask.NameToLayer("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == m_playerMask)
        {
            if (other.gameObject.GetComponent<WalkBody>() == null)
                return;

            m_playerTriggers.Add(other.gameObject);
            m_playerTriggers = m_playerTriggers.Where(x => x != null && x.layer == m_playerMask).ToList();

            if(m_playerTriggers.Count == 1)
                m_cameraController.SetSettings(tunnelCameraSettings, 0.3f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == m_playerMask)
        {
            if (other.gameObject.GetComponent<WalkBody>() == null)
                return;

            m_playerTriggers.Remove(other.gameObject);
            m_playerTriggers = m_playerTriggers.Where(x => x != null && x.layer == m_playerMask).ToList();

            if(m_playerTriggers.Count == 0)
                m_cameraController.RevertToOriginalSettings();
        }
    }
}
