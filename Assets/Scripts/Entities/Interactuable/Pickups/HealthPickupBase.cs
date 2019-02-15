using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HealthPickupBase : MonoBehaviour
{
    [SerializeField] protected int _lifeGiven;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            if (player.CurrentHp < player.maxHp)
            {
                player.OnPickedUpLife(_lifeGiven);
                gameObject.SetActive(false);
            }
        }
    }
}
