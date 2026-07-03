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
    [SerializeField] private LayerMask obstacleLayer; 
    [SerializeField] private LayerMask InteractableLayer;
    

    private Vector3 _currentTilePosition;
    private Vector3 targetPosition;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private bool IsMoving;
    private bool in_battlezone;
    private Vector3 facingDirection;
    

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        targetPosition = transform.position;
    }

    private void Update()
    {
        if(IsMoving)
        {
            MoveToTile();
            return;
        }

        //Movement Keys 
        if (Input.GetKey(KeyCode.W))
            startMove(Vector3.up);  

        else if (Input.GetKey(KeyCode.S))
            startMove(Vector3.down);

        else if (Input.GetKey(KeyCode.A))
            startMove(Vector3.left);

        else if (Input.GetKey(KeyCode.D))
            startMove(Vector3.right);


    //Interacting Key
        if (Input.GetKeyDown(KeyCode.E))
        {
            interact();
        }
    }

    private void startMove(Vector3 direction)
    {
        facingDirection = direction;

        //this is calculating how far the player has to move based on the direction they are facing and the tile (since tile size is always 1 so tilesize is just there to keep it consistent)
        targetPosition = transform.position + (direction * tileSize);
       if(Iswalkable(targetPosition))
        {
            animator.SetFloat("face_X", facingDirection.x);
            animator.SetFloat("face_Y", facingDirection.y);
            
            IsMoving = true;
            animator.SetBool("isMoving", IsMoving);
        } 
        
    }


    //This function basically moves the player to the specific tile while the player is holding down a key 
    private void MoveToTile()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementspeed * Time.deltaTime);

        if(Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.position = targetPosition;
            IsMoving = false;
            animator.SetBool("isMoving", IsMoving);
        }
        
        CheckForEncounters();
        
    }

    private void interact()
    {
        var facingdir = new Vector3(animator.GetFloat("face_X"), animator.GetFloat("face_Y"));
        var interactPos = transform.position + facingdir;

        //This is testing if the direction thing works
        //Debug.DrawLine(transform.position, interactPos, Color.red, 1f);

        var collider = Physics2D.OverlapCircle(interactPos,0.2f, InteractableLayer);
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact();
        }
    }

    private bool Iswalkable(Vector3 position)
    {
        Collider2D obstacle = Physics2D.OverlapCircle(position, 0.2f, obstacleLayer | InteractableLayer);

        if(obstacle != null)
        {
            Debug.Log("you bumped into something");
            return false;
        }
        return true;
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
    
