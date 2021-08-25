using UnityEngine;

public class OrientToCamera : MonoBehaviour
{
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(-Camera.main.transform.position + transform.position, Vector3.up);
    }
}
