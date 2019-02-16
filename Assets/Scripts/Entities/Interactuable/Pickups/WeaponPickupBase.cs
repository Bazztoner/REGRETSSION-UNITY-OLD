using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WeaponPickupBase : AmmoPickupBase
{
    [SerializeField] string _weaponName;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            player.OnPickedUpWeapon(_weaponName);
            player.OnPickedUpAmmo(_ammoGiven, ammoType);
            _src.PlayOneShot(pickedSound);
            DisablePickable();
        }
    }

}
