using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LockedDoor : Door
{
    public KeysForDoors keyNeeded;
    PlayerController _player;
    SpriteRenderer _lockImage;

    protected override void Start()
    {
        base.Start();
        _player = FindObjectOfType<PlayerController>();
        _lockImage = GetComponentsInChildren<SpriteRenderer>().Where(x => x.gameObject.name == "Lock").First();
    }

    public override void Use()
    {
        if (!_player.GetIfKeyInInventory(keyNeeded)) return;
        base.Use();
        _lockImage.enabled = false;
    }
}
