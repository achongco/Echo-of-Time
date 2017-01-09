using UnityEngine;

public class MapCamera : MonoBehaviour
{
    void Update()
    {
        transform.position = new Vector3(transform.position.x + Input.GetAxis("Horizontal"), transform.position.y + Input.GetAxis("Vertical"), -10);
    }
}
