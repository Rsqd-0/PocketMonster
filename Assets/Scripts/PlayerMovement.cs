using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask ground;
    private bool grounded;
    [SerializeField] private float groundDrag;

    [SerializeField] private Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rigidBody;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.freezeRotation = true;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position,Vector3.down,playerHeight*0.5f + 0.2f, ground);
        
        MyInput();
        SpeedControl();
        

        if (grounded)
            rigidBody.drag = groundDrag;
        else
            rigidBody.drag = 0;


    }

    private void FixedUpdate()
    {
        
        MovePlayer();
        
        if (Mathf.Approximately(horizontalInput, 0f) && Mathf.Approximately(verticalInput, 0f))
        {
            rigidBody.velocity = Vector3.zero;
        }
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        moveDirection = moveDirection.normalized * (moveSpeed * Time.fixedDeltaTime);
        
        rigidBody.MovePosition(transform.position + moveDirection);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rigidBody.velocity.x, 0f, rigidBody.velocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rigidBody.velocity = new Vector3(limitedVel.x, rigidBody.velocity.y, limitedVel.z);
        }
    }
}