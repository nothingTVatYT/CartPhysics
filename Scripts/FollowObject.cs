using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public Rigidbody attachTo;
    public bool keepInitialDistance = true;
    public float maxDistance;
    [Range(0,1)]
    public float tolerance = 0.01f;
    public float force = 2;
    [Range(1,4)]
    public int falloffPower = 1;
    [Range(0,1)]
    public float reflectForce = 0f;
    public float dragSideways = 1f;
    public ForceMode forceMode = ForceMode.VelocityChange;
    public float maxError = 60;
    public float generalDrag = 5f;
    public float angularDrag = 0.05f;

    private Rigidbody body;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        if (keepInitialDistance) {
            Vector3 distance = attachTo.position - transform.position;
            maxDistance = distance.magnitude;
        }
        body.drag = generalDrag;
        body.angularDrag = angularDrag;
        Vector3 com = Vector3.zero;
        bool firstCollider = true;
        int collidersFound = 0;
        foreach (Collider collider in GetComponentsInChildren<Collider>()) {
            if (firstCollider) {
                com = collider.bounds.center;
                firstCollider = false;
            } else {
                com += collider.bounds.center;
            }
            collidersFound++;
        }
        com /= collidersFound;
        com.y -= 0.2f;
        body.centerOfMass = body.transform.InverseTransformPoint(com);
    }

    void FixedUpdate()
    {
        Vector3 distance = attachTo.position - transform.position;
        float currentDistance = distance.magnitude;
        Vector3 direction = distance;
        transform.rotation = Quaternion.LookRotation(direction.normalized, transform.TransformDirection(Vector3.up));

        float forceFactor = Mathf.Abs(currentDistance - maxDistance)/maxDistance;

        // add drag sideways: a force relative to the negative sideway velocity
        Vector3 localVelocity = transform.InverseTransformDirection(body.velocity);
        float dragForceX = -localVelocity.x * dragSideways;
        body.AddRelativeForce(dragForceX, 0, 0, ForceMode.VelocityChange);
        Debug.DrawRay(body.position, dragForceX*transform.TransformDirection(Vector3.right), Color.red);

        // stop escalation
        if (forceFactor > maxError) {
            Debug.LogWarning("Teleport back: " + gameObject.name);
            body.velocity = attachTo.velocity;
            body.MovePosition(attachTo.transform.TransformDirection(Vector3.back) * maxDistance + attachTo.position);
        } else {
            forceFactor = Mathf.Pow(forceFactor, falloffPower);

            if (currentDistance > maxDistance + tolerance * maxDistance) {
                Vector3 relForce = Vector3.forward * force * forceFactor;
                Debug.DrawRay(transform.position, transform.TransformDirection(relForce), Color.magenta);
                body.AddRelativeForce(relForce * (1-reflectForce), forceMode);
                attachTo.AddForce(transform.TransformDirection(-relForce) * reflectForce, forceMode);
            } else if (currentDistance < maxDistance - maxDistance * tolerance) {
                Vector3 relForce = Vector3.back * force * forceFactor;
                Debug.DrawRay(transform.position, transform.TransformDirection(relForce), Color.magenta);
                body.AddRelativeForce(relForce * (1-reflectForce), forceMode);
                attachTo.AddForce(transform.TransformDirection(-relForce) * reflectForce, forceMode);
            }
        }

        Debug.DrawRay(body.worldCenterOfMass, body.velocity, Color.yellow);
        Debug.DrawRay(body.worldCenterOfMass, body.transform.TransformDirection(Vector3.up), Color.green);
        Debug.DrawRay(body.worldCenterOfMass, Vector3.up, Color.green);
        Debug.DrawRay(body.worldCenterOfMass, body.transform.TransformDirection(Vector3.forward), Color.blue);
        Debug.DrawRay(body.worldCenterOfMass, body.transform.TransformDirection(Vector3.right), Color.red);
    }
}
