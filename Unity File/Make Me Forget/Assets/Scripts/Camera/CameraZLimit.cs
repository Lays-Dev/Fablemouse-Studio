using UnityEngine;

// stops following once player hits the number I said on Z
[DefaultExecutionOrder(100)]
[RequireComponent(typeof(SideScrollerCamera))]
public class CameraZLimit : MonoBehaviour
{
    public float playerMinZ = -1.205518f;

    SideScrollerCamera follow;

    void Awake()
    {
        follow = GetComponent<SideScrollerCamera>();
    }

    void LateUpdate()
    {
        // where the camera is where player stands
        float forwardZ = (transform.rotation * Vector3.forward).z;
        float cameraMinZ = playerMinZ - forwardZ * follow.distance;

        Vector3 pos = transform.position;
        if (pos.z < cameraMinZ)
        {
            pos.z = cameraMinZ;
            transform.position = pos;
        }
    }
}