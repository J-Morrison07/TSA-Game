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
    public float grapplingHookStrength = 1f;
    bool grappling = false;
    bool wasGroundedLastFrame;
    bool velocityFromJump;
    // Start is called before the first frame update
    void Awake()
    {
        mainInput = new MainControls();
        
        mainInput.Movement.Movement.performed += ctx => Move = ctx.ReadValue<Vector2>();
        mainInput.Movement.Movement.canceled += ctx => Move = Vector2.zero;
        mainInput.Movement.Jump.performed += ctx => Jump();
        mainInput.Movement.Grapple.performed += ctx => Grapple(true);
        mainInput.Movement.Grapple.canceled += ctx => Grapple(false);
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
        if(wasGroundedLastFrame != isGrounded && wasGroundedLastFrame == false && !velocityFromJump){
            volecity.x = volecity.y = volecity.z = 0;
            volecity.x = x;
            volecity.z = z;
            Debug.Log("velocity should have been reset");
        }

        wasGroundedLastFrame = isGrounded;
        x = Move.x;
        z = Move.y;

        Vector3 move = transform.right * x + transform.forward * z;

        if(grappling == false) {
            controller.Move(move * speed * Time.deltaTime);
        }
        volecity.y += gravity * Time.deltaTime;
        controller.Move(volecity * Time.deltaTime);
    }

    void Jump()
    {
        if (isGrounded)
        {
            velocityFromJump = true;
            volecity.y = Mathf.Sqrt(jumpHight * -2f * gravity);
        }
    }
    void Grapple(bool start){
        grappling = start;
        Vector3 grappleDirection;
        if(start == false && isGrounded){
            isGrounded = false;
            velocityFromJump = false;
            grappleDirection.x = x;
            grappleDirection.z = z;
            grappleDirection.y = 0;
            RaycastHit hit;
            Vector3 tmpVec2;
            tmpVec2.x = tmpVec2.y = 0;
            tmpVec2.z = 3;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position + tmpVec2, grappleDirection, out hit, 50))
            {
                /*volecity.x = grappleDirection.x * grapplingHookStrength * hit.distance;
                volecity.y = grapplingHookStrength * hit.distance * 2 * grappleDirection.z;
                isGrounded = false;
                velocityFromJump = false;*/
                transform.position = hit.point;
            }
            else
            {
                Debug.DrawRay(transform.position, grappleDirection * 1000, Color.white);
                Debug.Log("Did not Hit");
            }
        }
    }
}
