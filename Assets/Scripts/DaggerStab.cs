using UnityEngine;

public class DaggerStab : MonoBehaviour
{
    public Transform DaggerTransform;
    Vector3 DaggerLocalpos;
    Vector3 DaggerLocalrot;
    public Vector3 StabOffsetPos;
    public Vector3 StabOffsetRotation;

    public float StabDuration;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DaggerLocalpos = DaggerTransform.localPosition;
        DaggerLocalrot = DaggerTransform.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            StabStart();
        }
    }

    public void StabStart()
    {
        DaggerTransform.localPosition = DaggerLocalpos + StabOffsetPos;
        DaggerTransform.localEulerAngles = DaggerLocalrot + StabOffsetRotation;

        Invoke("StabEnd", StabDuration);
    }

    void StabEnd()
    {
        DaggerTransform.localPosition = DaggerLocalpos;
        DaggerTransform.localEulerAngles = DaggerLocalrot;
    }
}
