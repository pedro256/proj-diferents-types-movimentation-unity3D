using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float MovementSpeedForward = 5f;
    public float MovementSpeedBackward = 4f;
    public float MovementSpeedHorizontal = 4.5f;
    public float MaxSpeed = 10f;
    public float Drag = 10f;
    public float RotationSpeed = 40f;
    public float JumpForce = 100f;

    public Animator AnimatorPlayer;
    public Rigidbody Rb;
    public Transform Cam;
    public LayerMask layerMaskGround;


    bool forward, backward, right, left,grounded, jump;

    void Start()
    {
        Rb = GetComponent<Rigidbody>();

    }

    void Update()
    {
        Handlepdate();
        LimitVelocity();
        HandleDrag();
        checkGrounded();
    }

    void checkGrounded()
    {
        grounded = Physics.Raycast(transform.position + Vector3.up*.1f,Vector3.down,2f,layerMaskGround);
    }
    void HandleDrag()
    {
        Rb.velocity = new Vector3(Rb.velocity.x,0,Rb.velocity.z)/(1+Drag/100) + new Vector3(0,Rb.velocity.y,0);
    }
    void handleRotation()
    {
        if ( (new Vector2(Rb.velocity.x,Rb.velocity.z)).magnitude > .1f)
        {
            Vector3 hDirRotation = (new Vector3(Rb.velocity.x, 0f, Rb.velocity.z));
            Quaternion rotate = Quaternion.LookRotation(hDirRotation, Vector3.up);
            transform.rotation =Quaternion.RotateTowards(transform.rotation,rotate,RotationSpeed);
        }
    }
    void Handlepdate() {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            forward = true;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            backward = true;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            right = true;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            left = true;
        }
        if(Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            jump = true;
        }
    }

    void LimitVelocity()
    {
        Vector3 horizontalVelocity = new Vector3(Rb.velocity.x, 0f, Rb.velocity.z);
        if(horizontalVelocity.magnitude > MaxSpeed)
        {
            Vector3 Limited = horizontalVelocity.normalized * MaxSpeed;
            Rb.velocity = new Vector3(Limited.x, Rb.velocity.y, Limited.z);
        }

    }
    void HandleMoviment()
    {
        //CAMERA DIRECTION
        Quaternion dirRotation = Quaternion.Euler(0f, Cam.rotation.eulerAngles.y, 0f);

        if (forward)
        {

            Rb.AddForce(dirRotation*Vector3.forward * MovementSpeedForward );
            forward = false;
        }
        if (backward)
        {
            Rb.AddForce(dirRotation*Vector3.back * MovementSpeedBackward );
            backward = false;
        }
        if (right)
        {
            Rb.AddForce(dirRotation * Vector3.right * MovementSpeedHorizontal);
            right = false;
        }
        if (left)
        {
            Rb.AddForce(dirRotation * Vector3.left * MovementSpeedHorizontal );
            left = false;
        }

        if(jump && grounded)
        {
            transform.position += Vector3.up * .1f;
            Rb.velocity = new Vector3 (Rb.velocity.x, 0f, Rb.velocity.z);
            Rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            jump = false;
        }
    }
    void FixedUpdate()
    {
        HandleMoviment();
        handleRotation();
    }

}


