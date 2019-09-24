using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IPuzzle
{
    private enum DoorType { Wooden, Barred };

    [SerializeField]
    [Tooltip("Audio clip to play on door open.")]
    private AudioClip openingClip;

    [SerializeField]
    [Tooltip("The type of door to display.")]
    private DoorType doorType;

    [SerializeField]
    [Tooltip("The solution to the puzzle.\n" +
        "This should be a combination of the connected switches' entries.")]
    private string solution = "1";

    [SerializeField]
    [Tooltip("The name of the Animator bool for door type.")]
    private string animTypeBool = "Type";

    [SerializeField]
    [Tooltip("The name of the Animator bool for door open.")]
    private string animOpenBool = "Open";
    
    /// <summary>
    /// The animator attached to the door
    /// </summary>
    private Animator animator;

    /// <summary>
    /// The audio source attached to the door
    /// </summary>
    private AudioSource audioSource;

    /// <summary>
    /// A list of all switches connected to the puzzle
    /// </summary>
    private List<Switch> connectedSwitches;

    /// <summary>
    /// The current attempted solution
    /// </summary>
    private string currentSolution_useProperty;

    /// <summary>
    /// Property to access the current solution
    /// Compares it to the intended solution when modified
    /// </summary>
    private string CurrentSolution
    {
        get { return currentSolution_useProperty; }
        set
        {
            currentSolution_useProperty = value;
            if (DoesSolutionMatch())
            {
                if (CurrentSolution.Length == solution.Length)
                    OpenDoor();
            }
            else
            {
                currentSolution_useProperty = "";
                ResetSwitches();
            }
        }
    }

    /// <summary>
    /// Play an audio clip and the door opening animation
    /// </summary>
    private void OpenDoor()
    {
        animator.SetBool(Animator.StringToHash(animOpenBool), true);
        if (audioSource != null)
            audioSource.PlayOneShot(openingClip);
    }

    private void Awake()
    {
        connectedSwitches = new List<Switch>();
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetInteger(Animator.StringToHash(animTypeBool),
            (int)doorType);
        audioSource = GetComponent<AudioSource>();
    }

    // Functionality matches that described in IPuzzle
    public void AppendSolution(char entry)
    {
        CurrentSolution += entry;
    }

    // Functionality matches that described in IPuzzle
    public void AppendSwitchList(Switch addedSwitch)
    {
        connectedSwitches.Add(addedSwitch);
    }

    /// <summary>
    /// Calls the ResetSwitch function in each of the connected switches
    /// </summary>
    private void ResetSwitches()
    {
        foreach (Switch s in connectedSwitches)
            s.ResetByPuzzle();
    }

    /// <summary>
    /// Compares the attempted solution to the actual solution
    /// </summary>
    /// <returns>True if the solutions match up to the current length, false otherwise</returns>
    private bool DoesSolutionMatch()
    {
        string subSolution = solution.Substring(0, CurrentSolution.Length);
        return CurrentSolution == subSolution;
    }
}
