using System.Collections;
using System.Collections.Generic;
using Assets.HoloLens.Scripts.Model;
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
    }

    public class MapMenuController : MonoBehaviour, IMapMenuController
    {
        private static IMapMenuModel model;
        private static IMapMenuView view;


   

        public IMapMenuModel GETMapMenuModel()
        {
            return model;
        }
        //Initialize Model, view and Listeners

        void Start()
        {
            model = new MapMenuModel();
            view = transform.GetChild(4).GetComponent<MapMenuView>();

            // Listen to input from the view
            view.OnOneBack += HandleBack;
            view.OnGeneratePin += HandleGeneratePin;
            // Listen to changes in the model
            
            model.VisibilityChange += MenuStatusVisibility;
            model.OnMapPinGenerate += GenerateMapPins;
        }
        
        
        //Functions to call once an Event occurs

        private void GenerateMapPins(object sender, NotifyMapModel_GenerateEventArgs e)
        {
            throw new System.NotImplementedException();
           
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
            
        }
    }
}