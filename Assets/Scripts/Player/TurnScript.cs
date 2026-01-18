using UnityEngine;

public class TurnScript : MonoBehaviour
{
    float MouseX;
    float MouseY;

    public float MouseSensitivity;
    public float TurnSpeed;

    public Transform LookDir;

    float Xrotation;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        MouseX = Input.GetAxis("Mouse X") * MouseSensitivity;
        MouseY = Input.GetAxis("Mouse Y") * MouseSensitivity;

        Xrotation -= MouseY;
        Xrotation = Mathf.Clamp(Xrotation, -90f, 90f);

    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(Xrotation, LookDir.eulerAngles.y, LookDir.eulerAngles.z);
        LookDir.Rotate(Vector3.up * MouseX);
    }
}