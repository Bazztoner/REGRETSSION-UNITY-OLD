using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WPN_RailCannon : WeaponBase
{
    public float reloadTime;

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
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            if (_canReload()) Reload();
        }
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
        var rndStyle = Random.Range(0, 2);
        var idleStyle = Random.Range(0, 2);
        _an.SetInteger("idleStyle", idleStyle);

        var stateName = "shoot" + rndStyle;
        _an.CrossFadeInFixedTime(stateName, .1f);

        yield return new WaitForEndOfFrame();

        //shoot
        AddRecoil();

        yield return new WaitForSeconds(shootCooldown - Time.deltaTime);

        _shooting = false;

        if (GetBulletsInMag() <= 0)
        {
            Reload();
        }
    }

    protected override IEnumerator ReloadWeapon()
    {
        //wait for anim

        _reloading = true;

        _an.CrossFadeInFixedTime("reload", .1f);

        yield return new WaitForSeconds(reloadTime);

        _reloading = false;

        var bulletsToReload = GetReserveAmmo() >= magSize ? magSize : GetReserveAmmo();
        OnReload(bulletsToReload);
    }

    void ForceDrawReload()
    {
        StartCoroutine(WaitForDrawEnd());
    }

    void OnReload(int bulletsToReload)
    {
        SetBulletsInMag(bulletsToReload);
        UpdateReserveAmmo(-_currentBulletsInMag);

        SetAmmoOnHUD();
    }

    void OnShoot()
    {
        SetBulletsInMag(-1);
        SetAmmoOnHUD();
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
}
