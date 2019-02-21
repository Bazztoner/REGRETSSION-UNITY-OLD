using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CheckpointManager: MonoBehaviour
{
	static CheckpointManager instance;
	public static CheckpointManager Instance
	{
		get
		{
			if(instance == null)
			{
				instance = FindObjectOfType<CheckpointManager>();
				if(instance == null)
				{
					instance = new GameObject("new CheckpointManager Object").AddComponent<CheckpointManager>().GetComponent<CheckpointManager>();
				}
			}
			return instance;
		}
	}
    public Checkpoint lastCheckpoint = null;
    PlayerController _player;

     void Awake()
    {
        _player = GameObject.FindObjectOfType<PlayerController>();
    }

    public void SetCheckpoint(Checkpoint last)
    {
        lastCheckpoint = last;
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
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    void LoadFromLastCheckpoint()
    {
        _player.transform.position = lastCheckpoint.spawnPos.position;
        _player.OnRespawn(lastCheckpoint.life);
        HUDController.Instance.OnRespawn();
    }
}
