using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;


public class player : MonoBehaviour
{
    [SerializeField] private float movementspeed = 5f;
    [SerializeField] private Tilemap _tiledetector;
    [SerializeField] private GameObject battletile;

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private bool IsMoving;
    private bool battlezone;
    
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
        if(!battlezone) return;

        Vector3Int tilepostion = _tiledetector.WorldToCell(_tiledetector.transform.position);
        TileBase tile = _tiledetector.GetTile(tilepostion);
    }
}
    //The Fixed update is for rendering all the physics 
    
