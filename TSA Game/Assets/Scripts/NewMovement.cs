using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewMovement : MonoBehaviour
{
    public Animator animator; //for model animations
    public GameObject armatureRef;
    public CharacterController controller;
    public MainControls mainInput;

    bool isGrounded;
    bool wasGrounded;

    public Transform groundCheck;
    public Transform wallCheck;
    public LayerMask groundMask;

    public float acceleration = 2;
    public float airControl = 1;
    public Vector3 maxAirVelocity; // leave at 0 to not enforce that axis, a good example would be the y axis to let the jump velocity be as high as possible
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

    void Awake() {
        mainInput = new MainControls();
        
        mainInput.Movement.Movement.performed += ctx => input = ctx.ReadValue<Vector2>();
        mainInput.Movement.Movement.canceled += ctx => input = Vector2.zero;
        mainInput.Movement.Jump.performed += ctx => Jump();
        mainInput.Movement.Grapple.performed += ctx => Grapple(true);
        mainInput.Movement.Grapple.canceled += ctx => Grapple(false);

        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
    }

    private void OnEnable() {
        mainInput.Movement.Enable();
    }

    public void OnDisable() {
        mainInput.Movement.Disable();
    }

    bool withinRange(float min, float max, float t){
        return t > min && t < max;
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
        if(Mathf.Sqrt(input.x * input.x + input.y + input.y) > 1)
        {
            Debug.Log(input.x / Mathf.Sqrt(input.x * input.x + input.y + input.y));
            input.x = input.x / Mathf.Sqrt(input.x * input.x + input.y + input.y);
            input.y = input.y / Mathf.Sqrt(input.x * input.x + input.y + input.y);
        }
        if(velocity.y < 0.1f) {
            isGrounded = Physics.CheckSphere(groundCheck.position, stepUpDistance, groundMask);
            if(isGrounded){
                animator.ResetTrigger("jumpend");
            }else{
                animator.SetTrigger("jumpend");
            }
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

                //rotates the player based off input
                if(input.x != 0 || input.y != 0){
                    armatureRef.transform.rotation = Quaternion.Euler(-90, 0, Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg);
                }
                animator.ResetTrigger("jumpstart");
            } else {
                //no clamping for max speed and no clamping for friction
                if(maxAirVelocity.x != 0 && withinRange(maxAirVelocity.x * -1, maxAirVelocity.x, velocity.x + input.x * acceleration * airControl)){
                    velocity.x = velocity.x + input.x * acceleration * airControl;
                    velocity.x = Mathf.Clamp(maxAirVelocity.x * -1, velocity.x, maxAirVelocity.x);
                } else if(maxAirVelocity.x == 0 && withinRange(maxAirVelocity.x * -1, maxAirVelocity.x, velocity.x)){
                    velocity.x = velocity.x + input.x * acceleration * airControl;
                }

                if(maxAirVelocity.z != 0 && withinRange(maxAirVelocity.z * -1, maxAirVelocity.z, velocity.z + input.y * acceleration * airControl)){
                    velocity.z = velocity.z + input.y * acceleration * airControl;
                    velocity.z = Mathf.Clamp(maxAirVelocity.z * -1, velocity.z, maxAirVelocity.z);
                } else if(maxAirVelocity.z == 0 && withinRange(maxAirVelocity.z * -1, maxAirVelocity.z, velocity.z)) {
                    velocity.z = velocity.z + input.y * acceleration * airControl;
                }

                if(maxAirVelocity.y != 0 && withinRange(maxAirVelocity.y * -1, maxAirVelocity.y, velocity.y)){
                    velocity.y = velocity.y + gravity;
                    velocity.y = Mathf.Clamp(maxAirVelocity.y * -1, velocity.y, maxAirVelocity.y);
                } else if(maxAirVelocity.y == 0) {
                    velocity.y = velocity.y + gravity;
                }

                //rotates the player based off velocity
                if(input.x != 0 || input.y != 0){
                    armatureRef.transform.rotation = Quaternion.Euler(-90, 0, Mathf.Atan2(velocity.normalized.x, velocity.normalized.z) * Mathf.Rad2Deg);
                }
                animator.SetFloat("verticalvelocity", Mathf.Clamp(velocity.y / 4 + 0.5f, 0, 1));
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

            //animation code
            animator.SetFloat("walk", Mathf.Sqrt((input.x * input.x) + (input.y * input.y)));
            animator.SetFloat("walkspeed", Mathf.Sqrt((input.x * input.x) + (input.y * input.y)) + 1.0f);
        }
    }

    void Jump() {
        if (isGrounded && !grappling)
        {
            isGrounded = false;
            controller.Move(makeVector3(0, 1, 0));
            velocity.y = jumpStrength;
            animator.SetTrigger("jumpstart");
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
            } else {
                //probably an animation here?
            }
        }
    }

}
