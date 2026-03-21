using System.Net;
using UnityEngine;

public class TurnScript : MonoBehaviour
{
    float MouseX;
    float MouseY;

    public float MouseSensitivity;
    public float TurnSpeed;

    public Transform LookDir;

    float Xrotation;

    public float CamBobAmp;
    public float CamBobSpd;
    float CamMid = 1.04f;
    private float timer;
    public bool IsWalking;
    public bool IsRunning;

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

        if (IsWalking)
        {
            timer += Time.deltaTime * CamBobSpd;

            float newY = CamMid + Mathf.Sin(timer) * CamBobAmp;
            transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
        }else if (IsRunning)
        {
            timer += Time.deltaTime * CamBobSpd * 2; //bob speed when running

            float newY = CamMid + Mathf.Sin(timer) * CamBobAmp;
            transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
        }
        else
        {
            timer = 0;
            transform.localPosition = new Vector3(transform.localPosition.x, CamMid, transform.localPosition.z);
        }
    }
}