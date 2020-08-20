using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    static CheckpointManager instance;
    public static CheckpointManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CheckpointManager>();
                if (instance == null)
                {
                    instance = new GameObject("new CheckpointManager Object").AddComponent<CheckpointManager>().GetComponent<CheckpointManager>();
                }
            }
            return instance;
        }
    }
    [HideInInspector] public Checkpoint lastCheckpoint = null;
    [SerializeField] GameObject _enemyContainer;

    List<Enemy> _allEnemies = new List<Enemy>();
    List<Vector3> _enemiesPos = new List<Vector3>();
    PlayerController _player;

    void Awake()
    {
        _player = GameObject.FindObjectOfType<PlayerController>();
        _allEnemies = _enemyContainer.GetComponentsInChildren<Enemy>().ToList();
    }

    public void SetCheckpoint(Checkpoint last)
    {
        lastCheckpoint = last;
        TutorialManager.Instance.StartCheckPoint();

        for (int i = 0; i < _allEnemies.Count; i++)
        {
            _enemiesPos.Add(_allEnemies[i].transform.position);
        }
    }

    public void OnPlayerDeath(float delay)
    {
        Invoke("OnPlayerDeath", delay);
    }

    void OnPlayerDeath()
    {
        if (!lastCheckpoint)
        {
            ResetScene();
            return;
        }
        else LoadFromLastCheckpoint();

    }

    void ResetScene()
    {
        instance = null;
        Invoke("LoadScene", 1f);
    }

    void LoadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void LoadFromLastCheckpoint()
    {
        for (int i = 0; i < _allEnemies.Count; i++)
        {
            if (_allEnemies[i].HP > 0) _allEnemies[i].transform.position = _enemiesPos[i];
        }

        _player.transform.position = lastCheckpoint.spawnPos.position;
        _player.transform.rotation = lastCheckpoint.spawnPos.rotation;
        _player.OnRespawn(lastCheckpoint.life);
        HUDController.Instance.OnRespawn();
    }
}
