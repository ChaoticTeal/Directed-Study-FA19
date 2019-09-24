using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class automatically selects the attached selectable if nothing else 
/// is selected. Should be attached to an object with a Selectable component
/// </summary>
public class AutoSelectButton : MonoBehaviour
{
    /// <summary>
    /// The Selectable component of this GameObject
    /// </summary>
    private UnityEngine.UI.Selectable selectable;

    /// <summary>
    /// The scene's event system
    /// </summary>
    private UnityEngine.EventSystems.EventSystem eventSystem;

    // Start is called before the first frame update
    void Awake()
    {
        eventSystem = GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        selectable = GetComponent<UnityEngine.UI.Selectable>();
    }

    private void OnEnable()
    {
        if (eventSystem.currentSelectedGameObject == null)
            selectable.Select();
    }
}
