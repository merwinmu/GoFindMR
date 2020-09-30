using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Windows.WebCam;


public class HDeviceCam : MonoBehaviour
{
     PhotoCapture photoCaptureObject = null;

    bool isRunning = false;

    void Start()
    {
        StartCoroutine(StartCameraCapture());
    }

    public IEnumerator StartCameraCapture()
    {
        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        }
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            Debug.Log("Creating PhotoCapture");
            PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
        }
        else
        {
            Debug.Log("Webcam Permission not granted");
        }
    }

    private void Update()
    {
        if (isRunning)
        {
            photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
        }
    }

    void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        photoCaptureObject = captureObject;

        IEnumerable<Resolution> availableResolutions = PhotoCapture.SupportedResolutions;

        foreach (var res in availableResolutions)
        {
            Debug.Log("PhotoCapture Resolution: " + res.width + "x" + res.height);
        }

        Resolution cameraResolution = availableResolutions.OrderByDescending((res) => res.width * res.height).First();

        CameraParameters c = new CameraParameters();
        c.hologramOpacity = 0.0f;
        c.cameraResolutionWidth = cameraResolution.width;
        c.cameraResolutionHeight = cameraResolution.height;
        c.pixelFormat = CapturePixelFormat.BGRA32;

        captureObject.StartPhotoModeAsync(c, OnPhotoModeStarted);
    }

    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            isRunning = true;
            photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
        }
    }

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame frame)
    {
        if (result.success)
        {
            if (frame.TryGetCameraToWorldMatrix(out Matrix4x4 cameraToWorldMatrix))
            {
                Debug.Log("Successfully obtained CameraToWorldMatrix: " + cameraToWorldMatrix.ToString());

            }
            else
            {
                Debug.Log("Failed to obtain CameraToWorldMatrix");
            }
        }
        frame.Dispose();
    }
}
