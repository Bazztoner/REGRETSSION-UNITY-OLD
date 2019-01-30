using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CameraController : MonoBehaviour
{
    float _currentRotation, _currentRotVelocity;
    float _recoilRecoveryTime;

    void Update()
    {
        ManageRecoil();
    }

    public void AddRecoil(float recoveryTime, float amount)
    {
        _recoilRecoveryTime = recoveryTime;
        _currentRotation -= amount;
    }

    void ManageRecoil()
    {
        _currentRotation = Mathf.SmoothDamp(_currentRotation, 0, ref _currentRotVelocity, _recoilRecoveryTime);

        transform.localRotation = Quaternion.Euler(_currentRotation, 0, 0);
    }
}
