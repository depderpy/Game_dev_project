using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;


public class player : MonoBehaviour
{

    [SerializeField] private float encounterNum = 100;
    [SerializeField] private float movementspeed = 5f;
    [SerializeField] private float tileSize = 1f;
    [SerializeField] private Tilemap _battletile;
    [SerializeField] private GameObject _tiledetector;
    

    private Vector3 _currentTilePosition;
    private Vector3 targetPosition;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private bool IsMoving;
    private bool in_battlezone;
    
    void Start()
    {
        targetPosition = transform.position;
    }

    private void Update()
    {
            moveDirection.x = Input.GetAxisRaw("Horizontal");
            moveDirection.y = Input.GetAxisRaw("Vertical");
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
        if(!in_battlezone) return;


        //This converts the 3d/world-space Coords to the tilemap coords so the grid's coordinates
        Vector3Int tilePosition = _battletile.WorldToCell(_tiledetector.transform.position);

        //this assigns the specific battletile sprite on screen to the tile varieble 
        TileBase tile = _battletile.GetTile(tilePosition);

        //Checking if the player is on the same tile and if they are we won't do more encounter checks cuz that is unfair LOL 
        if(tile == null || _currentTilePosition == (Vector3) tilePosition) return;

        _currentTilePosition = (Vector3)tilePosition;

        int randomNum = Random.Range(0, 500);
        if(randomNum < encounterNum)
        {
            Debug.Log("Encounter!!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("battlezone"))
        {
            in_battlezone = true;
            Debug.Log("entered battle zone");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("battlezone"))
        {
            in_battlezone = false;
            Debug.Log("exited battle zone");
            _currentTilePosition = new Vector3(-100000, -100000, -100000); //Random coord to reset the current tile position
        }
    }
}
    //The Fixed update is for rendering all the physics 
    
