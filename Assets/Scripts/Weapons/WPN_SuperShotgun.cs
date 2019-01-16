using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WPN_SuperShotgun : WeaponBase
{
    protected override void CheckInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_canShoot()) Shoot();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            if (_canReload()) Reload();
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

        yield return new WaitForSeconds(shootCooldown);

        _shooting = false;
    }
}
