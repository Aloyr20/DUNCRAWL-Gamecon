using UnityEngine;

public class CameraHealthBar : MonoBehaviour
{
    public Camera cam;

    void Start()
    {
        
    }

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
    }
}
