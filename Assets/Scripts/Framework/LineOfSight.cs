using UnityEngine;
using System.Collections;

public class LineOfSight : MonoBehaviour
{
    public GameObject target;
    public float viewAngle;
    public float viewDistance;

    public bool showGizmosInEditor;

    bool targetInSight;
    int layerMask;

    public Transform Target { get { return target.transform; } }
    public bool TargetInSight { get { return targetInSight; } }

    public void Configure(float viewAngle, float viewDistance)
    {
        this.viewAngle = viewAngle;
        this.viewDistance = viewDistance;
    }

    void Start()
    {
        layerMask = 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("Floor");
        if (Target != null) target = GameObject.FindObjectOfType<PlayerController>().gameObject;
    }

    void Update()
    {
        var dirToTarget = target.transform.position - transform.position;

        var angleToTarget = Vector3.Angle(transform.forward, dirToTarget);

        var sqrDistanceToTarget = (transform.position - target.transform.position).sqrMagnitude;

        RaycastHit rch;

        targetInSight = angleToTarget <= viewAngle && sqrDistanceToTarget <= viewDistance * viewDistance &&

            !Physics.Raycast(
                transform.position,
                dirToTarget,
                out rch,
                Mathf.Sqrt(sqrDistanceToTarget),
                layerMask
            );
    }

    void OnDrawGizmos()
    {
        if (showGizmosInEditor)
        {
            if (!target) target = GameObject.FindObjectOfType<PlayerController>().gameObject;

            Gizmos.color = targetInSight ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, target.transform.position);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, viewDistance);

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + (transform.forward * viewDistance));

            Vector3 rightLimit = Quaternion.AngleAxis(viewAngle, transform.up) * transform.forward;
            Gizmos.DrawLine(transform.position, transform.position + (rightLimit * viewDistance));

            Vector3 leftLimit = Quaternion.AngleAxis(-viewAngle, transform.up) * transform.forward;
            Gizmos.DrawLine(transform.position, transform.position + (leftLimit * viewDistance));
        }
    }
}
