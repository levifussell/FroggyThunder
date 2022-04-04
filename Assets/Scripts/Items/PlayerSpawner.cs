using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject m_playerPrefab = null;

    [SerializeField]
    Transform m_monsterTransform = null;

    [SerializeField]
    public List<Vector3> m_spawnPositions = new List<Vector3>();

    [SerializeField]
    public List<Quaternion> m_spawnRotations = new List<Quaternion>();

    [SerializeField]
    CameraController m_cameraController = null;

    [SerializeField]
    float m_spawnDelayTimeSeconds = 1.0f;

    private void Awake()
    {
        if(m_spawnPositions.Count == 0)
        {
            Debug.LogError("No spawn points made.");
        }

        if(m_spawnPositions.Count != m_spawnRotations.Count)
        {
            Debug.LogError("Spawn positions and rotations don't match.");
        }

        SpawnNewPlayer();
    }

    void SpawnNewPlayer()
    {
        // Find best spawn point.

        List<float> distList = m_spawnPositions.Select(x => (x - m_monsterTransform.position).magnitude).ToList();
        int minIndex = distList.IndexOf(distList.Max());
        SpawnNewPlayer(minIndex);
    }

    void SpawnNewPlayer(int index)
    {
        Vector3 spawnPosition = m_spawnPositions[index];
        Quaternion spawnRotation = m_spawnRotations[index];

        GameObject newPlayer = GameObject.Instantiate(m_playerPrefab);
        newPlayer.transform.position = new Vector3(spawnPosition.x, newPlayer.transform.position.y, spawnPosition.z);
        newPlayer.transform.rotation = spawnRotation;

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

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerSpawner))]
public class E_PlayerSpawner : Editor
{
    private void OnSceneGUI()
    {
        PlayerSpawner spawner = (PlayerSpawner)target;

        for(int i = 0; i < spawner.m_spawnPositions.Count; ++ i)
        {
            spawner.m_spawnPositions[i] = Handles.PositionHandle(spawner.m_spawnPositions[i], spawner.m_spawnRotations[i]);
            spawner.m_spawnRotations[i] = Handles.RotationHandle(spawner.m_spawnRotations[i], spawner.m_spawnPositions[i]);
        }
    }
}
#endif
