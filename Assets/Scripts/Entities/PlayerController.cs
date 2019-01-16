using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PlayerController : MonoBehaviour
{
    public List<WeaponBase> allWeapons;
    byte _currentWpn = 0;

    List<KeyCode> _wpnKeys;

    bool _changingWeapon = false;

    void Start()
    {
        _wpnKeys = new List<KeyCode>();
        for (byte i = 1; i <= 9; i++)
        {
            //Alpha 0 is KeyCode 48.
            _wpnKeys.Add((KeyCode)Enum.ToObject(typeof(KeyCode), i + 48));
        }

        WeaponControlUtilities.Initialize();
        allWeapons = GetComponentsInChildren<WeaponBase>(true).OrderBy(X => X.wpnNumber).ToList();
    }

    void Update()
    {
        CheckChangeWeapon();
    }

    void CheckChangeWeapon()
    {
        for (byte i = 0; i < _wpnKeys.Count; i++)
        {
            if (i != _currentWpn)
            {
                if (Input.GetKeyDown(_wpnKeys[i]) && !_changingWeapon)
                {
                    _changingWeapon = true;
                    StartCoroutine(ChangeWeapon(i));
                    return;
                }
            }
        }
    }

    IEnumerator ChangeWeapon(byte indx)
    {
        allWeapons[_currentWpn].ChangeWeapon();

        yield return new WaitUntil(() => !allWeapons[_currentWpn].Drawn);

        _currentWpn = indx;

        allWeapons[_currentWpn].gameObject.SetActive(true);

        yield return new WaitUntil(() => allWeapons[_currentWpn].Drawn);

        _changingWeapon = false;
    }
}

public static class WeaponControlUtilities
{
    static Dictionary<byte, KeyCode> _keys;

    public static void Initialize()
    {
        _keys = new Dictionary<byte, KeyCode>();

        for (byte i = 1; i <= 9; i++)
        {
            _keys.Add(i, (KeyCode)Enum.ToObject(typeof(KeyCode), i + 48));
        }
    }

    public static KeyCode IndexToKey(byte indx)
    {
        if (_keys == null || !_keys.ContainsKey(indx))
        {
            Initialize();
        }

        return _keys[indx];
    }
}

