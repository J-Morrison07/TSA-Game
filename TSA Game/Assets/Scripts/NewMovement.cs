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
    public bool limitAirVelocity = false;
    public Vector3 maxAirVelocity; // leave at 0 to not enforce that axis, a good example would be the y axis
    public float gravity = -9.81f;
    public float jumpStrength = 10;
    public float maxSpeed = 2;
    public float friction = 2;
    public float stepUpDistance = 0.4f;
    public float wallHitDistance = 0.6f;
    public float grapplingHookStrength = 1;
    bool grappling = false;
    Vector3 velocity;
    Vector2 input;
    // Start is called before the first frame update
    void Awake() {
        mainInput = new MainControls();
        
        mainInput.Movement.Movement.performed += ctx => input = ctx.ReadValue<Vector2>();
        mainInput.Movement.Movement.canceled += ctx => input = Vector2.zero;
        mainInput.Movement.Jump.performed += ctx => Jump();
        mainInput.Movement.Grapple.performed += ctx => Grapple(true);
        mainInput.Movement.Grapple.canceled += ctx => Grapple(false);
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
        input = input.normalized; // makes it so you don't go any faster while moving diagonally
        if(velocity.y < 0.1f) {
            isGrounded = Physics.CheckSphere(groundCheck.position, stepUpDistance, groundMask);
        }
        if(!grappling){
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
                velocity.y = hit.distance * -1 / Time.deltaTime;
            } else {
                //no clamping for max speed and no clamping for friction
                if (limitAirVelocity){
                    if(!(maxAirVelocity.x != 0 && maxAirVelocity.x < Mathf.Abs(velocity.x))){
                        velocity.x = velocity.x + input.x * acceleration * airControl;
                    }
                    if(!(maxAirVelocity.y != 0 && maxAirVelocity.y < Mathf.Abs(velocity.y))){
                        velocity.z = velocity.z + input.y * acceleration * airControl;
                    }
                    if(!(maxAirVelocity.z != 0 && maxAirVelocity.z < Mathf.Abs(velocity.z))){
                        velocity.y = velocity.y + gravity;
                    }
                }else{    
                    velocity.x = velocity.x + input.x * acceleration * airControl;
                    velocity.z = velocity.z + input.y * acceleration * airControl;
                    velocity.y = velocity.y + gravity;
                }
            }
            Vector3 tmpVec;
            tmpVec.x = Vector3.Normalize(velocity).x / 1.5f;
            tmpVec.z = Vector3.Normalize(velocity).z / 1.5f;
            tmpVec.y = 0;
            if(Physics.CheckSphere(wallCheck.position + tmpVec, wallHitDistance, groundMask)){// this makes it so if you're up close to a wall with a lot of velocity in that direction, moving in a different direction is still possible
                    velocity.x = 0;
                    velocity.z = 0;
            }
            controller.Move(velocity * Time.deltaTime);
        }
    }

    void Jump() {
        if (isGrounded && !grappling)
        {
            isGrounded = false;
            controller.Move(makeVector3(0, 1, 0));
            velocity.y = jumpStrength;
        }
    }
    void Grapple(bool start) {
        grappling = start;
        Vector3 grappleDirection;
        if(!start){ // add " && isGrounded" to make it so you can only grapple on land, i think it's fun to do it in midair but that's just personal prefrence
            isGrounded = false;
            grappleDirection.x = input.x;
            grappleDirection.y = input.y;
            grappleDirection.z = 0;
            RaycastHit hitResult;
            Vector3 tmpVec2; // adds a vector offset so the ray comes from the player's head
            tmpVec2.x = tmpVec2.y = 0;
            tmpVec2.z = 0.25f;
            // Does the ray intersect any objects excluding the player layer
            bool hitSomething = Physics.Raycast(transform.position + tmpVec2, grappleDirection, out hitResult, 50);
            if (hitSomething)
            {
                velocity.x = grappleDirection.x * grapplingHookStrength * (hitResult.distance * 2);
                velocity.y = grapplingHookStrength * ((hitResult.distance + 3) * 5) * grappleDirection.z;
                isGrounded = false;
                Debug.Log("hit something");
            } else {
                //probably an animation here?
            }
        }
    }

}
