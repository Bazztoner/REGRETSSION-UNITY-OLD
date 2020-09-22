﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour, IDamageable
{
    public List<WeaponBase> allWeapons;

    [SerializeField]
    [Header("Bullets, Shells, Rockets, Cells, Energy, Cores")]
    int[] _initialAmmoReserve, _maxAmmoReserve;

    CameraShake _camShake;
    CameraController _camController;
    PlayerHead _head;
    Rigidbody _rb;
    [SerializeField] int _currentWeaponIndex, _currentWeaponSubIndex;


    List<KeyCode> _wpnKeys;

    bool _changingWeapon = false, _grounded = false;
    public Camera cam;

    public float movementSpeed, jumpForce;
    public int maxHp;
    float _currentHp;

    float _accelerationCoeficient = 12f;
    float _fow, _back, _left, _right;
    public float Fow { get => _fow; set => _fow = Mathf.Clamp01(value); }
    public float Back { get => _back; set => _back = Mathf.Clamp01(value); }
    public float Left { get => _left; set => _left = Mathf.Clamp01(value); }
    public float Right { get => _right; set => _right = Mathf.Clamp01(value); }

    bool _lockedByGame = false;

    Dictionary<KeysForDoors, bool> _keysOnInventory;

    public Dictionary<AmmoTypes, int> ammoReserve;

    public int[] MaxAmmoReserve { get => _maxAmmoReserve; }
    public float CurrentHp
    {
        get => Mathf.Round(_currentHp);
        set
        {
            _currentHp = Mathf.Round(Mathf.Clamp(value, 0, maxHp));
        }
    }

    public PlayerHead Head { get => _head; private set => _head = value; }
    public bool LockedByGame { get => _lockedByGame; private set => _lockedByGame = value; }

    void Awake()
    {
        InitializeAmmo();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        CurrentHp = maxHp;
        _keysOnInventory = new Dictionary<KeysForDoors, bool>();
    }

    void Start()
    {
        UpdateHP();

        cam = GetComponentInChildren<Camera>();
        _camShake = cam.GetComponent<CameraShake>();
        _camController = cam.GetComponent<CameraController>();
        _rb = GetComponent<Rigidbody>();
        Head = GetComponentInChildren<PlayerHead>();

        InitializeWeapons();
    }

    public void InitializeAmmo()
    {
        ammoReserve = new Dictionary<AmmoTypes, int>();
        for (int i = 0; i < _initialAmmoReserve.Length; i++)
        {
            ammoReserve.Add((AmmoTypes)i, _initialAmmoReserve[i]);
        }
    }

    void InitializeWeapons()
    {
        _wpnKeys = new List<KeyCode>();
        for (byte i = 0; i <= 9; i++)
        {
            //Number 0 is KeyCode 48.
            _wpnKeys.Add((KeyCode)Enum.ToObject(typeof(KeyCode), i + 48));
        }

        WeaponControlUtilities.Initialize();
        allWeapons = GetComponentsInChildren<WeaponBase>(true).OrderBy(X => X.wpnNumberX).ThenBy(x => x.wpnNumberY).ToList();

        //Get current WeaponBase or default(WeaponBase) - almost the same as null
        var currWpn = allWeapons.Where(x => x.isActiveAndEnabled).FirstOrDefault();
        _currentWeaponIndex = currWpn == default(WeaponBase) ? 1 : allWeapons.IndexOf(currWpn);
        _currentWeaponSubIndex = currWpn == default(WeaponBase) ? 0 : currWpn.wpnNumberY;
    }

    public void OnPickedUpAmmo(int ammoGiven, AmmoTypes type)
    {
        ammoReserve[type] += ammoGiven;
        if (allWeapons.Count > _currentWeaponIndex && allWeapons[_currentWeaponIndex] != null) allWeapons[_currentWeaponIndex].SetAmmoOnHUD();
    }

    public void OnPickedUpWeapon(string wpnName)
    {
        if (allWeapons.Where(x => x.gameObject.name == wpnName).Count() < 1)
        {
            //create and add prefab to player head

            var addedWeapon = GameObject.Instantiate(WeaponPrefabManager.Instance.GetWeapon(wpnName), cam.transform.position, Quaternion.identity, cam.transform);
            addedWeapon.gameObject.name = wpnName;
            addedWeapon.transform.localPosition = Vector3.zero;
            addedWeapon.transform.localEulerAngles = Vector3.zero;

            allWeapons.Add(addedWeapon);

            ExecuteChangeWeapon(allWeapons.IndexOf(addedWeapon));
        }
    }

    public void AddWeaponToList(WeaponBase wpnToAdd)
    {
        if (allWeapons == null) allWeapons = new List<WeaponBase>();

        allWeapons.Add(wpnToAdd);

        allWeapons = allWeapons.OrderBy(X => X.wpnNumberX).ThenBy(x => x.wpnNumberY).ToList();
    }

    public void RemoveWeaponFromList(WeaponBase wpnToRemove)
    {
        if (allWeapons == null) allWeapons = new List<WeaponBase>();

        allWeapons.Remove(wpnToRemove);

        allWeapons = allWeapons.Where(x => x != null).OrderBy(X => X.wpnNumberX).ThenBy(x => x.wpnNumberY).ToList();
    }

    public void OnPickedUpLife(int lifeGiven)
    {
        TakeHealing(lifeGiven);
    }

    void Update()
    {
        if (LockedByGame) return;

        CheckJump();
        CheckMovementInput();
        CheckInteract();
        CheckChangeWeapon();
    }

    void FixedUpdate()
    {
        if (LockedByGame) return;

        CheckMovement();
    }

    void CheckMovementInput()
    {
        Fow += Input.GetKey(KeyCode.W) ? _accelerationCoeficient * Time.deltaTime : -_accelerationCoeficient * Time.deltaTime;
        Back += Input.GetKey(KeyCode.S) ? _accelerationCoeficient * Time.deltaTime : -_accelerationCoeficient * Time.deltaTime;
        Left += Input.GetKey(KeyCode.A) ? _accelerationCoeficient * Time.deltaTime : -_accelerationCoeficient * Time.deltaTime;
        Right += Input.GetKey(KeyCode.D) ? _accelerationCoeficient * Time.deltaTime : -_accelerationCoeficient * Time.deltaTime;
    }

    void CheckMovement()
    {
        var dir = transform.forward * (Fow - Back) + transform.right * (Right - Left);
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
            var hits = Physics.RaycastAll(cam.transform.position, cam.transform.forward, 2, mask);
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

    public void ChangeWeaponByOutOfAmmo()
    {
        var nextIndex = _currentWeaponIndex - 1;

        if (nextIndex < 0) nextIndex = allWeapons.Count - 1;
        else if (nextIndex >= allWeapons.Count) nextIndex = 0;

        ExecuteChangeWeapon(nextIndex, true);
    }

    void CheckChangeWeapon()
    {
        var mouseWheelDelta = Input.GetAxis("Mouse ScrollWheel");

        if (mouseWheelDelta != 0 && !_changingWeapon && allWeapons.Count > 0)
        {
            var nextIndex = _currentWeaponIndex + (int)Mathf.Sign(mouseWheelDelta);

            if (nextIndex < 0) nextIndex = allWeapons.Count - 1;
            else if (nextIndex >= allWeapons.Count) nextIndex = 0;

            ExecuteChangeWeapon(nextIndex);
        }
        else
        {
            for (byte i = 0; i < _wpnKeys.Count; i++)
            {
                if (Input.GetKeyDown(_wpnKeys[i]) && !_changingWeapon && allWeapons.Count > 0)
                {
                    //Weapons ordered by sub index
                    var weaponsToChangeTo = allWeapons.Where(x => x.wpnNumberX == i).OrderBy(x => x.wpnNumberY).ToList();

                    //initialize variable
                    WeaponBase pickedWeapon = default;

                    //save current weapon in local variable so we don't iterate a lot of times in the array
                    var currentWeaponData = allWeapons[_currentWeaponIndex];

                    if (weaponsToChangeTo.Count == 1)
                    {
                        //if it's our current weapon we do nothing
                        if (currentWeaponData.wpnNumberX == i) return;
                        else pickedWeapon = weaponsToChangeTo.FirstOrDefault();
                    }
                    else if (weaponsToChangeTo.Count > 1)
                    {
                        //Here we will select a weapon based on the sub index
                        if (currentWeaponData.wpnNumberX == i)
                        {
                            //If we have the last weapon on the category we get the first one
                            if (currentWeaponData.wpnNumberY == weaponsToChangeTo.Count - 1) pickedWeapon = weaponsToChangeTo.FirstOrDefault();
                            else //we get the next on the list, as it is ordered by wpnNumberY
                            {
                                var actualIndex = weaponsToChangeTo.IndexOf(currentWeaponData);
                                pickedWeapon = weaponsToChangeTo[actualIndex + 1];
                            }
                        }
                    }

                    if (pickedWeapon != default) ExecuteChangeWeapon(allWeapons.IndexOf(pickedWeapon));
                    return;
                }
            }
        }
    }

    public void ExecuteChangeWeapon(int indx, bool fastChange = false)
    {
        _changingWeapon = true;
        if (fastChange) StartCoroutine(ChangeWeaponFast(indx));
        else StartCoroutine(ChangeWeapon(indx));
    }

    IEnumerator ChangeWeapon(int indx)
    {
        if (allWeapons.Count > _currentWeaponIndex && allWeapons[_currentWeaponIndex] != null)
        {
            allWeapons[_currentWeaponIndex].ChangeWeapon();

            yield return new WaitUntil(() => !allWeapons[_currentWeaponIndex].Drawn);
        }

        _currentWeaponIndex = indx;

        allWeapons[_currentWeaponIndex].gameObject.SetActive(true);

        yield return new WaitUntil(() => allWeapons[_currentWeaponIndex].Drawn);

        _changingWeapon = false;
    }

    IEnumerator ChangeWeaponFast(int indx)
    {
        allWeapons[_currentWeaponIndex].gameObject.SetActive(false);
        if (allWeapons.Count <= 1)
        {
            _currentWeaponIndex = 0;
            RemoveWeaponFromList(allWeapons[_currentWeaponIndex]);
            _changingWeapon = false;
            yield break;
        }

        _currentWeaponIndex = indx;

        allWeapons[_currentWeaponIndex].gameObject.SetActive(true);

        yield return new WaitUntil(() => allWeapons[_currentWeaponIndex].Drawn);

        _changingWeapon = false;

        allWeapons = allWeapons.Where(x => x != null).ToList();
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

        HUDController.Instance.SetKey(key);
    }

    public bool GetIfKeyInInventory(KeysForDoors key)
    {
        if (!_keysOnInventory.ContainsKey(key))
        {
            _keysOnInventory.Add(key, false);
        }

        return _keysOnInventory[key];
    }

    public void TakeDamage(int damage, string damageType)
    {
        CurrentHp -= damage;
        AddRecoil(.3f, 5f);
        HUDController.Instance.OnDamage();

        if (CurrentHp < 1)
        {
            Die();
            CurrentHp = 0;
        }

        CurrentHp = Mathf.Round(CurrentHp);

        UpdateHP();
    }

    public void OnRespawn(float hp)
    {
        GetComponent<Collider>().enabled = true;
        _rb.isKinematic = false;
        _rb.useGravity = true;
        Head.OnRespawn();
        CurrentHp = hp;
        UpdateHP();
        LockedByGame = false;
        allWeapons[_currentWeaponIndex].gameObject.SetActive(true);
    }

    public void TakeHealing(int healing)
    {
        CurrentHp += healing;
        UpdateHP();
    }

    void UpdateHP()
    {
        HUDController.Instance.SetHealth(CurrentHp.ToString());
    }

    void Die()
    {
        GetComponent<Collider>().enabled = false;
        _rb.isKinematic = true;
        _rb.useGravity = false;
        allWeapons[_currentWeaponIndex].gameObject.SetActive(false);

        _head.OnDeath();
        LockedByGame = true;
        HUDController.Instance.OnDeath();
        CheckpointManager.Instance.OnPlayerDeath(1);
        //UnityEngine.SceneManagement.SceneManager.LoadScene("DeadPlayer");
        //gameObject.SetActive(false);
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

