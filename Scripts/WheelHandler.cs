using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WheelHandler : MonoBehaviour
{
    public GameObject leftWheel;
    public GameObject rightWheel;
    private Rigidbody body;

    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        RotateWheel(leftWheel);
        RotateWheel(rightWheel);
    }

    void RotateWheel(GameObject wheel) {
        Vector3 velocity = transform.InverseTransformVector(body.GetPointVelocity(wheel.transform.position));
        float radius = 1;
        SphereCollider collider = wheel.GetComponent<SphereCollider>();
        if (collider != null) {
            radius = collider.radius;
        }
        // we only care about the velocity in z direction
        float angle = velocity.z * Time.fixedDeltaTime / ( 2 * radius * Mathf.PI) * 360f;
        //Debug.Log("rotate wheel " + wheel.name + " by " + angle + " degrees.");
        wheel.transform.rotation *= Quaternion.AngleAxis(angle, Vector3.right);
    }
}
