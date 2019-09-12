using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    #region SerializeFields
    [SerializeField]
    [Tooltip("Duration, in seconds, of a slow movement (through sand, etc.).")]
    private float slowMoveDuration = .75f;
    [SerializeField]
    [Tooltip("Duration, in seconds, of a single movement.")]
    private float normalMoveDuration = .5f;
    [SerializeField]
    [Tooltip("Threshold to stop movement coroutine and snap to position.")]
    private float movementMargin = .05f;
    [SerializeField]
    [Tooltip("Point from which to raycast to check collision before moving.")]
    private Transform collisionRaycastOrigin;
    [SerializeField]
    [Tooltip("Layer(s) the player collides with when moving.")]
    private LayerMask collisionLayerMask;
    [SerializeField]
    [Tooltip("Layer that slows the player down when moving.")]
    private LayerMask slowTerrainLayerMask;
    [SerializeField]
    [Tooltip("Tag of terrain that slows the player down when moving.")]
    private string slowTerrainTag;
    #endregion
    
    /// <summary>
    /// The animator attached to the player
    /// </summary>
    private Animator playerAnimator;
    /// <summary>
    /// Is the player in slow terrain?
    /// </summary>
    private bool isInSlowTerrain;
    /// <summary>
    /// Is the movement coroutine currently running?
    /// </summary>
    private bool isMoving;
    /// <summary>
    /// Is something in the way of movement?
    /// </summary>
    private bool isMovementBlocked;
    /// <summary>
    /// At what time did movement start?
    /// </summary>
    private float movementStartTime;
    /// <summary>
    /// Stores movement input
    /// </summary>
    private Vector2 movementInput;
    /// <summary>
    /// Stores the intended movement direction aligned to the X or Y axisd
    /// </summary>
    private Vector2 desiredMovementDirection;
    /// <summary>
    /// The intended move position
    /// </summary>
    private Vector2 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        bool hasReceivedMovementInput = movementInput.magnitude > 0;
        if (hasReceivedMovementInput && !isMoving)
            StartMoving();
        if (isMoving)
        {
            if (Vector3.Distance(targetPosition, transform.position) > movementMargin)
            {
                float chosenMovementDuration = isInSlowTerrain ? slowMoveDuration : normalMoveDuration;
                Move(chosenMovementDuration);
            }
            else
                CleanUpMovement();
        }
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    ToggleSlowMovement(collision);
    //}

    /// <summary>
    /// Set variables and begin movement
    /// </summary>
    private void StartMoving()
    {
        desiredMovementDirection = GetOneDimensionalMovementVector();
        isMovementBlocked = IsCollisionBlockingMovement(desiredMovementDirection);
        //isInSlowTerrain = IsEnteringSlowTerrain(desiredMovementDirection) || isInSlowTerrain;
        PlayAnim(desiredMovementDirection, isInSlowTerrain, isMovementBlocked);
        movementStartTime = Time.time;
        targetPosition = GetTargetPosition();
        if(!isMovementBlocked)
            isMoving = true;
    }

    /// <summary>
    /// Snap the player to the grid and allow movement to begin again
    /// </summary>
    private void CleanUpMovement()
    {
        SnapToGrid();
        isMoving = false;
    }

    /// <summary>
    /// Move the player toward the target position at a specified length of time
    /// </summary>
    /// <param name="movementDurationInSeconds">How long should the movement take, from start to finish?</param>
    private void Move(float movementDurationInSeconds)
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, 
            (Time.time - movementStartTime) / movementDurationInSeconds);
    }

    /// <summary>
    /// Calculates the desired position based on movement direction
    /// Target position will always be one unit away from the current position
    /// </summary>
    /// <returns>The desired end position of movement</returns>
    private Vector2 GetTargetPosition()
    {
        return (Vector2)transform.position + desiredMovementDirection;
    }

    ///// <summary>
    ///// Toggles slow movement when crossing the border of slow terrain
    ///// </summary>
    ///// <param name="collision">The trigger collider the player crossed</param>
    //private void ToggleSlowMovement(Collider2D collision)
    //{
    //    if (collision.gameObject.tag == slowTerrainTag)
    //        isInSlowTerrain = !isInSlowTerrain;
    //}

    /// <summary>
    /// Gets player input
    /// </summary>
    private void GetInput()
    {
        movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    /// <summary>
    /// Adjusts the player to fit exactly into a tile
    /// </summary>
    private void SnapToGrid()
    {
        int positionX = Mathf.RoundToInt(transform.position.x);
        int positionY = Mathf.RoundToInt(transform.position.y);
        transform.position = new Vector3(positionX, positionY);
    }

    /// <summary>
    /// Triggers the player's animations based on movement direction
    /// </summary>
    /// <param name="movementVector">What direction is the player moving/facing?</param>
    /// <param name="slowTerrain">Is the player in slow terrain?</param>
    /// <param name="isMovementBlocked">Is the player able to move?</param>
    private void PlayAnim(Vector2 movementVector, bool slowTerrain, bool isMovementBlocked)
    {
        playerAnimator.SetBool("Slow", slowTerrain);
        playerAnimator.SetBool("Idle", isMovementBlocked);
        if (movementVector == Vector2.down)
            playerAnimator.SetTrigger("Move Down");
        else if (movementVector == Vector2.up)
                playerAnimator.SetTrigger("Move Up");
        else if (movementVector == Vector2.left)
            playerAnimator.SetTrigger("Move Left");
        else if(movementVector == Vector2.right)
            playerAnimator.SetTrigger("Move Right");
    }

    /// <summary>
    /// Raycasts for collision in the current direction before moving
    /// </summary>
    /// <returns>True if there is something in the way, else false.</returns>
    private bool IsCollisionBlockingMovement(Vector2 movementVector)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            collisionRaycastOrigin.position, movementVector,
            1f, collisionLayerMask);
        return hit.collider != null;
    }

    /// <summary>
    /// Raycasts for slow terrain in the current direction before moving
    /// </summary>
    /// <returns>True if there is slow terrain in the way, else false.</returns>
    private bool IsEnteringSlowTerrain(Vector2 movementVector)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            collisionRaycastOrigin.position, movementVector, 
            1f, slowTerrainLayerMask);
        return hit.collider != null;
    }

    /// <summary>
    /// Removes the less influential movement axis and normalizes what's left
    /// </summary>
    private Vector2 GetOneDimensionalMovementVector()
    {
        Vector2 normalizedMovementDirection = movementInput;
        if (Mathf.Abs(normalizedMovementDirection.x) >= Math.Abs(normalizedMovementDirection.y))
            normalizedMovementDirection.y = 0;
        else
            normalizedMovementDirection.x = 0;
        return normalizedMovementDirection.normalized;
    }
}
