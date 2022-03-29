using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public CharacterController controller;
    public MainControls mainInput;

    public Vector2 Move;
    Vector3 volecity;
    bool isGrounded;
    public float gravity = -9.81f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public float x;
    public float z;
    public float speed = 12f;
    public float jumpHight = 3f;
    // Start is called before the first frame update
    void Awake()
    {
        mainInput = new MainControls();
        
        mainInput.Movement.Movement.performed += ctx => Move = ctx.ReadValue<Vector2>();
        mainInput.Movement.Movement.canceled += ctx => Move = Vector2.zero;
        mainInput.Movement.Jump.performed += ctx => Jump();
    }

    private void OnEnable()
    {
        mainInput.Movement.Enable();
    }

    public void OnDisable()
    {
        mainInput.Movement.Disable();
    }
    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && volecity.y < 0) {
            volecity.y = -2f;
        }
        
        x = Move.x;
        z = Move.y;

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        volecity.y += gravity * Time.deltaTime;
        controller.Move(volecity * Time.deltaTime);
    }

    void Jump()
    {
        if (isGrounded)
        {
            volecity.y = Mathf.Sqrt(jumpHight * -2f * gravity);
        }
    }
}
