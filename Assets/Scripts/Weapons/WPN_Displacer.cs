using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WPN_Displacer : WeaponBase
{
    public float maxChargeTime;
    public int maxCharge;
    public int minCharge;
    public float ticks;
    float _tickDuration;
    bool _keyDown = false, _canTap = true;

    public float ChargePerTick
    {
        get { return (maxCharge / maxChargeTime) / ticks; }
    }
    protected override void Start()
    {
        base.Start();
        UpdateAmmo(maxAmmo);
        _tickDuration = 1 / ticks;
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
        else
        {
            _keyDown = false;
            _canTap = true;
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
        _canShoot = () => !_holstering && GetAmmo() >= minCharge && _drawn && _canTap;
        _canReload = () => !_shooting && !_reloading && !_holstering && GetAmmo() < maxAmmo && _drawn;
    }

    protected override void Shoot()
    {
        if (!_keyDown && _canTap) StartCoroutine(ShootHandler());
    }

    IEnumerator ShootHandler()
    {
        _shooting = true;

        var channelTime = 0f;
        var chargeAcum = minCharge;

        var idleStyle = Random.Range(0, 2);
        _an.SetInteger("idleStyle", idleStyle);

        _an.CrossFadeInFixedTime("spinup", .1f);

        _keyDown = true;

        while (_keyDown)
        {
            chargeAcum = Mathf.FloorToInt(Mathf.Lerp(minCharge, maxCharge, channelTime / maxChargeTime));

            yield return new WaitForSeconds(_tickDuration);

            if (channelTime < ChargePerTick && chargeAcum < maxCharge && chargeAcum < _ammo)
            {
                channelTime += _tickDuration;
            }
            else
            {
                _keyDown = false;
                _canTap = false;
            }
        }

        UpdateAmmo(-chargeAcum);
        _an.CrossFadeInFixedTime("fire", .1f);

        yield return new WaitForEndOfFrame();

        //shoot
        AddRecoil();

        _shooting = false;
    }
}
