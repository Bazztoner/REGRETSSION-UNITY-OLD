using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HUD_Tooltip : MonoBehaviour
{
    Animator _an;

    void Awake()
    {
        _an = GetComponent<Animator>();
    }

    void OnEnable()
    {
        if (!_an) _an = GetComponent<Animator>();
        _an.CrossFadeInFixedTime("Show", .1f);
    }

    public void Hide()
    {
        _an.CrossFadeInFixedTime("Hide", .1f);
        Invoke("HideMe", 2f);
    }

    public void Hide(float delay)
    {
        _an.CrossFadeInFixedTime("Hide", .1f);
        Invoke("HideMe", delay);
    }

    void HideMe()
    {
        gameObject.SetActive(false);
    }
}
