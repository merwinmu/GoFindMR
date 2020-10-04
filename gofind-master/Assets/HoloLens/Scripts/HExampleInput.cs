using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Modules.SimpleLogging;
using UnityEngine;
using UnityEngine.Windows.WebCam;


namespace Assets.HoloLens.Scripts
{
    public class HExampleInput : MonoBehaviour
    {
        public PhotoCapture photoCaptureObject = null;

        // Start is called before the first frame update
        public bool DebugStoreImage = true;
        public Action<string> PhotoCapturedHandler;


        public static string ToBase64DataUrl(Texture2D texture)
        {
            byte[] png = texture.EncodeToPNG();
            string data = Convert.ToBase64String(png);
            return PNG_DATA_PREFIX + data;
        }

        public const string PNG_DATA_PREFIX = DATA_URL_PREFIX + IMAGE_PNG + DATA_URL_POST_IMAGE_SEQUENCE;

        public static Texture2D FromBase64DataUrl(string data)
        {
            if (data.StartsWith(PNG_DATA_PREFIX))
            {
                data = data.Substring(PNG_DATA_PREFIX.Length);
            }

            byte[] png = Convert.FromBase64String(data);
            Texture2D texture = new Texture2D(100, 100);
            texture.LoadImage(png);
            return texture;
        }


        private Modules.SimpleLogging.Logger logger;

        // Use this for initialization
        void Start()
        {
        }

        public void TakePhoto()
        {
            PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
        }

        void OnPhotoCaptureCreated(PhotoCapture captureObject)
        {
            photoCaptureObject = captureObject;

            Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

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
                string filename = string.Format(@"CapturedImage{0}_n.jpg", Time.time);
                string filePath = System.IO.Path.Combine(Application.persistentDataPath, filename);

                photoCaptureObject.TakePhotoAsync(filePath, PhotoCaptureFileOutputFormat.JPG, OnCapturedPhotoToDisk);
            }
            else
            {
                Debug.LogError("Unable to start photo mode!");
            }
        }


        void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
        {
            if (result.success)
            {
                // Create our Texture2D for use and set the correct resolution
                Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
                Texture2D targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);
                // Copy the raw image data into our target texture
                photoCaptureFrame.UploadImageDataToTexture(targetTexture);
                // Do as we wish with the texture such as apply it to a material, etc.
                this.result = ToBase64DataUrl(targetTexture);
            }
            // Clean up
            photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
        }

        
        // ======= DEBUG STORE IMAGE =========

        void OnCapturedPhotoToDisk(PhotoCapture.PhotoCaptureResult result)
        {
            if (result.success)
            {
                Debug.Log("Saved Photo to disk!");
                photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
            }
            else
            {
                Debug.Log("Failed to save Photo to disk");
            }
        }

        void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
        {
            photoCaptureObject.Dispose();
            photoCaptureObject = null;
        }

        // Update is called once per frame
        void Update()
        {

        }


        private string result;

        public string GetResult()
        {
            return result;
        }

        

        public const string DATA_URL_PREFIX = "data:";
        public const string IMAGE_PNG = "image/png;";
        public const string IMAGE_JPEG = "image/jpeg;";
        public const string DATA_URL_POST_IMAGE_SEQUENCE = "base64,";
        
    }
}

