using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateNPC : MonoBehaviour
{
    /// <summary>
    /// The NPC's Animator component
    /// </summary>
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Sets the NPC's animation direction to face the player initiating the conversation
    /// Called by the Dialogue System OnUse event
    /// </summary>
    /// <param name="playerTransform">The location of the player.</param>
    public void SetAnimationDirectionOnConversationStart(Transform playerTransform)
    {
        Vector2 playerDirection = GetDirectionFromNPCToPlayer(playerTransform.position);
        if (playerDirection == Vector2.down)
            animator.SetTrigger("Move Down");
        else if (playerDirection == Vector2.up)
            animator.SetTrigger("Move Up");
        else if (playerDirection == Vector2.left)
            animator.SetTrigger("Move Left");
        else if (playerDirection == Vector2.right)
            animator.SetTrigger("Move Right");

    }

    /// <summary>
    /// Creates a vector pointing in the nearest cardinal direction from the NPC to the player
    /// </summary>
    /// <returns>Returns a vector of magnitude 1 pointing up, down, left, or right</returns>
    private Vector2 GetDirectionFromNPCToPlayer(Vector2 playerPosition)
    {
        Vector2 normalizedPlayerDirection = playerPosition - (Vector2)transform.position;
        if (Mathf.Abs(normalizedPlayerDirection.x) >= Mathf.Abs(normalizedPlayerDirection.y))
            normalizedPlayerDirection.y = 0;
        else
            normalizedPlayerDirection.x = 0;
        return normalizedPlayerDirection.normalized;
    }
}
