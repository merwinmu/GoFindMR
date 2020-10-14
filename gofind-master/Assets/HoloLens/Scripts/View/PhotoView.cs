using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.WebCam;

namespace Assets.HoloLens.Scripts.View
{
    public class PhotoChangedEventArgs : EventArgs
    {
        public Texture Photo { get; private set; }
       
        
        public PhotoChangedEventArgs(Texture photo)
        {
            this.Photo = photo;
            
            //Debugging
            //Debug.Log("Received event from Temporal View");
        }
    }
    
    public interface IPhotoView
    {
        event EventHandler<PhotoChangedEventArgs> OnReceived;
    }
    
    public class PhotoView: MonoBehaviour,IPhotoView
    {
        public event EventHandler<PhotoChangedEventArgs> OnReceived;
        
        public PhotoCapture photoCaptureObject = null;
        public GameObject Result;
        public static Texture2D data;
        public static Queue<Vector3> vectorstack = new Queue<Vector3>(); 

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
        
        /*
         * https://docs.microsoft.com/en-us/windows/mixed-reality/develop/unity/locatable-camera-in-unity
         * Offical Microsoft Doc
         */

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

                //photoCaptureObject.TakePhotoAsync(filePath, PhotoCaptureFileOutputFormat.JPG, OnCapturedPhotoToDisk);
                photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
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
                
                Result  = Instantiate(Result, Camera.main.transform.position,Camera.main.transform.rotation);
                Result.GetComponent<Renderer>().material.mainTexture = targetTexture;

                // Do as we wish with the texture such as apply it to a material, etc.
                
                //Result.GetComponent<Renderer>().material.mainTexture = targetTexture;
                
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


        private string result;

        public string GetResult()
        {
            return result;
        }

        

        public const string DATA_URL_PREFIX = "data:";
        public const string IMAGE_PNG = "image/png;";
        public const string IMAGE_JPEG = "image/jpeg;";
        public const string DATA_URL_POST_IMAGE_SEQUENCE = "base64,";
        
        private void Update()
        {
            // If the primary mouse button was pressed this frame
            if (Input.GetMouseButtonDown(0))
            {
                //Debug
                var eventArgs = new PhotoChangedEventArgs(new Texture2D(200,200));
                OnReceived(this, eventArgs);
            }
        }
    }
    
}