using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Door : MonoBehaviour, IPuzzle
{
    [SerializeField]
    [Tooltip("The type of door to display. 0 for wooden, 1 for barred.")]
    [Range(0, 1)]
    private int doorType;

    [SerializeField]
    [Tooltip("The solution to the puzzle.\nThis should be a string comprised of the connected switches' entry.")]
    private string solution = "1";
    
    /// <summary>
    /// The animator attached to the door
    /// </summary>
    private Animator animator;

    /// <summary>
    /// A list of all switches connected to the puzzle
    /// </summary>
    private List<Switch> connectedSwitches;

    /// <summary>
    /// The current attempted solution
    /// </summary>
    private string _currentSolution;

    /// <summary>
    /// Property to access the current solution
    /// Compares it to the intended solution when modified
    /// </summary>
    private string CurrentSolution
    {
        get { return _currentSolution; }
        set
        {
            _currentSolution = value;
            if (DoesSolutionMatch())
            {
                if (CurrentSolution.Length == solution.Length)
                    animator.SetBool("Open", true);
            }
            else
            {
                _currentSolution = "";
                ResetSwitches();
            }
        }
    }

    private void Awake()
    {
        connectedSwitches = new List<Switch>();
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetInteger("Type", doorType);
    }

    /// <summary>
    /// Add to the current solution
    /// </summary>
    /// <param name="entry">The solution piece connected to the activated switch</param>
    public void AppendSolution(char entry)
    {
        CurrentSolution += entry;
    }

    /// <summary>
    /// Adds a switch to the list of connected switches
    /// </summary>
    /// <param name="addedSwitch">The switch to add</param>
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
            s.ResetSwitch();
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
