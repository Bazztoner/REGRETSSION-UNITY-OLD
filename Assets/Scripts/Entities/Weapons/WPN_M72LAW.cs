using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WPN_M72LAW : WeaponBase
{
    public GameObject rocketPrefab;
    protected override void InitializeSoundModule()
    {
        _sound = GetComponent<M72LAWSoundModule>();
    }

    protected override void CheckInput()
    {
        if (Input.GetMouseButtonDown(0) && _canShoot())
        {
            Shoot();
        }
    }

    protected override void InitializeConditions()
    {
        _canShoot = () => !_shooting && !_reloading && !_holstering && _drawn;
    }

    protected override void Shoot()
    {
        StartCoroutine(ShootHandler());
    }

    IEnumerator ShootHandler()
    {
        _shooting = true;

        _an.CrossFadeInFixedTime("shoot", .1f);

        yield return new WaitForEndOfFrame();

        _sound.OnShoot();
        ManageProjectile();
        AddRecoil();

        yield return new WaitForSeconds(shootCooldown - Time.deltaTime);

        Reload();
    }

    protected override IEnumerator ReloadWeapon()
    {
        //wait for anim

        _reloading = true;

        _an.CrossFadeInFixedTime("reload", .1f);

        var smb = _an.GetBehaviour<SMB_ReloadState>();

        while (true)
        {
            yield return new WaitForEndOfFrame();

            if (smb.finishedAnim || smb.reloadCancelled)
            {
                break;
            }
        }
        _reloading = false;

        _owner.ChangeWeaponByOutOfAmmo();

        Destroy(this.gameObject, .1f);
    }

    protected override void ManageProjectile()
    {
        var dir = (_owner.cam.transform.forward + _muzzle.transform.forward).normalized;
        dir.Normalize();

        var b = Instantiate(rocketPrefab, _muzzle.transform.position, Quaternion.identity);
        b.transform.forward = dir.normalized;

        var muzzleFlashID = ParticleIDs.MUZZLE_FLASH_GENERIC;
        var muzzleFlashParticle = SimpleParticleSpawner.Instance.GetParticleByID(muzzleFlashID).GetComponentInChildren<ParticleSystem>();

        SimpleParticleSpawner.Instance.SpawnParticle(muzzleFlashParticle.gameObject, _muzzle.transform.position, dir.normalized, _muzzle.transform);
    }

    public override void SetAmmoOnHUD()
    {
        HUDController.Instance.SetAmmo("1");
    }

    protected override int GetReserveAmmo()
    {
        return 0;
    }

    protected override void SetBulletsInMag(int bullets, bool overrideBullets = false)
    {
        //
    }

    protected override void UpdateReserveAmmo(int ammo)
    {
        //
    }

    protected override int GetBulletsInMag()
    {
        return 0;
    }
}
