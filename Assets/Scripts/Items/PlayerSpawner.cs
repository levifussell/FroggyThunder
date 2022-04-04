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
    GameObject m_monsterPrefab = null;

    [SerializeField]
    float m_timeUntilMonsterSpawn = 10.0f;

    //[SerializeField]
    //Transform m_monsterTransform = null;

    [SerializeField]
    public List<Vector3> m_spawnPositions = new List<Vector3>();

    [SerializeField]
    public List<Quaternion> m_spawnRotations = new List<Quaternion>();

    [SerializeField]
    CameraController m_cameraController = null;

    //[SerializeField]
    //PlayerDialogueManager m_dialogueManager = null;

    [SerializeField]
    float m_spawnDelayTimeSeconds = 1.0f;

    int m_numSpawns = 0;
    public int numSpawns { get => m_numSpawns; }

    public bool monsterSpawn = false;

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

    private void Start()
    {
        StartCoroutine(TimedSpawnMonster(m_timeUntilMonsterSpawn));
    }

    void SpawnNewPlayer()
    {
        MonsterController mController = FindObjectOfType<MonsterController>();

        if (mController == null)
        {
            SpawnNewPlayer(2);
        }
        else
        {
            Transform m_monsterTransform = mController.transform;

            // Find best spawn point.

            List<float> distList = m_spawnPositions.Select(x => (x - m_monsterTransform.position).magnitude).ToList();
            int minIndex = distList.IndexOf(distList.Max());
            SpawnNewPlayer(minIndex);
        }
    }

    void SpawnNewPlayer(int index)
    {
        m_numSpawns++;

        Vector3 spawnPosition = m_spawnPositions[index];
        Quaternion spawnRotation = m_spawnRotations[index];

        GameObject newPlayer = GameObject.Instantiate(m_playerPrefab);
        newPlayer.transform.position = new Vector3(spawnPosition.x, newPlayer.transform.position.y, spawnPosition.z);
        newPlayer.transform.rotation = spawnRotation;

        if(m_cameraController.followTransform != null)
        {
            Destroy(m_cameraController.followTransform.GetComponentInChildren<AudioListener>());
        }
        m_cameraController.followTransform = newPlayer.GetComponentInChildren<CharacterController>().transform;
        m_cameraController.RevertToOriginalSettings();

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
    void SpawnMonster()
    {
        Transform m_playerTransform = FindObjectOfType<CharacterController>().transform;

        // Find best spawn point.

        List<float> distList = m_spawnPositions.Select(x => (x - m_playerTransform.position).magnitude).ToList();
        int minIndex = distList.IndexOf(distList.Max());
        SpawnMonster(minIndex);
    }

    void SpawnMonster(int index)
    {
        Vector3 spawnPosition = m_spawnPositions[index];
        Quaternion spawnRotation = m_spawnRotations[index];

        GameObject newMonster = GameObject.Instantiate(m_monsterPrefab);
        newMonster.transform.position = new Vector3(spawnPosition.x, newMonster.transform.position.y, spawnPosition.z);
        newMonster.transform.rotation = spawnRotation;

        //m_dialogueManager.m_monsterTransform = newMonster.transform;
        monsterSpawn = true;
    }

    IEnumerator TimedSpawnMonster(float timeSeconds)
    {
        yield return new WaitForSeconds(timeSeconds);
        SpawnMonster();
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
