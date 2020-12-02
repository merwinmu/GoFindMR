using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using UnityEngine.Windows.WebCam;

/*
 * Views are primarly used for Input and Output. It is primarly a Monobehaviour class with the associate functions 
 * Input actions such as OnClick invoke an event to the controller which then executes a function to model
 * Output actions are in example rendering gameobjects etc.
 */



/*
 * Various EventArgs has been created so that if an Input occurs , a callback can be
 * invoked to the controller which then sends it to the model
 */

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
    
    public class TakePhotoEventArgs : EventArgs
    {
    }
    
    public class BackEventArgs : EventArgs
    {
    }
    
    public interface IPhotoView
    {
        event EventHandler<PhotoChangedEventArgs> OnReceived;
        event EventHandler<TakePhotoEventArgs> OnTakePhoto;
        event EventHandler<BackEventArgs> OnBack;
        void MenuVisibility(bool flag);

    }
    
    public class PhotoView: MonoBehaviour,IPhotoView
    {
        public event EventHandler<PhotoChangedEventArgs> OnReceived = (sender, e) => { };
        public event EventHandler<TakePhotoEventArgs> OnTakePhoto = (sender, e) => { };
        public event EventHandler<BackEventArgs> OnBack = (sender, e) => { };

        
        
        private bool MainMenuStatus;
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
            this.transform.gameObject.SetActive(false);
            
            take_photo_button = transform.GetChild(2).GetChild(0).gameObject;
            take_photo_interactable = take_photo_button.GetComponent<Interactable>();
            take_photo_button_AddOnClick(take_photo_interactable);
            
            back_button = transform.GetChild(2).GetChild(1).gameObject;
            back_interactable = back_button.GetComponent<Interactable>();
            back_AddOnClick(back_interactable);
        }

        //Input actions
        private void back_AddOnClick(Interactable back_interactable)
        {
            back_interactable.OnClick.AddListener((() => OnBackButtonLogic()));
        }

       

        private void take_photo_button_AddOnClick(Interactable take_photo_interactable)
        {
            take_photo_interactable.OnClick.AddListener((() => OnTakePhotoButtonLogic()));
        }
        
        private void OnTakePhotoButtonLogic()
        {
            TakePhoto();
            var eventArgs = new TakePhotoEventArgs();
            OnTakePhoto(this, eventArgs);
        }
        
        private void OnBackButtonLogic()
        {
            var eventArgs = new BackEventArgs();
            OnBack(this, eventArgs);
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
        
        
        
        private GameObject take_photo_button;
        private Interactable take_photo_interactable;
        
        private GameObject back_button;
        private Interactable back_interactable;
        
        
        public void MenuVisibility(bool flag)
        {
            this.transform.gameObject.SetActive(flag);
        }
        
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