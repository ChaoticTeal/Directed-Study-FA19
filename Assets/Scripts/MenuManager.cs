using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Provides functions for the buttons on the title screen
/// </summary>
public class MenuManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The panel for the main menu.")]
    private GameObject mainMenuPanel;

    [SerializeField]
    [Tooltip("The panel for the credits.")]
    private GameObject creditsPanel;

    [SerializeField]
    [Tooltip("The name of the next scene to load.")]
    private string nextScene;

    /// <summary>
    /// A button to select when changing panels
    /// </summary>
    private Selectable buttonToSelect;

    /// <summary>
    /// Loads the game
    /// </summary>
    public void StartButton()
    {
        if (nextScene != "")
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextScene);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    /// <summary>
    /// Quits the game
    /// </summary>
    public void QuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// Switches to the credits panel
    /// </summary>
    public void CreditsButton()
    {
        creditsPanel.SetActive(true);
        buttonToSelect = creditsPanel.GetComponentInChildren<Selectable>();
        buttonToSelect.Select();
        mainMenuPanel.SetActive(false);
    }

    /// <summary>
    /// Switches to the main menu panel
    /// </summary>
    public void BackButton()
    {
        mainMenuPanel.SetActive(true);
        buttonToSelect = mainMenuPanel.GetComponentInChildren<Selectable>();
        buttonToSelect.Select();
        creditsPanel.SetActive(false);
    }
}
