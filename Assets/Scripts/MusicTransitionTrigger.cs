using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTransitionTrigger : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The music to trigger when the player enters.")]
    private MusicManager.CurrentMusic musicToTrigger;

    [SerializeField]
    [Tooltip("The MusicManager in the scene.")]
    private MusicManager musicManager;

    [SerializeField]
    [Tooltip("The player tag.")]
    private string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == playerTag)
            musicManager.SetMusicOnTriggerEnter(musicToTrigger);
    }
}
