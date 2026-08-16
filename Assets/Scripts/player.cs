using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using System;



// Controls the player's movement, interaction with NPCs/objects,
// and random encounter system.
public class player : MonoBehaviour
{
    // Determines the chance of encountering an enemy when
    // the player enters a new battle tile.
    [SerializeField] private float encounterNum = 100;

    // Controls how quickly the player moves between tiles.
    [SerializeField] private float movementspeed = 5f;

    // Determines the size of each movement tile.
    // A value of 1 means the player moves one Unity unit per tile.
    [SerializeField] private float tileSize = 1f;

    // Tilemap containing the tiles where random battles can occur.
    [SerializeField] private Tilemap _battletile;

    // GameObject used to detect which tile the player is currently standing on.
    [SerializeField] private GameObject _tiledetector;

    // LayerMask used to identify objects that the player cannot walk through.
    [SerializeField] private LayerMask obstacleLayer; 

    // LayerMask used to identify objects that the player can interact with.
    [SerializeField] private LayerMask InteractableLayer;
    

    // Stores the tile position that the player was previously checked on.
    // This prevents multiple encounter checks from happening on the same tile.
    private Vector3 _currentTilePosition;

    // Stores the position that the player is currently trying to move towards.
    private Vector3 targetPosition;

    // Stores the player's Rigidbody2D component.
    // Currently declared but not directly used in this script.
    private Rigidbody2D rb;

    // Stores the player's current movement direction.
    // Currently declared but not directly used in this script.
    private Vector2 moveDirection;

    // Determines whether the player is currently moving towards a tile.
    private bool IsMoving;

    // Determines whether the player is currently inside a battle zone.
    private bool in_battlezone;

    // Stores the direction the player is facing.
    private Vector3 facingDirection;

    // Public variable that can be used by other scripts to determine
    // whether the player is currently in battle.
    public bool InBattle;

    // Event that is triggered whenever a random encounter occurs.
    // GameManager listens for this event and starts a battle.
    public event Action OnEncounter;
    

    // Reference to the player's Animator component.
    private Animator animator;


    // Awake runs when the object is initialized.
    // Gets the Animator component attached to the player.
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    // Start runs when the game begins.
    // Sets the player's initial target position to their starting position.
    void Start()
    {
        targetPosition = transform.position;
    }


    // Handles the player's input and movement.
    // GameManager calls this method while the game is in FreeRoam state.
    public void HandleUpdate()
    {
        // If the player is already moving towards a tile,
        // continue moving and do not accept another movement input.
        if(IsMoving)
        {
            MoveToTile();
            return;
        }

        // Movement Keys 
        // Each key moves the player exactly one tile.
        // Else-if ensures the player can only move in one direction at a time.
        if (Input.GetKey(KeyCode.W))
            startMove(Vector3.up);  

        else if (Input.GetKey(KeyCode.S))
            startMove(Vector3.down);

        else if (Input.GetKey(KeyCode.A))
            startMove(Vector3.left);

        else if (Input.GetKey(KeyCode.D))
            startMove(Vector3.right);


        // Interacting Key
        // Pressing E attempts to interact with whatever is
        // directly in front of the player.
        if (Input.GetKeyDown(KeyCode.E))
        {
            interact();
        }
    }


    // Begins the player's movement towards a specific tile.
    // The direction is supplied by the movement keys.
    private void startMove(Vector3 direction)
    {
        // Store the direction so the Animator knows which way
        // the player should be facing.
        facingDirection = direction;

        // Calculate the position of the next tile.
        // The direction is multiplied by tileSize so that the player
        // moves exactly one tile in the chosen direction.
        targetPosition = transform.position + (direction * tileSize);

        // Check whether the destination tile is safe to walk onto.
        if(Iswalkable(targetPosition))
        {
            // Update the Animator with the player's horizontal
            // and vertical facing direction.
            animator.SetFloat("face_X", facingDirection.x);
            animator.SetFloat("face_Y", facingDirection.y);
            
            // Tell the movement system that the player has started moving.
            IsMoving = true;

            // Tell the Animator to play the moving animation.
            animator.SetBool("isMoving", IsMoving);
        } 
        
    }


