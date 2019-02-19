using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InteractTutorial : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() is PlayerController player)
        {
            TutorialManager.Instance.StartUse();
            gameObject.SetActive(false);
        }
    }
}
