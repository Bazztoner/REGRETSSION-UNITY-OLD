using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LiftButton : MonoBehaviour, IInteractuable
{
    [SerializeField] Lift _lift;

    void Start()
    {
        if (!_lift) _lift = GetComponentInParent<Lift>();       
    }

    public void Use()
    {
        _lift.OnButtonInteract();
    }
}
