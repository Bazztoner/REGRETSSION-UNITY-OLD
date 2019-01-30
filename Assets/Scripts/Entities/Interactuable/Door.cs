using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Door : MonoBehaviour, IInteractuable
{
    bool _opened = false;
    Animator _an;
    Queue<string> _states;

    void Start ()
    {
        _an = GetComponent<Animator>();
        _states = new Queue<string>();
        _states.Enqueue("Open");
        _states.Enqueue("Close");
    }

    public void Use()
    {
        var dq = _states.Dequeue();
        _states.Enqueue(dq);

        _an.CrossFadeInFixedTime(dq, .1f);
        _opened = !_opened;
    }

}
