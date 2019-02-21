using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Checkpoint : MonoBehaviour
{
    bool available = true;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.LayerMatchesWith("Player") && available)
        {
            CheckpointManager.Instance.SetCheckpoint(this);
            available = false;
        }
    }
}
