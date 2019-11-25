using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Cinemachine;
using System;

public class VignetteTrigger : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The post processing profile to set when the player enters.")]
    private PostProcessProfile postProcessProfile;

    [SerializeField]
    [Tooltip("The intensity of the vignette to set.")]
    private float vignetteIntensity = 0f;

    [SerializeField]
    [Tooltip("How close does the intensity need to be to stop interpolating?")]
    private float intensityMargin = 0.05f;

    [SerializeField]
    [Tooltip("How long should the vignette change take (seconds)?")]
    private float vignetteInterpTime = 1.5f;

    [SerializeField]
    [Tooltip("The player tag.")]
    private string playerTag = "Player";

    /// <summary>
    /// Should the vignette value change?
    /// </summary>
    private bool shouldInterpVignette = false;

    /// <summary>
    /// The Cinemachine virtual camera in the scene
    /// </summary>
    private CinemachineVirtualCamera virtualCamera;

    /// <summary>
    /// The post processing extension to add to the virtual camera
    /// </summary>
    private Cinemachine.PostFX.CinemachinePostProcessing postProcessingExtension;

    /// <summary>
    /// The vignette in the post process profile
    /// </summary>
    private Vignette vignette;

    private static event Action StopInterpolation;

    private void Awake()
    {
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
    }

    private void OnEnable()
    {
        StopInterpolation += StopInterp;
    }

    private void OnDisable()
    {
        StopInterpolation -= StopInterp;
    }

    private void Update()
    {
        if (shouldInterpVignette)
            InterpVignette();
    }

    private void InterpVignette()
    {
        if(Mathf.Abs(vignette.intensity.value - vignetteIntensity) > intensityMargin)
            vignette.intensity.Interp(vignette.intensity.value, vignetteIntensity, 1.5f*Time.deltaTime);
        else
        {
            vignette.intensity.value = vignetteIntensity;
            shouldInterpVignette = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == playerTag)
        {
            postProcessingExtension = virtualCamera.gameObject.GetComponent<Cinemachine.PostFX.CinemachinePostProcessing>();
            if(postProcessingExtension == null)
                postProcessingExtension = virtualCamera.gameObject.AddComponent<Cinemachine.PostFX.CinemachinePostProcessing>();
            postProcessingExtension.m_Profile = postProcessProfile;
            vignette = postProcessProfile.GetSetting<Vignette>();
            if (StopInterpolation != null)
                StopInterpolation.Invoke();
            shouldInterpVignette = true;
        }
    }

    private void StopInterp()
    {
        shouldInterpVignette = false;
    }
}
