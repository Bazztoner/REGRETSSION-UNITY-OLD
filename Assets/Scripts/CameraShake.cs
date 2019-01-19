using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public void Shake(float duration, float intensity)
    {
        iTween.PunchPosition(gameObject, new Vector3(intensity, intensity / 2, 0), duration);
        //StartCoroutine(ShakeCoroutine(duration, intensity));
    }

    IEnumerator ShakeCoroutine(float duration, float intensity)
    {
        Vector3 lastMovement = Vector3.zero;
        Vector3 newMovement = Vector3.zero;
        float timer = 0;
        while (timer < duration)
        {
            var initialXPos = transform.position.x;

            timer += Time.deltaTime;
            lastMovement = newMovement;
            newMovement = new Vector3(Random.Range(initialXPos * Time.deltaTime, intensity * Time.deltaTime), Random.Range(-intensity * Time.deltaTime, intensity * Time.deltaTime), 0);
            transform.position -= lastMovement;
            transform.position += newMovement;
            yield return new WaitForEndOfFrame();
        }
        transform.position -= lastMovement;
    }
}
