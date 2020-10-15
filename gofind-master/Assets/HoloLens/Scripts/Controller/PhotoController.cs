using System;
using Assets.HoloLens.Scripts.Model;
using Assets.HoloLens.Scripts.View;
using UnityEngine;
using PhotoChangedEventArgs = Assets.HoloLens.Scripts.View.PhotoChangedEventArgs;

namespace Assets.HoloLens.Scripts.Controller
{
    public interface IPhotoController
    {
        IPhotoModel GETPhotoModel();
    }
    public class PhotoController: MonoBehaviour, IPhotoController
    {
        // Keep references to the model and view
        private static  IPhotoModel model;
        private static  IPhotoView view;
        
        
        public IPhotoModel GETPhotoModel()
        {
            return model;
        }

        private void Start()
        {
            model = new PhotoModel();
            model.Photo  = new Texture2D(100,100);
            view =  transform.GetChild(3).GetComponent<PhotoView>();
            
            // Listen to input from the view
            view.OnReceived += HandleInputReceived;
            view.OnTakePhoto += HandleTakePhoto;
            view.OnBack += HandleBack;
            // Listen to changes in the model
            model.OnPictureChanged += HandlePictureChanged;
            model.VisibilityChange += MenuStatusVisibility;
        }

        private void HandleBack(object sender, BackEventArgs e)
        {
            IMainMenuModel mainMenuModel = transform.GetComponent<MainMenuController>().GETMainMenuModel();
            model.ChangeVisibility(false);
            mainMenuModel.ChangeVisibility(true);
        }

        private void HandleTakePhoto(object sender, TakePhotoEventArgs e)
        {
            throw new NotImplementedException();
        }


        private void MenuStatusVisibility(object sender, PhotoMenuChangedEventArgs e)
        {
            view.MenuVisibility(e.flag);
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