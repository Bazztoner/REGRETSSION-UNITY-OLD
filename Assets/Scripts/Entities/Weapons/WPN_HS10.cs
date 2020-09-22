﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WPN_HS10 : WeaponBase
{
    public float dispersionConeRadius;
    int _bulletsInMagHash = Animator.StringToHash("bullets");

    #region HS10 Reload variables
    bool _cancelReload;

    #endregion

    protected override void OnEnable()
    {
        _an.SetInteger(_bulletsInMagHash, _currentBulletsInMag);
        _reloading = false;
        _cancelReload = false;
        base.OnEnable();
    }

    protected override void Draw()
    {
        base.Draw();
        if (GetBulletsInMag() == 0) ForceDrawReload();
    }

    protected override void CheckInput()
    {
        if (Input.GetMouseButton(0))
        {
            if (_canShoot()) Shoot();
            else if (_reloading) _cancelReload = true;
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            if (_canReload()) Reload();
        }
    }

    protected override void InitializeSoundModule()
    {
        _sound = GetComponent<HS10SoundModule>();
    }

    void OnShoot()
    {
        SetBulletsInMag(-1);
        SetAmmoOnHUD();
        _sound.OnShoot();
    }

    protected override void InitializeConditions()
    {
        _canShoot = () => !_shooting && !_reloading && !_holstering && GetBulletsInMag() > 0 && _drawn;
        _canReload = () => !_shooting && !_reloading && !_holstering && GetBulletsInMag() < magSize && GetReserveAmmo() > 0 && _drawn;
    }

    protected override void Shoot()
    {
        StartCoroutine(ShootHandler());
    }

    IEnumerator ShootHandler()
    {
        if (!_canShoot()) yield break;

        OnShoot();
        _shooting = true;

        _an.CrossFadeInFixedTime("shoot", .1f);

        yield return new WaitForEndOfFrame();

        ManageProjectile();
        AddRecoil();

        yield return new WaitForSeconds(shootCooldown - Time.deltaTime);

        _shooting = false;

        if (GetBulletsInMag() <= 0)
        {
            Reload();
        }
    }

    protected override void ManageProjectile()
    {
        for (int i = 0; i < pellets; i++)
        {
            var dispersionFactor = 1 / dispersionConeRadius;

            float xSpread = Random.Range(-dispersionFactor, dispersionFactor);
            float ySpread = Random.Range(-(dispersionFactor / 2), dispersionFactor / 2);

            var dirToCrosshair = (_owner.cam.transform.forward + _muzzle.transform.forward).normalized;

            var dir = (dirToCrosshair + new Vector3(xSpread, ySpread, 0));

            var b = new HitscanBullet(_muzzle.transform.position, dir.normalized, damage, pellets);

            var bulletParticleID = ParticleIDs.BULLET_TRACER_GENERIC;

            var bulletParticle = SimpleParticleSpawner.Instance.GetParticleByID(bulletParticleID).GetComponentInChildren<ParticleSystem>();
            var speed = bulletParticle.main.startSpeed.constant * bulletParticle.main.simulationSpeed;
            var lifeTime = b.objDist / speed;

            SimpleParticleSpawner.Instance.SpawnParticle(bulletParticle.gameObject, _muzzle.transform.position, dir.normalized, lifeTime);

        }

        var muzzleFlashID = ParticleIDs.MUZZLE_FLASH_GENERIC;
        var muzzleFlashParticle = SimpleParticleSpawner.Instance.GetParticleByID(muzzleFlashID).GetComponentInChildren<ParticleSystem>();

        var muzzleDir = (_owner.cam.transform.forward + _muzzle.transform.forward).normalized;
        muzzleDir.Normalize();

        SimpleParticleSpawner.Instance.SpawnParticle(muzzleFlashParticle.gameObject, _muzzle.transform.position, muzzleDir.normalized, _muzzle.transform);
    }

    void ForceDrawReload()
    {
        StartCoroutine(WaitForDrawEnd());
    }

    public override void Reload()
    {
        if (GetReserveAmmo() < 1) return;
        base.Reload();
    }
    void OnReload(int bulletsToReload)
    {
        SetBulletsInMag(bulletsToReload);
        UpdateReserveAmmo(-bulletsToReload);

        SetAmmoOnHUD();
    }
    protected override IEnumerator ReloadWeapon()
    {
        _an.ResetTrigger("stopReload");
        _reloading = true;
        _cancelReload = false;

        _an.CrossFadeInFixedTime("reload_intro", .1f);

        SMB_ReloadState smb = null;

        while (true)
        {
            yield return new WaitForEndOfFrame();

            if (_currentBulletsInMag >= magSize || _cancelReload)
            {
                _an.SetTrigger("stopReload");
                yield return new WaitForEndOfFrame();
                smb = _an.GetBehaviour<SMB_ReloadState>();
            }

            if (smb != null) if (smb.finishedAnim || smb.reloadCancelled) break;
        }
        _reloading = false;
        _cancelReload = false;

        //override and add update ammo
    }

    public void OnInsertShell()
    {
        OnReload(1);
    }

    IEnumerator WaitForDrawEnd()
    {
        //wait for anim

        var smb = _an.GetBehaviour<SMB_DrawState>();

        yield return new WaitUntil(() => smb.finishedAnim);

        Reload();
    }

    public override void SetAmmoOnHUD()
    {
        HUDController.Instance.SetAmmo(_currentBulletsInMag + "/" + GetReserveAmmo());
    }

    protected override int GetBulletsInMag()
    {
        return _currentBulletsInMag;
    }

    protected override int GetReserveAmmo()
    {
        return _owner.ammoReserve[ammoType];
    }

    protected override void SetBulletsInMag(int bullets, bool overrideBullets = false)
    {
        if (overrideBullets)
        {
            _currentBulletsInMag = bullets;
        }
        else
        {
            _currentBulletsInMag += bullets;
            _currentBulletsInMag = Mathf.Clamp(_currentBulletsInMag, 0, magSize);
        }
    }

    protected override void UpdateReserveAmmo(int ammo)
    {
        _owner.ammoReserve[ammoType] += ammo;

        _owner.ammoReserve[ammoType] = Mathf.Clamp(_owner.ammoReserve[ammoType], 0, _owner.MaxAmmoReserve[(int)ammoType]);
    }
}
