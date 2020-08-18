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
        _tickDuration = 1 / ticks;
    }

    protected override void InitializeSoundModule()
    {
        _sound = GetComponent<DisplacerSoundModule>();
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

    protected override int GetBulletsInMag()
    {
        return _currentBulletsInMag <= 0 ? 0 : _currentBulletsInMag;
    }

    protected override void InitializeConditions()
    {
        _canShoot = () => !_holstering && GetReserveAmmo() >= minCharge && _drawn && _canTap;
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

            if (channelTime < ChargePerTick && chargeAcum < maxCharge && chargeAcum < _currentBulletsInMag)
            {
                channelTime += _tickDuration;
            }
            else
            {
                _keyDown = false;
                _canTap = false;
            }
        }

        UpdateReserveAmmo(-chargeAcum);
        _an.CrossFadeInFixedTime("fire", .1f);

        yield return new WaitForEndOfFrame();

        //shoot
        AddRecoil();

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
