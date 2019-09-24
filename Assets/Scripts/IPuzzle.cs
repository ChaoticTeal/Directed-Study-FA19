using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A puzzle is an object connected to a switch or group of switches.
/// It has a solution, a string formed from a combination of each switch's character.
/// When the player activates the switches in a sequence which matches this solution,
/// the puzzle's state changes to allow the player to progress.
/// </summary>
public interface IPuzzle
{
    /// <summary>
    /// Adds a character to the currently attempted solution.
    /// This is called when a switch connected to the puzzle is used
    /// </summary>
    /// <param name="entry">The character held by the switch.</param>
    void AppendSolution(char entry);

    /// <summary>
    /// Adds a switch to the list of connected switches
    /// </summary>
    /// <param name="addedSwitch">The switch to add</param>
    void AppendSwitchList(Switch addedSwitch);
}
