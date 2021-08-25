using UnityEngine;

public class PlatformHandler : MonoBehaviour
{
    public GameObject cartRear;

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(cartRear.transform.position - transform.position);
    }
}
