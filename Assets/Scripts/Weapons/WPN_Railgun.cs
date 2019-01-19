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

    protected override void Draw()
    {
        base.Draw();
        if (GetAmmo() == 0) ForceDrawReload();
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

    void ForceDrawReload()
    {
        StartCoroutine(WaitForDrawEnd());
    }

    IEnumerator WaitForDrawEnd()
    {
        //wait for anim

        var smb = _an.GetBehaviour<SMB_DrawState>();

        yield return new WaitUntil(() => smb.finishedAnim);

        StartCoroutine(base.ReloadWeapon());
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
}

[Serializable]
public class RailgunRecoilStats
{
    public float recoilDelay;
    public float delayTicks;
}

