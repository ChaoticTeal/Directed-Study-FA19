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
    private Transform collisionRaycastPoint;
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
    private bool shouldMove;
    /// <summary>
    /// Is the player in slow terrain?
    /// </summary>
    private bool slowMove;
    /// <summary>
    /// Is the movement coroutine currently running?
    /// </summary>
    private bool moving;
    /// <summary>
    /// Which direction is the player trying to move?
    /// </summary>
    private directions currentDirection;
    /// <summary>
    /// An array for quick reference of directions, in conjunction with the enum
    /// This prevents the need to write four separate coroutines
    /// </summary>
    private Vector3[] movementVectors;

    // Start is called before the first frame update
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        movementVectors = new Vector3[4] { Vector2.down, Vector2.up, Vector2.left, Vector2.right };
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        if (canMove && shouldMove && !moving)
            StartCoroutine(MovementRoutine());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == slowTerrainTag)
            slowMove = !slowMove;
    }

    /// <summary>
    /// Gets player input
    /// </summary>
    private void GetInput()
    {
        if (Input.GetAxis("Vertical") < 0)
        {
            shouldMove = true;
            currentDirection = directions.Down;
        }
        else if (Input.GetAxis("Vertical") > 0)
        {
            shouldMove = true;
            currentDirection = directions.Up;
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            shouldMove = true;
            currentDirection = directions.Left;
        }
        else if (Input.GetAxis("Horizontal") > 0)
        {
            shouldMove = true;
            currentDirection = directions.Right;
        }
        else
            shouldMove = false;
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


    private void PlayWalkAnim(directions movementDirection, bool slowTerrain)
    {
        playerAnimator.SetBool("Slow", slowTerrain);
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
    private bool CheckCollisionRaycast(directions attemptedMoveDirection)
    {
        RaycastHit2D hit = Physics2D.Raycast(collisionRaycastPoint.position, movementVectors[(int)attemptedMoveDirection],1f, collisionLayerMask);
        return hit.collider != null;
    }

    /// <summary>
    /// Raycasts for slow terrain in the current direction before moving
    /// </summary>
    /// <returns>True if there is slow terrain in the way, else false.</returns>
    private bool CheckSlowTerrainRaycast(directions attemptedMoveDirection)
    {
        RaycastHit2D hit = Physics2D.Raycast(collisionRaycastPoint.position, movementVectors[(int)attemptedMoveDirection], 1f, slowTerrainLayerMask);
        return hit.collider != null;
    }

    /// <summary>
    /// Move the player in a set interval aligned with the tilemap
    /// </summary>
    /// <returns></returns>
    private IEnumerator MovementRoutine()
    {
        if(shouldMove)
        {
            directions movementDirection = currentDirection;
            if (CheckCollisionRaycast(movementDirection))
            {
                // TODO Play bump audio
            }
            else
            {
                moving = true;
                Vector3 targetPosition = transform.position + movementVectors[(int)movementDirection];
                float startTime = Time.time;
                if (CheckSlowTerrainRaycast(movementDirection) || slowMove)
                {
                    PlayWalkAnim(movementDirection, true);
                    while (Vector3.Distance(targetPosition, transform.position) > movementMargin)
                    {
                        transform.position = Vector3.Lerp(transform.position, targetPosition, (Time.time - startTime) / slowMoveTime);
                        yield return null;
                    }
                }
                else
                {
                    PlayWalkAnim(movementDirection, false);
                    while (Vector3.Distance(targetPosition, transform.position) > movementMargin)
                    {
                        transform.position = Vector3.Lerp(transform.position, targetPosition, (Time.time - startTime) / moveTime);
                        yield return null;
                    }
                }
                SnapToGrid();
                moving = false;
            }
        }
    }
}
