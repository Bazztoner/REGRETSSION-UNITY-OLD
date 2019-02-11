using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LiftButton : MonoBehaviour, IInteractuable
{
    Lift _lift;

    void Start()
    {
        _lift = GetComponentInParent<Lift>();
    }

    public void Use()
    {
        _lift.OnButtonInteract();
    }
}
