using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Lift : MonoBehaviour
{
    public Vector3 startPos, endPos;
    public float arriveTime;
    Rigidbody _rb;
    bool _moving = false;
    enum ArrivePositions { Start, End };
    ArrivePositions _toArrive = ArrivePositions.End;

    AudioSource _src;
    public AudioClip interactSound;

    void Start()
    {
        _src = GetComponent<AudioSource>();
        startPos = transform.position;
        _rb = GetComponent<Rigidbody>();
    }

    IEnumerator MoveToNextPos(Vector3 arrivePosition)
    {
        _src.PlayOneShot(interactSound);
        _moving = true;

        var elapsed = 0f;
        Vector3 posWhenStarted = transform.position;
        var perc = Time.fixedDeltaTime / arriveTime;

        while (Vector3.Distance(transform.position, arrivePosition) > .01f)
        {
            elapsed += perc;
            var newPos = Vector3.Lerp(posWhenStarted, arrivePosition, elapsed);

            _rb.MovePosition(newPos);

            yield return new WaitForFixedUpdate();
        }

        _moving = false;
    }

    public void OnButtonInteract()
    {
        if (!_moving)
        {
            var arriveTo = _toArrive == ArrivePositions.End ? endPos : startPos;

            StartCoroutine(MoveToNextPos(arriveTo));

            _toArrive = _toArrive == ArrivePositions.End ? ArrivePositions.Start : ArrivePositions.End;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.transform.SetParent(this.transform);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.transform.SetParent(null);
        }
    }
}
