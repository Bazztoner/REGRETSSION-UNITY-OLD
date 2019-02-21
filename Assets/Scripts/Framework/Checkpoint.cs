﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Checkpoint : MonoBehaviour
{
    bool available = true;
    public Transform spawnPos;
    public int life;

    void Awake()
    {
        spawnPos = transform.Find("CheckPointSpawn");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.LayerMatchesWith("Player") && available)
        {
            CheckpointManager.Instance.SetCheckpoint(this);
            available = false;
            life = Mathf.RoundToInt(other.gameObject.GetComponent<PlayerController>().CurrentHp / 2);
        }
    }
}
