using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The color of the switch. 0 if yellow, 1 if blue, 2 if red, 3 if green.")]
    [Range(0, 3)]
    private int switchColor;

    [SerializeField]
    [Tooltip("The puzzle this switch is connected to.\nMust have a script which implements the IPuzzle interface.")]
    private GameObject connectedPuzzle;

    [SerializeField]
    [Tooltip("The character this switch adds to the solution.")]
    private char solutionEntry = '1';

    /// <summary>
    /// The animator component attached to the switch
    /// </summary>
    private Animator animator;

    /// <summary>
    /// Is the switch active or not?
    /// </summary>
    private bool isSwitchOn;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetInteger("Color", switchColor);
    }

    /// <summary>
    /// Sends the switch's solution to the puzzle and turns it on
    /// Called by OnUse from the Dialogue System Usable component
    /// </summary>
    public void SetSwitch()
    {
        if (!isSwitchOn)
        {
            AnimateSwitch();
            connectedPuzzle.GetComponent<IPuzzle>().AppendSolution(solutionEntry);
        }
    }

    /// <summary>
    /// Resets the switch to off
    /// Called by Reset from the connected puzzle
    /// </summary>
    public void ResetSwitch()
    {
        if (isSwitchOn)
        {
            AnimateSwitch();
        }
    }

    /// <summary>
    /// Store the switch state and trigger animations
    /// </summary>
    private void AnimateSwitch()
    {
        isSwitchOn = !isSwitchOn;
        animator.SetBool("On", isSwitchOn);
    }
}
