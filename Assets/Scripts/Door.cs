using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A door is a puzzle which, when solved, opens and allows the player to pass
/// </summary>
public class Door : Puzzle
{
    private enum DoorType { Wooden, Barred };

    [SerializeField]
    [Tooltip("Audio clip to play on door open.")]
    private AudioClip openingClip;

    [SerializeField]
    [Tooltip("The type of door to display.")]
    private DoorType doorType;
    
    /// <summary>
    /// The animator attached to the door
    /// </summary>
    private Animator animator;

    /// <summary>
    /// The audio source attached to the door
    /// </summary>
    private AudioSource audioSource;

    /// <summary>
    /// Stores a hashed reference to the Type variable in the animator
    /// </summary>
    private int animParamType = Animator.StringToHash("Type");

    /// <summary>
    /// Stores a hashed reference to the Open variable in the animator
    /// </summary>
    private int animParamOpen = Animator.StringToHash("Open");

    /// <summary>
    /// Play an audio clip and the door opening animation. Called by the 
    /// CurrentSolutionAttempt property when the solution is correct.
    /// </summary>
    protected override void DoPuzzleSolved()
    {
        animator.SetBool(animParamOpen, true);
        if (audioSource != null)
            audioSource.PlayOneShot(openingClip);
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetInteger(animParamType, (int)doorType);
        audioSource = GetComponent<AudioSource>();
    }
}
