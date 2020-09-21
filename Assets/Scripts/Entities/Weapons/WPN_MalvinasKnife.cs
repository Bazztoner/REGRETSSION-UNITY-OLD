using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WPN_MalvinasKnife : WeaponBase
{
    readonly int _attackStyle = Animator.StringToHash("attackStyle");

    [Header("SwordCollider")]
    public float xSize;
    public float ySize;
    public float zSize;

    Vector3 SwordSize
    {
        get => new Vector3(xSize, ySize, zSize);
    }

    protected override void InitializeSoundModule()
    {
        _sound = GetComponent<MalvinasKnifeSoundModule>();
    }

    protected override void CheckInput()
    {
        if (Input.GetMouseButtonDown(0) && _canShoot())
        {
            Shoot();
        }
    }

    protected override void InitializeConditions()
    {
        _canShoot = () => !_shooting && !_reloading && !_holstering && _drawn;
    }

    protected override void Shoot()
    {
        StartCoroutine(ShootHandler());
    }

    IEnumerator ShootHandler()
    {
        var stateName = "slash";

        _shooting = true;
        _sound.OnShoot();

        ManageProjectile();

        _an.CrossFadeInFixedTime(stateName, .1f);

        yield return new WaitForSeconds(shootCooldown);
        _shooting = false;
    }

    protected override void ManageProjectile()
    {
        var dir = _owner.cam.transform.forward;

        var b = new MeleeHitscan(_owner.cam.transform.position, dir.normalized, damage, SwordSize);
    }

    public override void SetAmmoOnHUD()
    {
        HUDController.Instance.SetAmmo("-");
    }

    protected override int GetReserveAmmo()
    {
        return 0;
    }

    protected override void SetBulletsInMag(int bullets, bool overrideBullets = false)
    {
        //
    }

    protected override void UpdateReserveAmmo(int ammo)
    {
       //
    }

    protected override int GetBulletsInMag()
    {
        return 0;
    }
}
