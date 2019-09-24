using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSelectButton : MonoBehaviour
{
    /// <summary>
    /// The Selectable component of this GameObject
    /// </summary>
    private UnityEngine.UI.Selectable selectable;

    private UnityEngine.EventSystems.EventSystem eventSystem;

    // Start is called before the first frame update
    void Start()
    {
        eventSystem = GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        selectable = GetComponent<UnityEngine.UI.Selectable>();
    }

    // Update is called once per frame
    void Update()
    {
        if (eventSystem.currentSelectedGameObject == null)
            selectable.Select();
    }
}
