using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Key : MonoBehaviour
{
    public KeysForDoors keyType;
    AudioSource _src;
    public AudioClip pickedSound;

    void Awake()
    {
        _src = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player)
        {
            player.SetKeyOnInventory(keyType);
            _src.PlayOneShot(pickedSound);
            DisablePickable();
        }
    }

    protected void DisablePickable()
    {
        var cols = GetComponentsInChildren<Collider>();
        var rends = GetComponentsInChildren<Renderer>();
        foreach (var item in cols)
        {
            item.enabled = false;
        }
        foreach (var item in rends)
        {
            item.enabled = false;
        }

        Invoke("EndDisable", 5f);
    }

    void EndDisable()
    {
        gameObject.SetActive(false);
    }
}

public enum KeysForDoors
{
    RED,
    BLUE,
    YELLOW
}
