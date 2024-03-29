﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using System;

/// <summary>
/// Manages the background music at runtime
/// </summary>
public class MusicManager : MonoBehaviour
{
    /// <summary>
    /// The current state of the town, based on quests completed
    /// Determines which of the four tracks to play
    /// </summary>
    private enum TownMusicState { Cursed, Dreary, Light, Cheery };

    /// <summary>
    /// Which theme should currently be playing?
    /// </summary>
    public enum MusicType { Town, Cave, Forest, Castle };

    [SerializeField]
    [Tooltip("Music to play while in Gratia proper.")]
    private List<AudioClip> townMusic;

    [SerializeField]
    [Tooltip("Quest completion threshold for each track.")]
    private List<int> townQuestThresholds;

    [SerializeField]
    [Tooltip("Music to play in the cave.")]
    private AudioClip caveMusic;

    [SerializeField]
    [Tooltip("Music to play in the forest.")]
    private AudioClip forestMusic;

    [SerializeField]
    [Tooltip("Music to play in the castle.")]
    private AudioClip castleMusic;

    [SerializeField]
    [Tooltip("Time to fade music in/out for track transition.")]
    private float musicFadeSeconds = 0.25f;

    [SerializeField]
    [Tooltip("How close to the target does the fade need to be to be complete?")]
    private float fadeMargin = 0.1f;

    /// <summary>
    /// How many quests has the player completed?
    /// Sets current town music state based on the answer
    /// </summary>
    private int QuestsCompleted
    {
        get { return questsCompleted_UseProperty; }
        set
        {
            questsCompleted_UseProperty = value;
            if (QuestsCompleted >= townQuestThresholds[(int)currentMusicState] &&
                currentMusicState < TownMusicState.Cheery)
            {
                BeginMusicFade();
                currentMusicState++;
            }
        }
    }

    public MusicType CurrentMusic
    {
        private get { return currentMusic_UseProperty; }
        set
        {
            if(value != currentMusic_UseProperty)
            {
                BeginMusicFade();
                currentMusic_UseProperty = value;
            }
        }
    }

    /// <summary>
    /// Standard int for use with property
    /// </summary>
    private int questsCompleted_UseProperty;

    /// <summary>
    /// CurrentMusic variable for use with property
    /// </summary>
    private MusicType currentMusic_UseProperty;

    /// <summary>
    /// The AudioSource component attached to the object
    /// </summary>
    private AudioSource audioSource;

    /// <summary>
    /// Is the volume currently changing?
    /// </summary>
    private bool isVolumeChanging;

    /// <summary>
    /// Is the music fading out?
    /// </summary>
    private bool isFadingOut;

    /// <summary>
    /// When did the fade start?
    /// </summary>
    private float fadeStartTime;

    /// <summary>
    /// The volume of the AudioSource, as set in the editor
    /// </summary>
    private float startingVolume;

    /// <summary>
    /// What is the current music state, based on quests completed?
    /// </summary>
    private TownMusicState currentMusicState = TownMusicState.Cursed;

    /// <summary>
    /// Quests completed variable (DialogueSystem)
    /// </summary>
    private string completedQuestsName = "QuestsCompleted";

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        startingVolume = audioSource.volume;
    }

    // Update is called once per frame
    void Update()
    {
        CheckQuestsCompleted();
        if (isVolumeChanging)
            FadeMusic();
    }

    /// <summary>
    /// Check the quests completeed variable from dialogue system and update it locally
    /// </summary>
    private void CheckQuestsCompleted()
    {
        int dialogueQuests = DialogueLua.GetVariable(completedQuestsName).AsInt;
        if (dialogueQuests > QuestsCompleted)
            QuestsCompleted = dialogueQuests;
    }

    /// <summary>
    /// Sets variables to start fading the music
    /// </summary>
    private void BeginMusicFade()
    {
        isVolumeChanging = true;
        isFadingOut = true;
        fadeStartTime = Time.time;
    }

    /// <summary>
    /// Fade the music in and out
    /// </summary>
    private void FadeMusic()
    {
        if (isFadingOut && audioSource.volume > fadeMargin)
            audioSource.volume = Mathf.Lerp(startingVolume, 0f, 
                (Time.time - fadeStartTime) / musicFadeSeconds);
        else
        {
            audioSource.volume = 0;
            SetNewTrack();
            if (isFadingOut)
                fadeStartTime = Time.time;
            isFadingOut = false;
            audioSource.volume = Mathf.Lerp(0f, startingVolume,
                (Time.time - fadeStartTime) / musicFadeSeconds);
            if(audioSource.volume >= startingVolume - fadeMargin)
            {
                audioSource.volume = startingVolume;
                isVolumeChanging = false;
            }
        }
    }

    /// <summary>
    /// Set the new music track based on variables
    /// </summary>
    private void SetNewTrack()
    {
        switch (CurrentMusic)
        {
            case MusicType.Town:
                audioSource.clip = townMusic[(int)currentMusicState];
                break;
            case MusicType.Cave:
                audioSource.clip = caveMusic;
                break;
            case MusicType.Forest:
                audioSource.clip = forestMusic;
                break;
            case MusicType.Castle:
                audioSource.clip = castleMusic;
                break;
        }
        audioSource.Play();
    }
}

