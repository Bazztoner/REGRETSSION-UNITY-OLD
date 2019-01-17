using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WPN_RocketLauncher : WeaponBase
{
    public float reloadTime;

    protected override void CheckInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_canShoot()) Shoot();
        }
    }

    protected override int GetAmmo()
    {
        return _ammo <= 0 ? 0 : _ammo;
    }

    protected override void SetAmmo(int ammo)
    {
        _ammo = ammo <= 0 ? 0 : ammo >= maxAmmo ? maxAmmo : ammo;
    }

    protected override void InitializeConditions()
    {
        _canShoot = () => !_shooting && !_reloading && !_holstering && GetAmmo() > 0 && _drawn;
        _canReload = () => !_shooting && !_reloading && !_holstering && GetAmmo() < maxAmmo && _drawn;
    }

    protected override void Shoot()
    {
        StartCoroutine(ShootHandler());
    }

    IEnumerator ShootHandler()
    {
        UpdateAmmo(-1);
        _shooting = true;

        _an.CrossFadeInFixedTime("shoot", .1f);

        yield return new WaitForEndOfFrame();

        //shoot

        yield return new WaitForSeconds(shootCooldown - Time.deltaTime);

        Reload();
    }

    public override void Reload()
    {
        StartCoroutine(ReloadWeapon());
    }

    protected override IEnumerator ReloadWeapon()
    {
        //wait for anim

        _reloading = true;

        yield return new WaitForSeconds(reloadTime);

        UpdateAmmo(maxAmmo);

        _reloading = false;
        _shooting = false;
    }
}
