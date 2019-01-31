using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public List<WeaponBase> allWeapons;
    CameraShake _camShake;
    CameraController _camController;
    Rigidbody _rb;
    byte _currentWpn;

    List<KeyCode> _wpnKeys;

    bool _changingWeapon = false, _grounded = false;
    public Camera cam;

    public float movementSpeed, jumpForce;

    Dictionary<KeysForDoors, bool> _keysOnInventory;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _keysOnInventory = new Dictionary<KeysForDoors, bool>();

        cam = GetComponentInChildren<Camera>();
        _camShake = cam.GetComponent<CameraShake>();
        _camController = cam.GetComponent<CameraController>();
        _rb = GetComponent<Rigidbody>();

        InitializeWeapons();
    }

    void InitializeWeapons()
    {
        _wpnKeys = new List<KeyCode>();
        for (byte i = 1; i <= 9; i++)
        {
            //Alpha 0 is KeyCode 48.
            _wpnKeys.Add((KeyCode)Enum.ToObject(typeof(KeyCode), i + 48));
        }

        WeaponControlUtilities.Initialize();
        allWeapons = GetComponentsInChildren<WeaponBase>(true).OrderBy(X => X.wpnNumber).ToList();
        _currentWpn = allWeapons.Where(x => x.isActiveAndEnabled).First().wpnNumber;
        _currentWpn--;
    }

    void Update()
    {
        CheckInteract();
        CheckJump();
        CheckChangeWeapon();
    }

    void FixedUpdate()
    {
        CheckMovement();
    }

    void CheckMovement()
    {
        var dir = transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal");
        var movVector = _rb.position + dir.normalized * Time.fixedDeltaTime * movementSpeed;
        _rb.MovePosition(movVector);
    }

    void CheckJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _grounded)
        {
            _rb.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
        }
    }

    void CheckInteract()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            var mask = HitscanLayers.BlockerLayerMask();
            var hits = Physics.RaycastAll(cam.transform.position, cam.transform.forward, 3, mask);
            IInteractuable interact;
            if (hits != null)
            {
                interact = hits.Select(x => x.collider.GetComponentInParent(typeof(IInteractuable)) as IInteractuable).Where(x => x != null).FirstOrDefault();
                if (interact != null) interact.Use();
            }
            else
            {
                //sound
            }
           
        }
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

    public void AddRecoil(float recoveryTime, float amount)
    {
        _camController.AddRecoil(recoveryTime, amount);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.LayerMatchesWith(LayerMask.NameToLayer("Floor"), LayerMask.NameToLayer("Jumpable")))
        {
            _grounded = true;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.LayerMatchesWith(LayerMask.NameToLayer("Floor"), LayerMask.NameToLayer("Jumpable")))
        {
            _grounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.LayerMatchesWith(LayerMask.NameToLayer("Floor"), LayerMask.NameToLayer("Jumpable")))
        {
            _grounded = false;
        }
    }

    public void SetKeyOnInventory(KeysForDoors key)
    {
        if (!_keysOnInventory.ContainsKey(key))
        {
            _keysOnInventory.Add(key, true);
        }
        else
        {
            _keysOnInventory[key] = true;
        }

    }

    public bool GetIfKeyInInventory(KeysForDoors key)
    {
        if (!_keysOnInventory.ContainsKey(key))
        {
            _keysOnInventory.Add(key, false);
        }

        return _keysOnInventory[key];
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

