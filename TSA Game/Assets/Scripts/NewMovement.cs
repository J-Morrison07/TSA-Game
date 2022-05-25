using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewMovement : MonoBehaviour
{
    public CharacterController controller;
    public MainControls mainInput;

    bool isGrounded;

    public Transform groundCheck;
    public Transform wallCheck;
    public LayerMask groundMask;

    public float acceleration = 2;
    public float airControl = 1;
    public float gravity = -9.81f;
    public float jumpStrength = 10;
    public float maxSpeed = 2;
    public float friction = 2;
    public float stepUpDistance = 0.4f;
    public float wallHitDistance = 0.6f;
    Vector3 velocity;
    Vector2 input;
    // Start is called before the first frame update
    void Awake() {
        mainInput = new MainControls();
        
        mainInput.Movement.Movement.performed += ctx => input = ctx.ReadValue<Vector2>();
        mainInput.Movement.Movement.canceled += ctx => input = Vector2.zero;
        mainInput.Movement.Jump.performed += ctx => Jump();
        //mainInput.Movement.Grapple.performed += ctx => Grapple(true);
        //mainInput.Movement.Grapple.canceled += ctx => Grapple(false);
    }

    private void OnEnable() {
        mainInput.Movement.Enable();
    }

    public void OnDisable() {
        mainInput.Movement.Disable();
    }

    Vector3 makeVector3(float ix, float iy, float iz) {
        Vector3 final;
        final.x = ix;
        final.y = iy;
        final.z = iz;
        return final;
    }
    // Update is called once per frame
    void Update() {
        if(velocity.y < 0) {
            isGrounded = Physics.CheckSphere(groundCheck.position, stepUpDistance, groundMask);
        }

        if(isGrounded) {
            //applies friction
            velocity.x = velocity.x / friction;
            velocity.z = velocity.z / friction;
            //moves the player but clamps velocity by the max speed
            velocity.x = Mathf.Clamp(velocity.x + input.x * acceleration, maxSpeed * -1, maxSpeed);
            velocity.z = Mathf.Clamp(velocity.z + input.y * acceleration, maxSpeed * -1, maxSpeed);
            velocity.y = 0;
            RaycastHit hit;
            Physics.Raycast(groundCheck.position + makeVector3(0, stepUpDistance, 0), makeVector3(0, -1, 0), out hit, stepUpDistance * 2);
            transform.position = makeVector3(0, hit.distance * -1, 0);
        } else {
            //no clamping for max speed and no clamping for friction
            velocity.x = velocity.x + input.x * acceleration * airControl;
            velocity.z = velocity.z + input.y * acceleration * airControl;
            velocity.y = velocity.y + gravity;
            Vector3 tmpVec;
            tmpVec.x = input.x / 3;
            tmpVec.z = input.y / 3;
            tmpVec.y = 0;
            if(Physics.CheckSphere(wallCheck.position + tmpVec, wallHitDistance, groundMask)){// this makes it so if you're up close to a wall with a lot of velocity in that direction, moving in a different direction is still possible
                velocity.x = 0;
                velocity.z = 0;
            }
        }
        controller.Move(velocity * Time.deltaTime);
    }

    void Jump() {
        if (isGrounded)
        {
            Debug.Log("jump started");
            isGrounded = false;
            controller.Move(makeVector3(0, 1, 0));
            velocity.y = jumpStrength;
        }
    }
    void Grapple(bool start) {
        
    }

}
