using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Door : MonoBehaviour, IInteractuable
{
    protected bool _opened = false;
    public bool locked;
    protected Animator _an;
    protected Queue<string> _states;

    protected virtual void Start()
    {
        _an = GetComponent<Animator>();
        _states = new Queue<string>();
        _states.Enqueue("Open");
        _states.Enqueue("Close");
    }

    public virtual void Use()
    {
        if (locked) return;
        var dq = _states.Dequeue();
        _states.Enqueue(dq);

        _an.CrossFadeInFixedTime(dq, .1f);
        _opened = !_opened;
    }

}
