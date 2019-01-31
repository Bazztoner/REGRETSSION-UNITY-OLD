using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LockedDoor : Door
{
    public KeysForDoors keyNeeded;
    PlayerController _player;

    protected override void Start()
    {
        base.Start();
        _player = FindObjectOfType<PlayerController>();
    }

    public override void Use()
    {
        if (!_player.GetIfKeyInInventory(keyNeeded)) return;
        base.Use();
    }
}
