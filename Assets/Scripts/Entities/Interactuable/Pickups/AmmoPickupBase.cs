using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
public class AmmoPickupBase : MonoBehaviour
{
    [SerializeField] protected int _ammoGiven;
    public AmmoTypes ammoType;
    public AudioClip pickedSound;
    protected AudioSource _src;

    void Awake()
    {
        _src = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            if (player.ammoReserve[ammoType] < player.MaxAmmoReserve[(int)ammoType])
            {
                other.GetComponent<PlayerController>().OnPickedUpAmmo(_ammoGiven, ammoType);
                _src.PlayOneShot(pickedSound);
                DisablePickable();
            }
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