    // Moves the player towards the target tile while they are moving.
    private void MoveToTile()
    {
        // Smoothly move the player towards the target position.
        // Time.deltaTime makes the movement consistent across different
        // frame rates.
        transform.position = Vector3.MoveTowards(
            transform.position, 
            targetPosition, 
            movementspeed * Time.deltaTime
        );

        // Check whether the player has almost reached the target tile.
        if(Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            // Snap the player exactly onto the target position.
            transform.position = targetPosition;

            // Movement is now complete.
            IsMoving = false;

            // Tell the Animator to stop the walking animation.
            animator.SetBool("isMoving", IsMoving);
        }
        
        // After movement, check whether the player has triggered
        // a random encounter.
        CheckForEncounters();
        
    }


    // Attempts to interact with an object or NPC in front of the player.
    private void interact()
    {
        // Get the direction the player is currently facing from
        // the Animator's face_X and face_Y values.
        var facingdir = new Vector3(
            animator.GetFloat("face_X"), 
            animator.GetFloat("face_Y")
        );

        // Calculate the position directly in front of the player.
        var interactPos = transform.position + facingdir;

        // This can be used to visually debug the interaction direction.
        // It is currently commented out, so it does not run.
        //Debug.DrawLine(transform.position, interactPos, Color.red, 1f);

        // Search for a collider directly in front of the player.
        // Only objects on the InteractableLayer are detected.
        var collider = Physics2D.OverlapCircle(
            interactPos,
            0.2f,
            InteractableLayer
        );

        // If an interactable object was found...
        if (collider != null)
        {
            // Try to get its Interactable component and call Interact().
            // The ?. prevents an error if the component does not exist.
            collider.GetComponent<Interactable>()?.Interact();
        }
    }


    // Checks whether the player is allowed to move onto a specific position.
    private bool Iswalkable(Vector3 position)
    {
        // Check for a collider at the destination position.
        // Both obstacles and interactable objects are treated as
        // locations the player cannot walk through.
        Collider2D obstacle = Physics2D.OverlapCircle(
            position, 
            0.2f, 
            obstacleLayer | InteractableLayer
        );

        // If an obstacle or interactable object was detected,
        // the position cannot be walked onto.
        if(obstacle != null)
        {
            return false;
        }

        // If nothing is blocking the position, allow movement.
        return true;
    }


    // Checks whether the player has triggered a random encounter.
    private void CheckForEncounters()
    {
        // If the player is not inside a battle zone,
        // no encounter check is performed.
        if(!in_battlezone) return;


        // Converts the detector's world position into the
        // corresponding coordinate on the battle tilemap.
        Vector3Int tilePosition = _battletile.WorldToCell(
            _tiledetector.transform.position
        );

        // Gets the tile located at the calculated tilemap position.
        // This determines whether the player is actually standing
        // on a battle tile.
        TileBase tile = _battletile.GetTile(tilePosition);

        // If there is no battle tile, or the player has already
        // checked this exact tile, do not perform another encounter check.
        if(tile == null || _currentTilePosition == (Vector3) tilePosition) return;

        // Store the current tile so another encounter cannot be
        // immediately triggered while remaining on the same tile.
        _currentTilePosition = (Vector3)tilePosition;

        // Generate a random number between 0 and 499.
        int randomNum = UnityEngine.Random.Range(0, 500);

        // Compare the random number against encounterNum.
        // If the random number is smaller, trigger an encounter.
        if(randomNum < encounterNum)
        {
            Debug.Log("Encounter!!");

            // Notify any scripts listening to the OnEncounter event.
            // GameManager uses this to start a random battle.
            OnEncounter?.Invoke();
        }
    }


    // Called automatically when the player enters a trigger collider.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check whether the object entered has the "battlezone" tag.
        if(other.CompareTag("battlezone"))
        {
            // The player is now inside a battle zone.
            in_battlezone = true;

            Debug.Log("entered battle zone");
        }
    }


    // Called automatically when the player leaves a trigger collider.
    private void OnTriggerExit2D(Collider2D other)
    {
        // Check whether the object exited has the "battlezone" tag.
        if(other.CompareTag("battlezone"))
        {
            // The player is no longer inside a battle zone.
            in_battlezone = false;

            Debug.Log("exited battle zone");

            // Reset the previous tile position to a location far away.
            // This ensures that when the player enters another battle zone,
            // the first tile they step onto can trigger an encounter check.
            _currentTilePosition = new Vector3(
                -100000, 
                -100000, 
                -100000
            );
        }
    }

    
}