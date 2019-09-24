using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A puzzle is an object connected to a switch or group of switches.
/// It has a solution, a string formed from a combination of each switch's character.
/// When the player activates the switches in a sequence which matches this solution,
/// the puzzle's state changes to allow the player to progress.
/// </summary>
public abstract class Puzzle : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The solution to the puzzle.\n This should be a combination " +
        "of the connected switches' solutionEntries.")]
    private string solution = "1";

    /// <summary>
    /// The current attempted solution
    /// </summary>
    private string currentSolutionAttempt_useProperty;

    /// <summary>
    /// Property to reference the current solution attempt. If the solution
    /// is completely correct, the puzzle performs its "solved" function.
    /// Otherwise, it resets the attempt and invokes SolutionIsIncorrect
    /// </summary>
    public virtual string CurrentSolutionAttempt
    {
        get { return currentSolutionAttempt_useProperty; }
        set
        {
            currentSolutionAttempt_useProperty = value;
            if (CheckIfSolutionMatchesSoFar())
            {
                if (CurrentSolutionAttempt.Length == solution.Length)
                    DoPuzzleSolved();
            }
            else
            {
                currentSolutionAttempt_useProperty = "";
                if (SolutionIsIncorrect != null)
                    SolutionIsIncorrect.Invoke(this);
            }
        }
    }

    /// <summary>
    /// An event to invoke when the solution attempt fails
    /// </summary>
    public static event System.Action<Puzzle> SolutionIsIncorrect;

    /// <summary>
    /// Compares the attempted solution to the actual solution
    /// </summary>
    /// <returns>True if the solutions match up to the current length, 
    /// false otherwise</returns>
    private bool CheckIfSolutionMatchesSoFar()
    {
        string subSolution = solution.Substring(0, CurrentSolutionAttempt.Length);
        return CurrentSolutionAttempt == subSolution;
    }

    /// <summary>
    /// Do what the puzzle should do upon solving the puzzle. As an abstract 
    /// function, the implementation will depend on the child class.
    /// </summary>
    protected abstract void DoPuzzleSolved();
}
