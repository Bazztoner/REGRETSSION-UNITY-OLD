using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WPN_Shockroach : WeaponBase
{
    public float initialChargeDuration;

    public float ammoPerSecond;
    public float ticks;
    float _tickDuration;
    bool _keyDown = false;

    public float RepairByTick
    {
        get { return ammoPerSecond / ticks; }
    }

    protected override void Start()
    {
        base.Start();
        UpdateAmmo(maxAmmo);
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
        var rechargeCondition = new WaitUntil(() => _ammo < maxAmmo && !_keyDown);

        while ("Nisman" != "Vivo")
        {
            yield return rechargeCondition;

            UpdateAmmo(Mathf.RoundToInt(ammoPerSecond));

            yield return waitASecond;
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
        _canShoot = () => !_holstering && GetAmmo() > 0 && _drawn;
        _canReload = () => !_shooting && !_reloading && !_holstering && GetAmmo() < maxAmmo && _drawn;
    }

    protected override void Shoot()
    {
        if (!_keyDown) StartCoroutine(ShootHandler());
    }

    IEnumerator ShootHandler()
    {
        UpdateAmmo(-1);
        _shooting = true;

        var channelTime = 0f;

        var idleStyle = Random.Range(0, 2);
        _an.SetInteger("idleStyle", idleStyle);

        _an.CrossFadeInFixedTime("shoot_start", Mathf.Epsilon);

        _keyDown = true;

        while (_keyDown)
        {
            if (channelTime > initialChargeDuration)
            {
                _an.CrossFadeInFixedTime("shoot_channel", Mathf.Epsilon);
            } 

            yield return new WaitForSeconds(shootCooldown);

            channelTime += shootCooldown;
        }

        _shooting = false;
    }
}
