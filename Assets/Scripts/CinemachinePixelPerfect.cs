using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinemachinePixelPerfect : MonoBehaviour
{
    private Camera mainCamera;
    private Cinemachine.CinemachineVirtualCamera vCam;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = FindObjectOfType<Cinemachine.CinemachineBrain>().
            gameObject.GetComponent<Camera>();
        vCam = GetComponent<Cinemachine.CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        MaintainPixelPerfectSize();
    }

    /// <summary>
    /// Resets the virtual camera size to match the pixel perfect main camera
    /// </summary>
    private void MaintainPixelPerfectSize()
    {
        if (vCam.m_Lens.OrthographicSize != mainCamera.orthographicSize)
            vCam.m_Lens.OrthographicSize = mainCamera.orthographicSize;
    }
}
