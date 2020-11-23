using System;
using System.Collections.Generic;
using Assets.HoloLens.Scripts.Model;
using Assets.HoloLens.Scripts.Properties;
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

        private static IAddedQueryOptionModel add_Querymodel;

        //Initialize Model, view and Listeners
        private void Start()
        {
            model = new MainMenuModel();
            view = transform.GetChild(1).GetComponent<MainMenuView>();

            add_Querymodel = new QueryOptionModel();

            // Listen to input from the view
            view.OnInputDataReceived += HandleInputData;
            view.OnCameraSelect += HandleCameraSelect;
            view.OnSpatialSelect += HandleSpatialSelect;
            view.OnCPositionSelect += HandleCPositionSelect;
            view.OnTemporalSelect += HandleTemporalSelect;
            view.OnSearchSelect += HandleReset;
            view.OnRemove += HandleRemoveQuery;
            

            // Listen to changes in the model
            model.DataOutput += HandleOutputData;
            model.VisibilityChange += MainMenuStatusVisibility;
            model.QueryDataChanged += UpdateQueryStatus;

            // Set the view's initial state by synching with the model
        }

        //An Interface so other controllers can access the model
        public IMainMenuModel GETMainMenuModel()
        {
            return model;
        }

        //Functions to call once an Event occurs
        
        //Handling models
        
        private void HandleRemoveQuery(object sender, RemoveQueryDataArgs e)
        {
            model.RemoveQueryOption(e.getID());
        }
        private void HandleSearchSelect(object sender, SearchEventArgs e)
        {
            IResultPanelModel resultPanelModel = transform.GetComponent<ResultPanelController>().GETResultPanelModel();
            model.ChangeVisibility(false);
            resultPanelModel.renderPicture();
            resultPanelModel.ChangeResultVisibility(true);
        }

        private void HandleReset(object sender, SearchEventArgs e)
        {
            ResultPanelController controller = GetComponent<ResultPanelController>();
            controller.GETResultPanelView().reset();
        }

        private void HandleTemporalSelect(object sender, TemporalEventArgs e)
        {
            ITemporalModel temporalModel = transform.GetComponent<TemporalController>().GETItTemporalModel();
            model.ChangeVisibility(false);
            temporalModel.ChangeVisibility(true);
        }

        private void HandleCPositionSelect(object sender, QueryOptionEventArgs e)
        {
            IGPSLoggerModel gpsLoggerModel = transform.GetComponent<GPSLoggerController>().GETGPSLoggerModel();
            //Needs function to wait until gps signal is available
           // model.setQueryData(gpsLoggerModel.getStringGPSCoordinates());

            QueryMenuController IqueryMenuView = transform.GetComponent<QueryMenuController>();
            POICoordinatesObject poiCoordinatesObject = new POICoordinatesObject(gpsLoggerModel.getLatitude(),gpsLoggerModel.getLongitude(),null);
            poiCoordinatesObject.setName("My Location");
            IqueryMenuView.addQuery(poiCoordinatesObject);
        }

        private void HandleSpatialSelect(object sender, SpatialEventArgs e)
        {
            IMapMenuModel mapMenuModel = transform.GetComponent<MapMenuController>().GETMapMenuModel();
            model.ChangeVisibility(false);
            mapMenuModel.ChangeVisibility(true);

            IMapModel mapModel = transform.GetComponent<MapController>().GETMapModel();
            model.ChangeVisibility(false);
            mapModel.ChangeVisibility(true);

            IQueryMenuController IqueryMenuController = transform.GetComponent<QueryMenuController>();
            IqueryMenuController.getview().setQueryMenuRadialPosition(new Vector3(0.1f,0,0), false);
            IqueryMenuController.getview().setQueryMenuPosition(new Vector3(0,0,1f));
        }

        private void HandleCameraSelect(object sender, CameraEventArgs e)
        {
            IPhotoModel photoModel = transform.GetComponent<PhotoController>().GETPhotoModel();
            model.ChangeVisibility(false);
            photoModel.ChangeVisibility(true);
        }
        
        

        //Handling views
        private void UpdateQueryStatus(object sender, AddedQueryOption e)
        {
            view.HideQueryOption(true);
            view.updateQueryButtonData(e.getData());
        }
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