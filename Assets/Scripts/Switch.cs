using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A Switch is one piece of activating a puzzle. When the player activates
/// the switch, it flips on and sends a character to the connected puzzle.
/// The switch will turn back off if it receives an event that the solution
/// was incorrect.
/// </summary>
public class Switch : MonoBehaviour
{
    private enum SwitchColor { Yellow, Blue, Red, Green };

    [SerializeField]
    [Tooltip("The audio clip played when the switch turns on.")]
    private AudioClip switchOnClip;

    [SerializeField]
    [Tooltip("The audio clip played when the switch turns off.")]
    private AudioClip switchOffClip;

    [SerializeField]
    [Tooltip("The color of the switch.")]
    private SwitchColor switchColor;

    [SerializeField]
    [Tooltip("The puzzle this switch is connected to.")]
    private Puzzle connectedPuzzle;

    [SerializeField]
    [Tooltip("The character added to the puzzle's alphanumeric " +
        "solution when the switch is activated.")]
    private char solutionEntry = '1';

    /// <summary>
    /// The animator component attached to the switch
    /// </summary>
    private Animator animator;

    /// <summary>
    /// The audio source component attached to the switch
    /// </summary>
    private AudioSource audioSource;

    /// <summary>
    /// Is the switch active or not?
    /// The player can only use it if it is not already active.
    /// </summary>
    private bool isSwitchOn_useProperty;

    /// <summary>
    /// Stores a hashed reference to the Color variable in the animator
    /// </summary>
    private int animParamColor = Animator.StringToHash("Color");

    /// <summary>
    /// Stores a hashed reference to the On variable in the animator
    /// </summary>
    private int animParamOn = Animator.StringToHash("On");

    /// <summary>
    /// Property to access current switch state. When on, the switch aims right.
    /// If the switch is turned on, it adds to the puzzle's current solution
    /// attempt. Whenever it's changed, it animates and plays audio.
    /// </summary>
    private bool IsSwitchOn
    {
        get { return isSwitchOn_useProperty; }
        set
        {
            isSwitchOn_useProperty = value;
            if (IsSwitchOn)
                connectedPuzzle.CurrentSolutionAttempt += solutionEntry;
            animator.SetBool(animParamOn, IsSwitchOn);
            if(audioSource != null)
            {
                if (IsSwitchOn)
                    audioSource.PlayOneShot(switchOnClip);
                else
                    audioSource.PlayOneShot(switchOffClip);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetInteger(animParamColor, (int)switchColor);
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        Puzzle.IncorrectSolutionAttempted += OnAttemptWasIncorrect;
    }

    private void OnDisable()
    {
        Puzzle.IncorrectSolutionAttempted -= OnAttemptWasIncorrect;
    }

    /// <summary>
    /// Handles input from the Usable component's OnUse event
    /// The switch can only be manually activated if it is not already active
    /// </summary>
    public void OnUse()
    {
        if (!IsSwitchOn)
        {
            IsSwitchOn = true;
        }
    }

    /// <summary>
    /// Handles the AttemptWasIncorrect event from the connected puzzle
    /// The switch can only be deactivated if it is already active
    /// </summary>
    public void OnAttemptWasIncorrect(Puzzle puzzle)
    {
        if (IsSwitchOn && puzzle == connectedPuzzle)
        {
            IsSwitchOn = false;
        }
    }
}
