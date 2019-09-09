using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    #region SerializeFields
    [SerializeField]
    [Tooltip("Duration, in seconds, of a slow movement (through sand, etc.).")]
    private float slowMoveTime = .75f;
    [SerializeField]
    [Tooltip("Duration, in seconds, of a single movement.")]
    private float moveTime = .5f;
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
    /// Defines directions in an enum for legible array reference
    /// </summary>
    private enum directions { Down, Up, Left, Right};

    /// <summary>
    /// The animator attached to the player
    /// </summary>
    private Animator playerAnimator;
    /// <summary>
    /// Is the player allowed to move? Will be false if paused, in a cutscene, etc.
    /// </summary>
    private bool canMove = true;
    /// <summary>
    /// Has the player received input to move?
    /// </summary>
    private bool hasReceivedMovementInput;
    /// <summary>
    /// Is the player in slow terrain?
    /// </summary>
    private bool isInSlowTerrain;
    /// <summary>
    /// Is the movement coroutine currently running?
    /// </summary>
    private bool isMoving;
    /// <summary>
    /// Stores movement input
    /// </summary>
    private Vector2 movementInput;
    /// <summary>
    /// Movement input normalized for easier use
    /// </summary>
    private Vector2 normalizedMovementDirection;

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
        if (canMove && hasReceivedMovementInput && !isMoving)
            StartCoroutine(MovementRoutine());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ToggleSlowMovement(collision);
    }

    /// <summary>
    /// Toggles slow movement when crossing the border of slow terrain
    /// </summary>
    /// <param name="collision">The trigger collider the player crossed</param>
    private void ToggleSlowMovement(Collider2D collision)
    {
        if (collision.gameObject.tag == slowTerrainTag)
            isInSlowTerrain = !isInSlowTerrain;
    }

    /// <summary>
    /// Gets player input
    /// </summary>
    private void GetInput()
    {
        movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (movementInput.magnitude > 0)
            hasReceivedMovementInput = true;
        else
            hasReceivedMovementInput = false;
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
    /// <param name="movementDirection">What direction is the player moving/facing?</param>
    /// <param name="slowTerrain">Is the player in slow terrain?</param>
    /// <param name="cannotMove">Is the player able to move?</param>
    private void PlayAnim(directions movementDirection, bool slowTerrain, bool cannotMove)
    {
        playerAnimator.SetBool("Slow", slowTerrain);
        playerAnimator.SetBool("Idle", cannotMove);
        switch (movementDirection)
        {
            case directions.Down:
                playerAnimator.SetTrigger("Move Down");
                break;
            case directions.Up:
                playerAnimator.SetTrigger("Move Up");
                break;
            case directions.Left:
                playerAnimator.SetTrigger("Move Left");
                break;
            case directions.Right:
                playerAnimator.SetTrigger("Move Right");
                break;
        }
    }

    /// <summary>
    /// Raycasts for collision in the current direction before moving
    /// </summary>
    /// <returns>True if there is something in the way, else false.</returns>
    private bool IsCollisionBlockingMovement(directions attemptedMoveDirection)
    {
        RaycastHit2D hit = Physics2D.Raycast(collisionRaycastOrigin.position, normalizedMovementDirection,1f, collisionLayerMask);
        return hit.collider != null;
    }

    /// <summary>
    /// Raycasts for slow terrain in the current direction before moving
    /// </summary>
    /// <returns>True if there is slow terrain in the way, else false.</returns>
    private bool IsEnteringSlowTerrain(directions attemptedMoveDirection)
    {
        RaycastHit2D hit = Physics2D.Raycast(collisionRaycastOrigin.position, normalizedMovementDirection, 1f, slowTerrainLayerMask);
        return hit.collider != null;
    }

    /// <summary>
    /// Removes the less influential movement axis and normalizes what's left
    /// </summary>
    private void NormalizeMovementVector()
    {
        normalizedMovementDirection = movementInput;
        if (Mathf.Abs(normalizedMovementDirection.x) >= Math.Abs(normalizedMovementDirection.y))
            normalizedMovementDirection.y = 0;
        else
            normalizedMovementDirection.x = 0;
        normalizedMovementDirection = normalizedMovementDirection.normalized;
    }

    /// <summary>
    /// Checks normalized movement and returns a direction used to activate player animations
    /// </summary>
    /// <returns></returns>
    private directions GetDirectionFromNormalizedMovement()
    {
        if (normalizedMovementDirection == Vector2.down)
            return directions.Down;
        if (normalizedMovementDirection == Vector2.up)
            return directions.Up;
        if (normalizedMovementDirection == Vector2.left)
            return directions.Left;
        else
            return directions.Right;
    }

    /// <summary>
    /// Move the player in a set interval aligned with the tilemap
    /// </summary>
    /// <returns></returns>
    private IEnumerator MovementRoutine()
    {
        isMoving = true;
        NormalizeMovementVector();
        directions movementDirection = GetDirectionFromNormalizedMovement();
        if (IsCollisionBlockingMovement(movementDirection))
        {
            // TODO Play bump audio
            PlayAnim(movementDirection, false, true);
        }
        else
        {
            Vector3 targetPosition = transform.position + new Vector3(normalizedMovementDirection.x, normalizedMovementDirection.y);
            float startTime = Time.time;
            if (IsEnteringSlowTerrain(movementDirection) || isInSlowTerrain)
            {
                PlayAnim(movementDirection, true, false);
                while (Vector3.Distance(targetPosition, transform.position) > movementMargin)
                {
                    transform.position = Vector3.Lerp(transform.position, targetPosition, (Time.time - startTime) / slowMoveTime);
                    yield return null;
                }
            }
            else
            {
                PlayAnim(movementDirection, false, false);
                while (Vector3.Distance(targetPosition, transform.position) > movementMargin)
                {
                    transform.position = Vector3.Lerp(transform.position, targetPosition, (Time.time - startTime) / moveTime);
                    yield return null;
                }
            }
            SnapToGrid();
        }
        isMoving = false;
    }
}
