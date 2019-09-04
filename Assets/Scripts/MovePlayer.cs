using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    #region SerializeFields
    [SerializeField]
    [Tooltip("How fast the player moves through sand and other\nslow terrain compared to ordinary terrain.")]
    private float sandSpeedMultiplier = .5f;
    [SerializeField]
    [Tooltip("Duration, in seconds, of a single movement.")]
    private float moveTime = .25f;
    #endregion

    private enum directions { Down, Up, Left, Right};

    private bool canMove = true;
    private bool shouldMove;
    private bool moving;
    private directions currentDirection;
    private Vector2[] movementVectors;

    // Start is called before the first frame update
    void Start()
    {
        movementVectors = new Vector2[4] { Vector2.down, Vector2.up, Vector2.left, Vector2.right };
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        if (canMove && shouldMove && !moving)
            StartCoroutine(MovePlayer());
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
    }

    /// <summary>
    /// Adjusts the player to fit exactly into a tile
    /// </summary>
    void SnapToGrid()
    {
        int positionX = Mathf.RoundToInt(transform.position.x);
        int positionY = Mathf.RoundToInt(transform.position.y);
        transform.position = new Vector3(positionX, positionY);
    }

    private IEnumerator MovePlayer()
    {
        if(shouldMove)
        {
            directions movementDirection = currentDirection;
            moving = true;
            
        }
    }
}
