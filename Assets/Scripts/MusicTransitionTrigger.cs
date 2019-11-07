using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTransitionTrigger : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The music to trigger when the player enters.")]
    private MusicManager.MusicType musicToTrigger;

    [SerializeField]
    [Tooltip("The player tag.")]
    private string playerTag = "Player";

    /// <summary>
    /// The MusicManager in the scene
    /// </summary>
    private MusicManager musicManager;

    private void Awake()
    {
        musicManager = FindObjectOfType<MusicManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == playerTag)
            musicManager.CurrentMusic = musicToTrigger;
    }
}
