using System;
using System.Collections.Generic;
using Assets.HoloLens.Scripts.Model;
using Assets.HoloLens.Scripts.View;
using UnityEngine;
using GeneratePinEventArgs = Assets.HoloLens.Scripts.Model.GeneratePinEventArgs;
/*
 * Controllers are used for controlling models and views of various classes
 * Events are registered in Controllers, once a event occurs, the controller will trigger the associate functions.
 * Every event must and should be registered in the Controller.
 * Controllers are can also access other controller functions eg. Interface functions
 */
namespace Assets.HoloLens.Scripts.Controller
{
    public interface IMapController
    {
        IMapModel GETMapModel();
        IMapView GETMapView();
    }
    
    public class MapController : MonoBehaviour, IMapController
    {
        
        // Keep references to the model and view
        private static IMapModel model;
        private static IMapView view;
        private static IMapMenuController mapMenuController;


        //An Interface so other controllers can access the model
        public IMapModel GETMapModel()
        {
            return model;
        }
        
        public IMapView GETMapView()
        {
            return view;
        }
        
        //Initialize Model, view and Listeners
        private void Start()
        {
            model = new MapModel();
            view =  transform.GetChild(5).GetComponent<MapView>();
            spawnObjectOnCoordinateEventArgses = new List<SpawnObjectOnCoordinateEventArgs>();
            removeSpawnObjectEventArgses = new List<SpawnObjectOnCoordinateEventArgs>();
            
            
            
            // Listen to input from the view
            ZoomToMapPin.OnMapObject += HandlePOIInput;
            
            
            // Listen to changes in the model
            model.GeneratePinMap += GeneratePinMaps;
            model.MapVisibility += MainMenuStatusVisibility;
            model.OnSpawnCoordinate += SpawnObjectRealWorld;
            model.OnRemoveObject += RemoveObjectRealWorld;

        }
        
        private List<SpawnObjectOnCoordinateEventArgs> removeSpawnObjectEventArgses;
        private void RemoveObjectRealWorld(object sender, SpawnObjectOnCoordinateEventArgs e)
        {
            removeSpawnObjectEventArgses.Add(e);
        }

        private List<SpawnObjectOnCoordinateEventArgs> spawnObjectOnCoordinateEventArgses;
        private void SpawnObjectRealWorld(object sender, SpawnObjectOnCoordinateEventArgs e)
        {
            spawnObjectOnCoordinateEventArgses.Add(e);
        }

        private void HandlePOIInput(object sender, POIEventArgs e)
        {
            model.AddPOILocation(e.GETPoiCoordinatesObject()) ;
            
            mapMenuController = transform.GetComponent<MapMenuController>();
            mapMenuController.AddPOIQuery(e.GETPoiCoordinatesObject());
            
        }


        //Functions to call once an Event occurs

        private void GeneratePinMaps(object sender, GeneratePinEventArgs e)
        {
            view.RenderGenerateMapPins();
        }

        private void MainMenuStatusVisibility(object sender, MapVisibilityEventArgs e)
        {
            view.setGameObjectVisibility(e.flag);
        }

        private void Update()
        {
            if (spawnObjectOnCoordinateEventArgses.Count != 0)
            {
                IMapMenuView mapMenuView = transform.GetComponent<MapMenuController>().GETMapMenuView();
                foreach (var VARIABLE in spawnObjectOnCoordinateEventArgses)
                {
                    mapMenuView.RenderGameObject(VARIABLE.get());
                }
                
                spawnObjectOnCoordinateEventArgses.Clear();
            }

            if (removeSpawnObjectEventArgses.Count != 0)
            {
                IMapMenuView mapMenuView = transform.GetComponent<MapMenuController>().GETMapMenuView();
                foreach (var VARIABLE in removeSpawnObjectEventArgses)
                {
                    mapMenuView.RemoveGameObject(VARIABLE.get());
                }
                removeSpawnObjectEventArgses.Clear();
            }
        }
    }
}