using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class player : MonoBehaviour
{
    [SerializeField] private float movementspeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private bool IsMoving;
    private LayerMask battlezone;
    
    void Start()
    {

    }

    private void Update()
    {
        if(!IsMoving)
        {
            moveDirection.x = Input.GetAxisRaw("Horizontal");
            moveDirection.y = Input.GetAxisRaw("Vertical");
        }
        move();
    }

    private void move()
    {
        if(moveDirection  != Vector2.zero)
        {
            transform.position = (moveDirection.normalized * movementspeed * Time.deltaTime) + (Vector2)transform.position;
        }
        CheckForEncounters();
    }

    private void CheckForEncounters()
    {
        if(Physics2D.OverlapCircle(transform.position, 0.01f, battlezone ) != null);
        {
            Debug.Log("battle encountered");
        }
    }
}
    //The Fixed update is for rendering all the physics 
    
