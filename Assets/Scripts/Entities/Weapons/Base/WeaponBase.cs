using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public abstract class WeaponBase : MonoBehaviour
{
    public AmmoTypes ammoType;
    protected WeaponSoundModuleBase _sound;

    protected Animator _an;
    protected readonly int _shootHash = Animator.StringToHash("shoot");
    protected readonly int _holsterHash = Animator.StringToHash("holster");
    protected readonly int _reloadHash = Animator.StringToHash("reload");
    protected PlayerController _owner;
    protected WeaponMuzzle _muzzle;

    protected bool _drawn = false;
    protected bool _holstering = false;
    protected bool _shooting = false;
    protected bool _reloading = false;

    [SerializeField]
    public WeaponRecoilStats recoilStats;

    public int magSize;
    public float shootCooldown;
    [SerializeField] protected int _currentBulletsInMag;

    //new Stuff
    public byte wpnNumberX;
    public byte wpnNumberY;


    protected abstract int GetBulletsInMag();
    protected abstract int GetReserveAmmo();
    protected abstract void SetBulletsInMag(int bullets, bool overrideBullets = false);
    protected abstract void UpdateReserveAmmo(int ammo);

    //"∞"

    public float damage;
    public int pellets;

    protected Func<bool> _canShoot;
    protected Func<bool> _canReload;

    public bool Drawn { get => _drawn; }
    public bool Holstering { get => _holstering; }

    protected virtual void Awake()
    {
        _an = GetComponent<Animator>();
        _owner = GetComponentInParent<PlayerController>();
        InitializeSoundModule();
    }

    protected abstract void InitializeSoundModule();

    protected virtual void Start()
    {
        _muzzle = GetComponentInChildren<WeaponMuzzle>();
        InitializeConditions();
        SetBulletsInMag(magSize);
    }

    protected abstract void InitializeConditions();

    protected virtual void OnEnable()
    {
        _an.SetBool(_holsterHash, false);
        _an.SetBool(_reloadHash, false);
        _an.SetBool(_shootHash, false);

        _an.Play("Entry");

        _drawn = false;
        Draw();
    }

    protected virtual void OnDisable()
    {
        _drawn = false;
        _holstering = false;
        _reloading = false;
        _shooting = false;
    }

    protected virtual void Update()
    {
        CheckInput();
        //ManageRecoil();
    }

    protected abstract void CheckInput();

    protected abstract void Shoot();

    protected virtual void Draw()
    {
        //no hash needed
        StartCoroutine(DrawWeapon());
        SetAmmoOnHUD();
    }

    public abstract void SetAmmoOnHUD();


    protected virtual IEnumerator DrawWeapon()
    {
        //wait for anim
        _sound.OnDraw();

        var smb = _an.GetBehaviour<SMB_DrawState>();

        yield return new WaitUntil(() => smb.finishedAnim);

        _drawn = true;
    }

    public virtual void ChangeWeapon()
    {
        //_an.SetBool(_holsterHash, true);
        if (gameObject.activeInHierarchy) StartCoroutine(HolsterWeapon());
    }

    protected IEnumerator HolsterWeapon()
    {
        //wait for anim

        var sm = _an.GetCurrentAnimatorStateInfo(0);
        if (sm.IsName("reload"))
        {
            var reloadState = _an.GetBehaviour<SMB_ReloadState>();
            if (reloadState != null) reloadState.CancelReload();
        }

        _holstering = true;

        _an.CrossFadeInFixedTime("holster", .1f);

        var smb = _an.GetBehaviour<SMB_HolsterState>();

        yield return new WaitUntil(() => smb.finishedAnim);

        _holstering = false;
        _drawn = false;

        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    public virtual void Reload()
    {
        StartCoroutine(ReloadWeapon());
    }

    protected virtual IEnumerator ReloadWeapon()
    {
        //wait for anim

        _reloading = true;

        _an.CrossFadeInFixedTime("reload", .1f);

        var smb = _an.GetBehaviour<SMB_ReloadState>();

        while(true)
        {
            yield return new WaitForEndOfFrame();

            if (smb.finishedAnim || smb.reloadCancelled)
            {
                break;
            }
        }
        _reloading = false;

        //override and add update ammo
    }

    protected virtual void ManageProjectile()
    {
        var dir = (_owner.cam.transform.forward + _muzzle.transform.forward).normalized;
        dir.Normalize();

        var muzzleFlashID = ParticleIDs.MUZZLE_FLASH_GENERIC;
        var muzzleFlashParticle = SimpleParticleSpawner.Instance.GetParticleByID(muzzleFlashID);
        SimpleParticleSpawner.Instance.SpawnParticle(muzzleFlashParticle, _muzzle.transform.position, dir.normalized, _muzzle.transform);

        var b = new HitscanBullet(_muzzle.transform.position, dir.normalized, damage, pellets);
        var particleID = ParticleIDs.BULLET_TRACER_GENERIC;

        var particle = SimpleParticleSpawner.Instance.GetParticleByID(particleID).GetComponentInChildren<ParticleSystem>();
        var speed = particle.main.startSpeed.constant * particle.main.simulationSpeed;
        var lifeTime = b.objDist / speed;
        SimpleParticleSpawner.Instance.SpawnParticle(particle.gameObject, _muzzle.transform.position, dir.normalized, lifeTime);

    }

    protected virtual void AddRecoil()
    {
        _owner.AddRecoil(recoilStats.recoveryTime, recoilStats.amount);
    }

    protected virtual void ManageRecoil()
    {
        recoilStats.currentZPosition = Mathf.SmoothDamp(recoilStats.currentZPosition, 0, ref recoilStats.currentZPosVelocity, recoilStats.recoveryTime);

        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, recoilStats.currentZPosition);
    }
}

public enum AmmoTypes
{
    Bullets,
    Shells,
    Rockets,
    Cells,
    Energy,
    Cores,
    GENERIC
}

[Serializable]
public struct WeaponRecoilStats
{
    [SerializeField]
    public float amount, recoveryTime;

    [HideInInspector]
    public float currentZPosition, currentZPosVelocity;

}
