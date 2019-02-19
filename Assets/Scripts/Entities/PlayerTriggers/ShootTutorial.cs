using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShootTutorial : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() is PlayerController player)
        {
            TutorialManager.Instance.StartShoot();
            gameObject.SetActive(false);
        }
    }
}
