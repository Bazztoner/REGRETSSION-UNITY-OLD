using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
public class HealthPickupBase : MonoBehaviour
{
    [SerializeField] protected int _lifeGiven;
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
            if (player.CurrentHp < player.maxHp)
            {
                player.OnPickedUpLife(_lifeGiven);
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
