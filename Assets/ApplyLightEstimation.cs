using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ApplyLightEstimation : MonoBehaviour
{
    [SerializeField] ARCameraManager cameraManager;
    Light lightReference;
    ARLightEstimationData lightEstimation;

    void Awake()
    {
        lightReference = GetComponent<Light>();
    }

    void OnEnable()
    {
        if (cameraManager != null)
        {
            cameraManager.frameReceived += FrameChanged;
        }
    }

    void OnDisable()
    {
        if (cameraManager != null)
        {
            cameraManager.frameReceived -= FrameChanged;
        }
    }

    void FrameChanged(ARCameraFrameEventArgs args)
    {
       lightEstimation = args.lightEstimation;
       if (lightEstimation.mainLightDirection != null)
       {
           lightReference.transform.position = lightEstimation.mainLightDirection.Value;
       }
       if (lightEstimation.mainLightIntensityLumens != null)
       {
           lightReference.intensity = lightEstimation.mainLightIntensityLumens.Value;
       }
       if (lightEstimation.mainLightColor != null)
       {
           lightReference.color = lightEstimation.mainLightColor.Value;
       }
    }
}
