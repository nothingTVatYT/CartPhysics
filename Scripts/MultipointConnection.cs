using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MultipointConnection : MonoBehaviour
{
    [Serializable]
    public class PointConnection {
        [Tooltip("attach the object to this transform")]
        public Transform target;
        [Tooltip("offset to target's center used as an anchor")]
        public Vector3 targetAnchor;
        [Tooltip("offset to the center of the object that is drawn towards the target's anchor")]
        public Vector3 ourAnchor;
        [Tooltip("Make this a bidrectional spring force")]
        public bool bidrectional = false;
        public float constantForce = 10;
        public float tolerance = 0.1f;
    }
    public PointConnection[] connections;
    private Rigidbody body;

    public void Start() {
        body = GetComponent<Rigidbody>();
    }

    public void OnDrawGizmos() {
        if (connections != null) {
            foreach (PointConnection conn in connections) {
                if (conn.target != null) {
                    Vector3 worldTargetAnchor = conn.target.transform.TransformPoint(conn.targetAnchor);
                    Vector3 worldOurAnchor = transform.TransformPoint(conn.ourAnchor);
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireSphere(worldTargetAnchor, 0.1f);
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireSphere(worldOurAnchor, 0.1f);
                    Gizmos.DrawLine(worldTargetAnchor, worldOurAnchor);
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (connections != null) {
            foreach (PointConnection conn in connections) {
                if (conn.target != null) {
                    Vector3 worldTargetAnchor = conn.target.transform.TransformPoint(conn.targetAnchor);
                    Vector3 worldOurAnchor = transform.TransformPoint(conn.ourAnchor);
                    Vector3 distance = worldTargetAnchor-worldOurAnchor;
                    if (distance.magnitude > conn.tolerance) {
                        body.AddForceAtPosition(distance.normalized * conn.constantForce, worldOurAnchor, ForceMode.Acceleration);
                        if (conn.bidrectional) {
                            Rigidbody other = conn.target.GetComponent<Rigidbody>();
                            if (other != null) {
                                other.AddForceAtPosition(-distance.normalized * conn.constantForce, worldTargetAnchor, ForceMode.Acceleration);
                            }
                        }
                    }
                }
            }
        }
    }
}
