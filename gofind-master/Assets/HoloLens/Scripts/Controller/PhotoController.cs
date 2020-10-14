using System;
using Assets.HoloLens.Scripts.Model;
using Assets.HoloLens.Scripts.View;
using UnityEngine;
using PhotoChangedEventArgs = Assets.HoloLens.Scripts.View.PhotoChangedEventArgs;

namespace Assets.HoloLens.Scripts.Controller
{
    public interface IPhotoController
    {
    }
    public class PhotoController: MonoBehaviour, IPhotoController
    {
        private GameObject IO_System;
        // Keep references to the model and view
        private static  IPhotoModel model;
        private static  IPhotoView view;

        private void Awake()
        {
            IO_System = GameObject.FindWithTag("IOSystem");

        }

        private void Start()
        {
            model = new PhotoModel();
            model.Photo  = new Texture2D(100,100);
            view = IO_System.GetComponent<PhotoView>();
            
        
            // Listen to input from the view
            view.OnReceived += HandleInputReceived;
            // Listen to changes in the model
            model.OnPictureChanged += HandlePictureChanged;
        }

        private void HandlePictureChanged(object sender, Model.PhotoChangedEventArgs e)
        {
            
        }

        private void HandleInputReceived(object sender, PhotoChangedEventArgs e)
        {
            model.Photo = e.Photo;
        }
    }
}