using System;
using Assets.HoloLens.Scripts.Model;
using Assets.HoloLens.Scripts.View;
using UnityEngine;
using PhotoChangedEventArgs = Assets.HoloLens.Scripts.View.PhotoChangedEventArgs;
/*
 * Controllers are used for controlling models and views of various classes
 * Events are registered in Controllers, once a event occurs, the controller will trigger the associate functions.
 * Every event must and should be registered in the Controller.
 * Controllers are can also access other controller functions eg. Interface functions
 */
namespace Assets.HoloLens.Scripts.Controller
{
    
    public interface IPhotoController
    {
        IPhotoModel GETPhotoModel();
        IPhotoView GETPhotoView();
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

        public IPhotoView GETPhotoView()
        {
            return view;
        }

        //Initialize Model, view and Listeners

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

        //Functions to call once an Event occurs

        private void HandleBack(object sender, BackEventArgs e)
        {
            IMainMenuModel mainMenuModel = transform.GetComponent<MainMenuController>().GETMainMenuModel();
            model.ChangeVisibility(false);
            mainMenuModel.ChangeVisibility(true);
        }

        private void HandleTakePhoto(object sender, TakePhotoEventArgs e)
        {
            IQueryMenuController controller = GetComponent<QueryMenuController>();
            controller.accessPhotoQuery();
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