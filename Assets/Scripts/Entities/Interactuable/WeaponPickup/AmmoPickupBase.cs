using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AmmoPickupBase : MonoBehaviour
{
    [SerializeField] protected int _ammoGiven;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.GetComponent<PlayerController>().OnPickedUpAmmo(_ammoGiven);
            gameObject.SetActive(false);
        }
    }
}
