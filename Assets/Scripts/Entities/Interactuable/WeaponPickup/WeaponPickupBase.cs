using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WeaponPickupBase : AmmoPickupBase
{
    [SerializeField] string _weaponName;

    AnimationCurve _anCurve;
    Transform _mesh;
    float _elapsed;
    readonly float _animDuration = 1f;

    void Awake()
    {
        _anCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(.25f, 90), new Keyframe(.5f, 180), new Keyframe(.75f, 270), new Keyframe(.1f, 360));
        _mesh = GetComponentInChildren<Renderer>().transform.parent;
    }

    void Update()
    { 
        //ExecuteAnimation();
    }

    void ExecuteAnimation()
    {
        if (_elapsed > _animDuration) _elapsed = 0;
        _mesh.localRotation = Quaternion.Euler(_mesh.transform.rotation.x, _anCurve.Evaluate(_elapsed), _mesh.transform.rotation.z);
        _elapsed += Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            player.OnPickedUpWeapon(_weaponName);
            player.OnPickedUpAmmo(_ammoGiven);
            gameObject.SetActive(false);
        }
    }

}
