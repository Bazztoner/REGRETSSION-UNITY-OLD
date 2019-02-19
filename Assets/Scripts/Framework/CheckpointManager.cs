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
}
