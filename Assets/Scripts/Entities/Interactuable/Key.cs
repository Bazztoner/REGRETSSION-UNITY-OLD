using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Key : MonoBehaviour
{
    public KeysForDoors keyType;

    void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player)
        {
            player.SetKeyOnInventory(keyType);
            gameObject.SetActive(false);
        }
    }
}

public enum KeysForDoors
{
    RED,
    BLUE,
    YELLOW
}
