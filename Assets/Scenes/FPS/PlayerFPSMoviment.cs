using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerFPSMoviment : MonoBehaviour
{

    [SerializeField] Transform playerCamera;
    [SerializeField][Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;
    [SerializeField] bool cursorLock = true;
    [SerializeField] float mouseSensitivity = 3.5f;
    [SerializeField] float Speed = 6.0f;
    [SerializeField][Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;
    [SerializeField] float gravity = -30f;

    [SerializeField] LayerMask ground;

    public float jumpHeight = 6f;
    float velocityY;
    bool isGrounded;
    public float Drag = 15f;

    float cameraCap;
    Vector2 currentMouseDelta;
    Vector2 currentMouseDeltaVelocity;

    CharacterController controller;
    Vector2 currentDir;
    Vector2 currentDirVelocity;
    Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        //LOCK MOUSE, NOT SHOW IN SCENE
        if (cursorLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
        }
    }

    void Update()
    {
        UpdateMouse();
        UpdateMove();
    }

    void UpdateMouse()
    {
        //MOUSE MOVIMENTS
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        //SMOOTH MOVIMENT
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);

        cameraCap -= currentMouseDelta.y * mouseSensitivity;


        //LIMIT MOVIMENT OF Y MOVIMENT
        cameraCap = Mathf.Clamp(cameraCap, -90.0f, 90.0f);
        //MOVE ANGLE Y
        playerCamera.localEulerAngles = Vector3.right * cameraCap;
        //MOVE ANGLE X
        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }

    void UpdateMove()
    {
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position + Vector3.up * .1f, Vector3.down,out hit, 2f, ground);
        if (hit.point!= null)
        {
            Debug.DrawLine(transform.position, hit.point, Color.magenta);
        }
        

        //GET MOVIMENTS
        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();
        //SMOOTH MOVIMENT
        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);

        //AVOID SLIDING
        if (targetDir.magnitude < 0.0001f && isGrounded)
        {
            currentDir = Vector2.zero;
        }

        velocityY += gravity * 2f * Time.deltaTime;

        //SET VELOCITY X & Z
        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * Speed + Vector3.up * velocityY;

        //APPEND VELOCITY IN X,Y & Z
        velocity = new Vector3(velocity.x,0,velocity.z)/(1+Drag/100) + new Vector3(0,velocity.y,0);

        //MOVE CHARACTER CONTROLLER
        controller.Move(velocity * Time.deltaTime);


        //CAL VELOCITY Y - JUMP
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocityY = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if (isGrounded! && controller.velocity.y < -1f)
        {
            velocityY = -8f;
        }
    }
}
