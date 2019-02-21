using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerHead : MonoBehaviour
{
    public float rotClamp = 45f;

    public float sensitivity;
    public float rotationSpeed;
    PlayerController _player;
    Animator _an;

    void Start()
    {
        _player = GetComponentInParent<PlayerController>();
        _an = GetComponentInParent<Animator>();
    }

    void Update()
    {
        if (_player.LockedByGame) return;

        MouseLook();
    }

    void MouseLook()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        mouseX *= Time.deltaTime * sensitivity;
        mouseY *= Time.deltaTime * sensitivity;

        Vector3 headRot = transform.localEulerAngles + new Vector3(-mouseY, 0, 0f);
        headRot.x = ClampAngle(headRot.x, -rotClamp, rotClamp);
        transform.localRotation = Quaternion.Euler(headRot);

        Vector3 playerRot = _player.transform.eulerAngles + new Vector3(0, mouseX, 0f);
        _player.transform.rotation = Quaternion.Euler(playerRot);
    }

    float ClampAngle(float angle, float from, float to)
    {
        // accepts e.g. -80, 80
        if (angle < 0f) angle = 360 + angle;
        if (angle > 180f) return Mathf.Max(angle, 360 + from);
        return Mathf.Min(angle, to);
    }

    public void OnDeath()
    {
        _an.CrossFadeInFixedTime("Death", .01f);
    }

    public void OnRespawn()
    {
        _an.CrossFadeInFixedTime("Idle", .01f);
    }
}
