using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Moves an object on the title screen for the camera to follow
/// </summary>
public class TitleCameraMovement : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Object movement speed.")]
    private float moveSpeed = 1;

    [SerializeField]
    [Tooltip("Maximum difference between X and Y velocity.")]
    private float maximumDirectionalDifference = 0.75f;

    [SerializeField]
    [Tooltip("How far from the center does a collision need to be to change direction?")]
    private float distanceMargin = 0.8f;

    /// <summary>
    /// The rigidbody attached to the object
    /// </summary>
    private Rigidbody2D rigidBody;

    /// <summary>
    /// The circle collider attached to the object
    /// </summary>
    private CircleCollider2D circleCollider;

    /// <summary>
    /// How far are the sides of the collider from its center?
    /// </summary>
    private float colliderRadius;

    /// <summary>
    /// Velocity of the object
    /// </summary>
    private Vector2 objectVelocity;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        colliderRadius = circleCollider.radius * distanceMargin;
        objectVelocity = GetRandomStartingVelocity() * moveSpeed;
        rigidBody.velocity = objectVelocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ReflectVelocity(collision.GetContact(0).point);
    }

    /// <summary>
    /// Reflect the object's velocity based on the relative location of collision.
    /// </summary>
    /// <param name="contactPoint">The point at which the object collided with another object.</param>
    private void ReflectVelocity(Vector2 contactPoint)
    {
        Vector2 difference = contactPoint - new Vector2(transform.position.x, transform.position.y);
        if (Mathf.Abs(difference.x) >= colliderRadius)
            objectVelocity.x = -objectVelocity.x;
        if (Mathf.Abs(difference.y) >= colliderRadius)
            objectVelocity.y = -objectVelocity.y;
        rigidBody.velocity = objectVelocity;
    }

    /// <summary>
    /// Generates a random, normalized vector for starting velocity
    /// </summary>
    /// <returns>A normalized Vector2 for starting velocity</returns>
    private Vector2 GetRandomStartingVelocity()
    {
        float x = Random.value;
        float y = Random.value;
        if(Mathf.Abs(x - y) > maximumDirectionalDifference)
        {
            if(x > y)
            {
                x -= y;
                y += y;
            }
            else
            {
                y -= x;
                x += x;
            }
        }
        return new Vector2(x, y).normalized;
    }
}
