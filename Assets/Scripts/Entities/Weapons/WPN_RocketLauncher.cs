using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WPN_RocketLauncher : WeaponBase
{
    public float reloadTime;
    public GameObject rocketPrefab;

    protected override void Draw()
    {
        base.Draw();
    }

    protected override void CheckInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_canShoot()) Shoot();
        }
    }

    protected override int GetBulletsInMag()
    {
        return _currentBulletsInMag <= 0 ? 0 : _currentBulletsInMag;
    }

    protected override void InitializeConditions()
    {
        _canShoot = () => !_shooting && !_reloading && !_holstering && GetReserveAmmo() > 0 && _drawn;
    }

    protected override void Shoot()
    {
        StartCoroutine(ShootHandler());
    }

    IEnumerator ShootHandler()
    {
        UpdateReserveAmmo(-1);
        SetAmmoOnHUD();
        _shooting = true;

        _an.CrossFadeInFixedTime("shoot", .1f);

        yield return new WaitForEndOfFrame();

        ManageProjectile();
        AddRecoil();

        yield return new WaitForSeconds(shootCooldown - Time.deltaTime);

        Reload();
    }

    protected override void ManageProjectile()
    {

        var dir = (_owner.cam.transform.forward + _muzzle.transform.forward).normalized;
        dir.Normalize();

        var b = Instantiate(rocketPrefab, _muzzle.transform.position, Quaternion.identity);
        b.transform.forward = dir.normalized;

        var muzzleFlashID = SimpleParticleSpawner.ParticleID.MUZZLEFLASH;
        var muzzleFlashParticle = SimpleParticleSpawner.Instance.particles[muzzleFlashID].GetComponentInChildren<ParticleSystem>();

        SimpleParticleSpawner.Instance.SpawnParticle(muzzleFlashParticle.gameObject, _muzzle.transform.position, dir.normalized, _muzzle.transform);
    }

    public override void Reload()
    {
        if (GetReserveAmmo() < 1) return;
        StartCoroutine(ReloadWeapon());
    }

    protected override IEnumerator ReloadWeapon()
    {
        //wait for anim
        _an.CrossFadeInFixedTime("reload", .1f);
        _reloading = true;

        yield return new WaitForSeconds(reloadTime);

        _reloading = false;
        _shooting = false;
    }

    public override void SetAmmoOnHUD()
    {
        HUDController.Instance.SetAmmo(GetReserveAmmo().ToString());
    }

    protected override int GetReserveAmmo()
    {
        return _owner.ammoReserve[ammoType];
    }

    protected override void SetBulletsInMag(int bullets, bool overrideBullets = false)
    {
        //
    }

    protected override void UpdateReserveAmmo(int ammo)
    {
        _owner.ammoReserve[ammoType] += ammo;

        _owner.ammoReserve[ammoType] = Mathf.Clamp(_owner.ammoReserve[ammoType], 0, _owner.MaxAmmoReserve[(int)ammoType]);
    }
}
