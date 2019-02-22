using System.Collections;
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

    HashSet<string> _weaponAvailability;
    CameraShake _camShake;
    CameraController _camController;
    PlayerHead _head;
    Rigidbody _rb;
    [SerializeField] int _currentWpn;

    List<KeyCode> _wpnKeys;

    bool _changingWeapon = false, _grounded = false;
    public Camera cam;

    public float movementSpeed, jumpForce;
    public int maxHp;
    float _currentHp;

    float _fow, _back, _left, _right;
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
        for (byte i = 1; i <= 9; i++)
        {
            //Alpha 0 is KeyCode 48.
            _wpnKeys.Add((KeyCode)Enum.ToObject(typeof(KeyCode), i + 48));
        }

        WeaponControlUtilities.Initialize();
        allWeapons = GetComponentsInChildren<WeaponBase>(true).OrderBy(X => X.wpnNumber).ToList();
        _currentWpn = allWeapons.Where(x => x.isActiveAndEnabled).First().wpnNumber;
        _currentWpn--;

        _weaponAvailability = new HashSet<string>()
        {
            allWeapons[0].gameObject.name,
            allWeapons[1].gameObject.name
        };
    }

    public void OnPickedUpAmmo(int ammoGiven, AmmoTypes type)
    {
        ammoReserve[type] += ammoGiven;
        allWeapons[_currentWpn].SetAmmoOnHUD();
    }

    public void OnPickedUpWeapon(string name)
    {
        if (!_weaponAvailability.Contains(name))
        {
            _weaponAvailability.Add(name);
            ExecuteChangeWeapon(allWeapons.Where(x => x.gameObject.name == name).FirstOrDefault().wpnNumber - 1);
        }
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
        _fow = Input.GetKey(KeyCode.W) ? 1 : 0;
        _back = Input.GetKey(KeyCode.S) ? -1 : 0;
        _left = Input.GetKey(KeyCode.A) ? -1 : 0;
        _right = Input.GetKey(KeyCode.D) ? 1 : 0;
    }

    void CheckMovement()
    {
        var dir = transform.forward * (_fow + _back) + transform.right * (_left + _right);
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

    void CheckChangeWeapon()
    {
        for (byte i = 0; i < _wpnKeys.Count; i++)
        {
            if (i != _currentWpn && _weaponAvailability.Contains(allWeapons[i].gameObject.name))
            {
                if (Input.GetKeyDown(_wpnKeys[i]) && !_changingWeapon)
                {
                    ExecuteChangeWeapon(i);
                    return;
                }
            }
        }
    }

    public void ExecuteChangeWeapon(int indx)
    {
        _changingWeapon = true;
        StartCoroutine(ChangeWeapon(indx));
    }

    IEnumerator ChangeWeapon(int indx)
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
        allWeapons[_currentWpn].gameObject.SetActive(true);
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
        allWeapons[_currentWpn].gameObject.SetActive(false);

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

