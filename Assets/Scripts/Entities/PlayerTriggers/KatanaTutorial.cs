using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class KatanaTutorial : MonoBehaviour
{
    [SerializeField] DestructibleBase _destruct;

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() is PlayerController player)
        {
            TutorialManager.Instance.StartKatana(_destruct);
            gameObject.SetActive(false);
        }
    }
}
