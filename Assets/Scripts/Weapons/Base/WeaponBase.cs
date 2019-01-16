using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public abstract class WeaponBase : MonoBehaviour
{
    protected Animator _an;
    protected readonly int _shootHash = Animator.StringToHash("shoot");
    protected readonly int _holsterHash = Animator.StringToHash("holster");
    protected readonly int _reloadHash = Animator.StringToHash("reload");

    protected bool _drawn = false;
    protected bool _holstering = false;
    protected bool _shooting = false;
    protected bool _reloading = false;

    public int maxAmmo;
    public float shootCooldown;
    protected int _ammo;
    protected abstract int GetAmmo();
    protected abstract void SetAmmo(int ammo);

    protected Func<bool> _canShoot;
    protected Func<bool> _canReload;

    protected virtual void Awake()
    {
        _an = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        InitializeConditions();
        SetAmmo(maxAmmo);
    }

    protected abstract void InitializeConditions();

    protected virtual void OnEnable()
    {
        _an.SetBool(_holsterHash, false);
        _an.SetBool(_reloadHash, false);
        _an.SetBool(_shootHash, false);

        _an.Play("Entry");

        Draw();
    }

    protected virtual void OnDisable()
    {
        _drawn = false;
        _holstering = false;
    }

    protected virtual void Update()
    {
        CheckInput();
    }

    protected abstract void CheckInput();

    protected abstract void Shoot();

    protected virtual void Draw()
    {
        //no hash needed
        StartCoroutine(DrawWeapon());
    }

    protected IEnumerator DrawWeapon()
    {
        //wait for anim

        var smb = _an.GetBehaviour<SMB_DrawState>();

        yield return new WaitUntil(() => smb.finishedAnim);

        _drawn = true;
    }

    protected virtual void ChangeWeapon()
    {
        //_an.SetBool(_holsterHash, true);
        StartCoroutine(HolsterWeapon());
    }

    protected IEnumerator HolsterWeapon()
    {
        //wait for anim

        _holstering = true;

        _an.CrossFadeInFixedTime("holster", .1f);

        var smb = _an.GetBehaviour<SMB_HolsterState>();

        yield return new WaitUntil(() => smb.finishedAnim);

        _holstering = false;
    }

    protected virtual void Reload()
    {
        StartCoroutine(ReloadWeapon());
    }

    protected virtual IEnumerator ReloadWeapon()
    {
        //wait for anim

        _reloading = true;

        _an.CrossFadeInFixedTime("reload", .1f);

        var smb = _an.GetBehaviour<SMB_ReloadState>();

        yield return new WaitUntil(() => smb.finishedAnim);

        UpdateAmmo(maxAmmo);

        _reloading = false;
    }

    protected virtual void UpdateAmmo(int ammo)
    {
        SetAmmo(_ammo + ammo);
    }
}

//Hay cosas de Unity que me enferman

/*IEnumerator OldShootHandler()
   {
       UpdateAmmo(-1);
       _shooting = true;

       _an.SetBool(_shootHash, true);

       var smb = _an.GetBehaviour<SMB_ShootState>();

       yield return new WaitUntil(() => smb.finishedAnim);

       _shooting = false;
       _an.SetBool(_shootHash, _shooting);
   }*/
