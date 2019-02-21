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

    public static HashSet<ISaveable> savedObjects = new HashSet<ISaveable>();
    public Checkpoint lastCheckpoint = null;

    public static void AddToSavedObjects(ISaveable obj)
    {
        if (savedObjects == null) savedObjects = new HashSet<ISaveable>();

        if (!savedObjects.Contains(obj))
        {
            savedObjects.Add(obj);
        }
    }

    public void SetCheckpoint(Checkpoint last)
    {
        lastCheckpoint = last;
        SaveAll();
    }

    public void SaveAll()
    {
        foreach (var item in savedObjects)
        {
            item.SaveData();
        }
    }

    public void OnPlayerDeath()
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
        savedObjects = null;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    void LoadFromLastCheckpoint()
    {
        foreach (var item in savedObjects)
        {
            item.SaveData();
        }
    }

}
