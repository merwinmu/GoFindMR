using System;
using Assets.HoloLens.Scripts.Model;
using Assets.HoloLens.Scripts.View;
using UnityEngine;
/*
 * Controllers are used for controlling models and views of various classes
 * Events are registered in Controllers, once a event occurs, the controller will trigger the associate functions.
 * Every event must and should be registered in the Controller.
 * Controllers are can also access other controller functions eg. Interface functions
 */
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

        //Initialize Model, view and Listeners
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
        
        //An Interface so other controllers can access the model
        public IMainMenuModel GETMainMenuModel()
        {
            return model;
        }

        //Functions to call once an Event occurs
        
        //Handling models
        private void HandleSearchSelect(object sender, SearchEventArgs e)
        {
            IResultPanelModel resultPanelModel = transform.GetComponent<ResultPanelController>().GETResultPanelModel();
            model.ChangeVisibility(false);
            resultPanelModel.ChangeResultVisibility(true);
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

            IMapModel mapModel = transform.GetComponent<MapController>().GETMapModel();
            model.ChangeVisibility(false);
            mapModel.ChangeVisibility(true);
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