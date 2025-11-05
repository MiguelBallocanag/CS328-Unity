using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public bool lockY = true;
    private float fixedY;

    void Start()
    {
        fixedY = transform.position.y;
    }

    void LateUpdate()
    {
        float yPos = lockY ? fixedY : player.position.y;
        transform.position = new Vector3(player.position.x, yPos, transform.position.z);
    }
}
