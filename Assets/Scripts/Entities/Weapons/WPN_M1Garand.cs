using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WPN_M1Garand : WeaponBase
{
    int _bulletsInMagHash = Animator.StringToHash("bullets");

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

    protected override void OnEnable()
    {
        _an.SetInteger(_bulletsInMagHash, _currentBulletsInMag);
        base.OnEnable();
    }

    protected override void InitializeSoundModule()
    {
        //TODO make m1garand sound module
        _sound = GetComponent<DeagleSoundModule>();
    }

    protected override void Draw()
    {
        StartCoroutine(DrawWeapon());
        SetAmmoOnHUD();
        if (GetBulletsInMag() == 0) ForceDrawReload();
    }

    protected override IEnumerator DrawWeapon()
    {
        //wait for anim
        _sound.OnDraw();

        var stateName = GetBulletsInMag() >= 1 ? "draw" : "draw_empty";

        _an.CrossFadeInFixedTime(stateName, .1f);

        var smb = _an.GetBehaviour<SMB_DrawState>();

        yield return new WaitUntil(() => smb.finishedAnim);

        _drawn = true;
    }

    protected override void CheckInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_canShoot()) Shoot();
        }
    }

    void OnReload(int bulletsToReload)
    {
        SetBulletsInMag(bulletsToReload);
        UpdateReserveAmmo(-bulletsToReload);

        SetAmmoOnHUD();
    }

    void OnShoot()
    {
        SetBulletsInMag(-1);
        _an.SetInteger(_bulletsInMagHash, GetBulletsInMag());
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

        var shootStates = new string[] { "shoot1", "shoot2", "shoot3" };

        var stateName = GetBulletsInMag() >= 1 ? shootStates[UnityEngine.Random.Range(0,shootStates.Length)] : "shoot_last";

        _an.CrossFadeInFixedTime(stateName, .1f);

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

    void ForceDrawReload()
    {
        StartCoroutine(WaitForDrawEnd());
    }

    IEnumerator WaitForDrawEnd()
    {
        //wait for anim

        var smb = _an.GetBehaviour<SMB_DrawState>();

        yield return new WaitUntil(() => smb.finishedAnim);

        Reload();
    }

    public override void Reload()
    {
        if (!_canReload()) return;
        base.Reload();
        var bulletsToReload = GetReserveAmmo() >= magSize ? magSize : GetReserveAmmo();
        var diff = bulletsToReload - _currentBulletsInMag;
        OnReload(diff);
    }

    public override void SetAmmoOnHUD()
    {
        HUDController.Instance.SetAmmo(_currentBulletsInMag + "/" + GetReserveAmmo());
    }
}
