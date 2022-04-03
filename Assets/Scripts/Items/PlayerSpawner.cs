using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject m_playerPrefab = null;

    [SerializeField]
    CameraController m_cameraController = null;

    [SerializeField]
    float m_spawnDelayTimeSeconds = 1.0f;

    private void Awake()
    {
        SpawnNewPlayer();
    }

    void SpawnNewPlayer()
    {
        GameObject newPlayer = GameObject.Instantiate(m_playerPrefab);
        newPlayer.transform.position = new Vector3(transform.position.x, newPlayer.transform.position.y, transform.position.z);
        newPlayer.transform.rotation = transform.rotation;

        m_cameraController.followTransform = newPlayer.GetComponentInChildren<CharacterController>().transform;

        CharacterKiller characterKiller = newPlayer.GetComponent<CharacterKiller>();
        characterKiller.onKill += StartTimedSpawnNewPlayer;

    }

    void StartTimedSpawnNewPlayer()
    {
        StartCoroutine(TimedSpawnNewPlayer(m_spawnDelayTimeSeconds));
    }

    IEnumerator TimedSpawnNewPlayer(float timeSeconds)
    {
        yield return new WaitForSeconds(timeSeconds);
        SpawnNewPlayer();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position, Vector3.one * 0.4f);
    }
}
