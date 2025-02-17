﻿using System.Collections;
using System.Collections.Generic;
using Assets.HoloLens.Scripts.Model;
using Assets.HoloLens.Scripts.Properties;
using Assets.HoloLens.Scripts.View;
using UnityEngine;
using GeneratePinEventArgs = Assets.HoloLens.Scripts.View.GeneratePinEventArgs;
/*
 * Controllers are used for controlling models and views of various classes
 * Events are registered in Controllers, once a event occurs, the controller will trigger the associate functions.
 * Every event must and should be registered in the Controller.
 * Controllers are can also access other controller functions eg. Interface functions
 */
namespace Assets.HoloLens.Scripts.Controller
{
    public interface IMapMenuController
    {
        IMapMenuModel GETMapMenuModel();
        IMapMenuView GETMapMenuView();

        void AddPOIQuery(POICoordinatesObject poiCoordinatesObject);
    }

    public class MapMenuController : MonoBehaviour, IMapMenuController
    {
        private static IMapMenuModel model;
        private static IMapMenuView view;
        private static IGPSLoggerController GpsLoggerController;
        

        public IMapMenuModel GETMapMenuModel()
        {
            return model;
        }

        public IMapMenuView GETMapMenuView()
        {
            return view;
        }
        
        //Initialize Model, view and Listeners

        void Start()
        {
            model = new MapMenuModel();
            view = transform.GetChild(4).GetComponent<MapMenuView>();
            GpsLoggerController = GetComponentInParent<GPSLoggerController>().GETGpsLoggerController();
            

            // Listen to input from the view
            view.OnOneBack += HandleBack;
            view.OnGeneratePin += HandleGeneratePin;
            view.OnPOIRemove += RemoveFromModel;
            view.OnZoomMap += ZoomMap;
            view.OnJourney += HandleJourneyStart;
            view.OnCancelJourney += HandleCancelJourney;
            
            //Debug
            view.OnDebugReceived += GpsLoggerController.HandleGPSReceived;
            // Listen to changes in the model
            
            model.VisibilityChange += MenuStatusVisibility;
            model.OnMapPinGenerate += GenerateMapPins;
            
        }

        private void HandleCancelJourney(object sender, BackOneEventArgs e)
        {
            view.MenuVisibility(false);
            IResultPanelView resultPanelView = GetComponent<ResultPanelController>().GETResultPanelView();
            resultPanelView.setAllResultMenuVisibility(true);
        }

        private void HandleJourneyStart(object sender, BackOneEventArgs e)
        {
            IMapModel iMapModel = transform.GetComponent<MapController>().GETMapModel();
            IMapView iMapView = transform.GetComponent<MapController>().GETMapView();
            iMapView.setGameObjectVisibility(false);
            iMapModel.GETPoiCoordinatesObjectsList();
            view.SpatialExploration();
            
        }

        private void ZoomMap(object sender, ZoomMapEventArgs e)
        {
            IMapView iMapView = transform.GetComponent<MapController>().GETMapView();
            iMapView.ZoomMap(e.get());
        }

        private void RemoveFromModel(object sender, RemoveQueryDataArgs e)
        {
            IMapModel mapModel = transform.GetComponent<MapController>().GETMapModel();
            mapModel.RemovePOI(e.getID());
        }


        //Functions to call once an Event occurs

        private void GenerateMapPins(object sender, NotifyMapModel_GenerateEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        public void AddPOIQuery(POICoordinatesObject poiCoordinatesObject)
        {
            view.createSelection(poiCoordinatesObject);
        }


        private void HandleGeneratePin(object sender, GeneratePinEventArgs e)
        {
            IMapModel mapModel = transform.GetComponent<MapController>().GETMapModel();
            mapModel.GenerateMapPins();
        }

        private void MenuStatusVisibility(object sender, MapMenuDataChangedOutputEventArgs e)
        {
            view.MenuVisibility(e.flag);
        }


        private void HandleBack(object sender, BackOneEventArgs e)
        {
            IMainMenuModel mainMenuModel = transform.GetComponent<MainMenuController>().GETMainMenuModel();
            model.ChangeVisibility(false);
            mainMenuModel.ChangeVisibility(true);
            IMapModel mapModel = transform.GetComponent<MapController>().GETMapModel();
            mapModel.ChangeVisibility(false);
            
            IQueryMenuController IqueryMenuController = transform.GetComponent<QueryMenuController>();
            Vector3 pos = IqueryMenuController.getview().getInitQueryMenuPosition();
            IqueryMenuController.getview().setQueryMenuRadialPosition(pos, true);
        }
        
        //DEBUG

        public void CustomCoordinates()
        {
            
        }
        
    }
}