using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WPN_Shockroach : WeaponBase
{
    public float initialChargeDuration;

    public float ammoPerSecond;
    public float damagePerSecond;
    public float ticks;
    float _tickDuration;
    bool _keyDown = false;

    [Header("Beam")]
    public float xSize;
    public float ySize;
    public float zSize;


    public float ReloadByTick
    {
        get { return ammoPerSecond / ticks; }
    }

    Vector3 BeamSize
    {
        get => new Vector3(xSize, ySize, zSize);
    }

    protected override int GetBulletsInMag()
    {
        return _currentBulletsInMag <= 0 ? 0 : _currentBulletsInMag;
    }

    protected override void InitializeSoundModule()
    {
        _sound = GetComponent<ShockroachSoundModule>();
    }

    protected override void InitializeConditions()
    {
        _canShoot = () => !_shooting && !_reloading && !_holstering && GetReserveAmmo() > 0 && _drawn;
    }

    protected override void Start()
    {
        base.Start();
        _tickDuration = 1 / ticks;
        StartCoroutine(Recharge());
    }


    protected override void CheckInput()
    {
        if (Input.GetMouseButton(0))
        {
            if (_canShoot())
            {
                Shoot();
            }
        }
        else _keyDown = false;
    }

    /// <summary>
    /// In ferm of shit
    /// </summary>
    /// <returns></returns>
    IEnumerator Recharge()
    {
        var waitASecond = new WaitForSeconds(1);
        var rechargeCondition = new WaitUntil(() => _currentBulletsInMag < magSize && !_keyDown);

        while ("Nisman" != "Vivo")
        {
            yield return rechargeCondition;

            UpdateReserveAmmo(Mathf.RoundToInt(ammoPerSecond));

            yield return waitASecond;
        }
    }

    protected override void Shoot()
    {
        if (!_keyDown) StartCoroutine(ShootHandler());
    }

    IEnumerator ShootHandler()
    {
        _shooting = true;

        var channelTime = 0f;

        var idleStyle = Random.Range(0, 2);
        _an.SetInteger("idleStyle", idleStyle);

        _an.CrossFadeInFixedTime("shoot_start", .1f);

        _keyDown = true;

        while (_keyDown)
        {
            if (channelTime > initialChargeDuration)
            {
                _sound.ForceStop();
                _sound.OnShoot();

                _an.CrossFadeInFixedTime("shoot_channel", .1f);             
            }

            yield return new WaitForSeconds(shootCooldown);
            UpdateReserveAmmo(-1);
            ManageProjectile();
            AddRecoil();

            channelTime += shootCooldown;
        }
        _shooting = false;
    }

    protected override void ManageProjectile()
    {
        //Owner.ApplyShake(ShakeDuration, ShakeIntensity);

        var dir = (_owner.cam.transform.forward + _muzzle.transform.forward).normalized;
        dir.Normalize();

        var b = new HitscanBeam(_muzzle.transform.position, dir.normalized, damage * shootCooldown, BeamSize);
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

        SetAmmoOnHUD();
    }
}
