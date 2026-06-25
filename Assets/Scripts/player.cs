using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    [SerializeField] private float movementspeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    //The Fixed update is for rendering all the physics 
    void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * movementspeed;
    }
}
