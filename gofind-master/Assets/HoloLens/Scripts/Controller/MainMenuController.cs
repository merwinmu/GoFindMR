using System;
using Assets.HoloLens.Scripts.Model;
using Assets.HoloLens.Scripts.View;
using UnityEngine;

namespace Assets.HoloLens.Scripts.Controller
{
    public interface IMainMenuController
    {
        IMainMenuModel GETMainMenuModel();
    }
    public class MainMenuController: MonoBehaviour, IMainMenuController
    {
        // Keep references to the model and view
        private static  IMainMenuModel model;
        private static  IMainMenuView view;


        private void Start()
        {
            model = new MainMenuModel();
            view = transform.GetChild(1).GetComponent<MainMenuView>();
            
            
            // Listen to input from the view
            view.OnInputDataReceived += HandleInputData;
            view.OnCameraSelect += HandleCameraSelect;
            view.OnSpatialSelect += HandleSpatialSelect;
            view.OnCPositionSelect += HandleCPositionSelect;
            view.OnTemporalSelect += HandleTemporalSelect;
            view.OnSearchSelect += HandleSearchSelect;

            // Listen to changes in the model
            model.DataOutput += HandleOutputData;
            model.VisibilityChange += MainMenuStatusVisibility;

            // Set the view's initial state by synching with the model

        }
        
        public IMainMenuModel GETMainMenuModel()
        {
            return model;
        }

        private void HandleSearchSelect(object sender, SearchEventArgs e)
        {
            model.Search_query();
        }

        private void HandleTemporalSelect(object sender, TemporalEventArgs e)
        {
            ITemporalModel temporalModel = transform.GetComponent<TemporalController>().GETItTemporalModel();
            model.ChangeVisibility(false);
            temporalModel.ChangeVisibility(true);
        }

        private void HandleCPositionSelect(object sender, CPositionEventArgs e)
        {
            model.CPosition_query();
        }

        private void HandleSpatialSelect(object sender, SpatialEventArgs e)
        {
            IMapMenuModel mapMenuModel = transform.GetComponent<MapMenuController>().GETMapMenuModel();
            model.ChangeVisibility(false);
            mapMenuModel.ChangeVisibility(true);
        }

        private void HandleCameraSelect(object sender, CameraEventArgs e)
        {
            IPhotoModel photoModel = transform.GetComponent<PhotoController>().GETPhotoModel();
            model.ChangeVisibility(false);
            photoModel.ChangeVisibility(true);
        }
        
        

        //Handling views
        private void MainMenuStatusVisibility(object sender, MainMainChangedEventArgs e)
        {
            view.SetMainMenuVisibility(e.flag);
        }

        private void HandleInputData(object sender, InputDataEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void HandleOutputData(object sender, DataChangedOutputEventArgs e)
        {
            throw new NotImplementedException();
        }

        
    }
}