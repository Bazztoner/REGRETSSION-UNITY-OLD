using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class WPN_Railgun : WeaponBase
{
    [SerializeField]
    public RailgunRecoilStats railgunRecoilStats;

    public int maxTargets;
    public float reloadTime;

    protected override void CheckInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_canShoot()) Shoot();
        }
    }

    protected override void InitializeSoundModule()
    {
        _sound = GetComponent<RailgunSoundModule>();
    }

    protected override void Draw()
    {
        base.Draw();
        if (GetBulletsInMag() == 0) ForceDrawReload();
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
        StartCoroutine(ProgressiveRecoil());

        yield return new WaitForSeconds(shootCooldown - Time.deltaTime);

        Reload();
    }

    IEnumerator ProgressiveRecoil()
    {
        var tickDuration = railgunRecoilStats.recoilDelay / railgunRecoilStats.delayTicks;

        for (int i = 0; i < railgunRecoilStats.delayTicks; i++)
        {
            yield return new WaitForSeconds(tickDuration);
            AddRecoil();
        }

    }

    protected override void AddRecoil()
    {
        _owner.AddRecoil(recoilStats.recoveryTime, recoilStats.amount/ railgunRecoilStats.delayTicks);
    }

    public override void Reload()
    {
        if (GetReserveAmmo() < 1)
        {
            _an.CrossFadeInFixedTime("idle", .1f);
            return;
        }
        StartCoroutine(ReloadWeapon());
    }

    protected override IEnumerator ReloadWeapon()
    {
        //wait for anim

        _reloading = true;
        _an.CrossFadeInFixedTime("reload", .1f);

        yield return new WaitForSeconds(reloadTime);

        _reloading = false;
        _shooting = false;
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

    protected override void ManageProjectile()
    {
        //Owner.ApplyShake(ShakeDuration, ShakeIntensity);

        var dir = (_owner.cam.transform.forward + _muzzle.transform.forward).normalized;
        dir.Normalize();

        var b = new HitscanRay(_muzzle.transform.position, dir.normalized, damage, maxTargets);
        var particleID = SimpleParticleSpawner.ParticleID.BULLET;

        var particle = SimpleParticleSpawner.Instance.particles[particleID].GetComponentInChildren<ParticleSystem>();
        var speed = particle.main.startSpeed.constant * particle.main.simulationSpeed;
        var lifeTime = b.objDist / speed;
        SimpleParticleSpawner.Instance.SpawnParticle(particle.gameObject, _muzzle.transform.position, dir.normalized, lifeTime);

        var muzzleFlashID = SimpleParticleSpawner.ParticleID.MUZZLEFLASH;
        var muzzleFlashParticle = SimpleParticleSpawner.Instance.particles[muzzleFlashID].GetComponentInChildren<ParticleSystem>();

        SimpleParticleSpawner.Instance.SpawnParticle(muzzleFlashParticle.gameObject, _muzzle.transform.position, dir.normalized, _muzzle.transform);
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

[Serializable]
public class RailgunRecoilStats
{
    public float recoilDelay;
    public float delayTicks;
}

