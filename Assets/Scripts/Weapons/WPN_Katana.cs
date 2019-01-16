using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WPN_Katana : WeaponBase
{
    int _attackStyle = Animator.StringToHash("attackStyle");

    protected override void CheckInput()
    {
        if (Input.GetMouseButtonDown(0) && _canShoot())
        {
            Shoot();
        }
    }

    protected override int GetAmmo()
    {
        return 0;
    }

    protected override void InitializeConditions()
    {
        _canShoot = () => !_shooting && !_reloading && !_holstering && _drawn;
    }

    protected override void SetAmmo(int ammo)
    {
       //
    }

    protected override void Shoot()
    {
        StartCoroutine(ShootHandler());
    }

    IEnumerator ShootHandler()
    {
        var rndStyle = Random.Range(0, 3);

        var stateName = "slash" + rndStyle;

        _shooting = true;

        _an.CrossFadeInFixedTime(stateName, .1f);

        yield return new WaitForSeconds(shootCooldown);

        _shooting = false;
    }
}
