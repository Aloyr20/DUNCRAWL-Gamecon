using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float MoveSpeed;
    Vector3 Movement = Vector3.zero;

    CharacterController controller;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();

        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Dagger"), true);

    }

    // Update is called once per frame
    void Update()
    {
        Movement.x = Input.GetAxisRaw("Horizontal");
        Movement.y = Input.GetAxisRaw("Vertical");

        Movement = Movement.normalized * MoveSpeed * Time.deltaTime;

        Movement = transform.right * Movement.x + transform.forward * Movement.y;
        controller.Move(Movement);
    }

    private void FixedUpdate()
    {
        
    }

    void Jump()
    {
        
    }
}
