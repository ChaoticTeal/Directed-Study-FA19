using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    private enum SwitchColor { Yellow, Blue, Red, Green };

    [SerializeField]
    [Tooltip("List of audio clips the switch can play." +
        "The first is played when it turns on, the second when it turns off.")]
    private List<AudioClip> switchClips;

    [SerializeField]
    [Tooltip("The color of the switch.")]
    private SwitchColor switchColor;

    [SerializeField]
    [Tooltip("The puzzle this switch is connected to.\n" +
        "Must have a script which implements the IPuzzle interface.")]
    private GameObject connectedPuzzle;

    [SerializeField]
    [Tooltip("The character added to the puzzle's alphanumeric " +
        "solution when the switch is activated.")]
    private char solutionEntry = '1';

    [SerializeField]
    [Tooltip("The name of the Animator bool for switch color.")]
    private string animColorBool = "Color";

    [SerializeField]
    [Tooltip("The name of the Animator bool for switch on.")]
    private string animOnBool = "On";

    /// <summary>
    /// The animator component attached to the switch
    /// </summary>
    private Animator animator;

    /// <summary>
    /// The current audio clip to play
    /// </summary>
    private AudioClip currentClip;

    /// <summary>
    /// The audio source component attached to the switch
    /// </summary>
    private AudioSource audioSource;

    /// <summary>
    /// Is the switch active or not?
    /// The player can only use it if it is not already active.
    /// </summary>
    private bool isSwitchActive_useProperty;

    /// <summary>
    /// The IPuzzle component of the connected puzzle
    /// </summary>
    private IPuzzle puzzle;

    /// <summary>
    /// Property to access current switch state
    /// </summary>
    private bool IsSwitchActive
    {
        get { return isSwitchActive_useProperty; }
        set
        {
            isSwitchActive_useProperty = value;
            if (isSwitchActive_useProperty)
                puzzle.AppendSolution(solutionEntry);
            animator.SetBool(Animator.StringToHash(animOnBool),
                isSwitchActive_useProperty);
            if(audioSource != null)
            {
                currentClip = isSwitchActive_useProperty ? switchClips[0] : switchClips[1];
                audioSource.PlayOneShot(currentClip);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetInteger(Animator.StringToHash(animColorBool),
            (int)switchColor);
        puzzle = connectedPuzzle.GetComponent<IPuzzle>();
        puzzle.AppendSwitchList(this);
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Handles input from the Usable component's OnUse event
    /// The switch can only be manually activated if it is not already active
    /// </summary>
    public void OnUse()
    {
        if (!IsSwitchActive)
        {
            IsSwitchActive = true;
        }
    }

    /// <summary>
    /// Handles Reset from the connected puzzle
    /// The switch can only be deactivated if it is already active
    /// </summary>
    public void ResetByPuzzle()
    {
        if (IsSwitchActive)
        {
            IsSwitchActive = false;
        }
    }
}
