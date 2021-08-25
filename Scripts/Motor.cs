using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Motor : MonoBehaviour
{
    public bool motorOn = false;
    [Tooltip("Maximum force to get this motor running")]
    public float force = 800;
    public float maxSpeed = 1f;
    [Tooltip("Turn this amount to the right every 1/50 sec.")]
    public float rotationSpeed = 0.001f;
    [Tooltip("Run an interval of start/stop for this number of seconds")]
    public float runInterval = 15;

    private Rigidbody body;
    private float forceFactor = 0f;

    // Start is called before the first frame update
    public void Start() {
        body = GetComponent<Rigidbody>();
    }    

    // Update is called once per frame
    void FixedUpdate()
    {
        if (runInterval > 0) {
            motorOn = (Mathf.Sin(Time.fixedTime * 2 * Mathf.PI / runInterval)) > -0.1f;
        }

        if (motorOn) {
            float speed = body.velocity.magnitude;
            if (forceFactor < 1f) {
                forceFactor += 0.1f;
            }
            forceFactor += 0.1f;
            body.AddRelativeForce(Vector3.Lerp(Vector3.zero, Vector3.forward * force * forceFactor, 1f - speed/maxSpeed));
            transform.rotation *= Quaternion.AngleAxis(90 * rotationSpeed * speed, Vector3.up);
            body.rotation = transform.rotation;
        } else {
            forceFactor = 0.1f;
        }
    }
}
