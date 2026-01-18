using UnityEngine;

public class TurnScript : MonoBehaviour
{
    float MouseX;
    float MouseY;

    public float MouseSensitivity;
    public float TurnSpeed;

    public Transform PlayerBody;

    float Xrotation;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        MouseX = Input.GetAxis("Mouse X") * MouseSensitivity;
        MouseY = Input.GetAxis("Mouse Y") * MouseSensitivity;

        Xrotation -= MouseY;
        Xrotation = Mathf.Clamp(Xrotation, -90f, 90f);

        //PlayerBody.Rotate(Vector3.up * Input.GetAxisRaw("Horizontal") * Time.deltaTime * TurnSpeed);
        //transform.Rotate(Vector3.right * Xrotation * Time.deltaTime);

    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(Xrotation, PlayerBody.eulerAngles.y, PlayerBody.eulerAngles.z);
        PlayerBody.Rotate(Vector3.up * MouseX);
    }
}
