using System;
using Assets.HoloLens.Scripts.Model;
using Assets.HoloLens.Scripts.Properties;
using Assets.HoloLens.Scripts.View;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.HoloLens.Scripts.Controller
{/*
 * Controllers are used for controlling models and views of various classes
 * Events are registered in Controllers, once a event occurs, the controller will trigger the associate functions.
 * Every event must and should be registered in the Controller.
 * Controllers are can also access other controller functions eg. Interface functions
 */
    public interface ITemporalController
    {
        ITemporalModel GETItTemporalModel();
    }
    
    public class TemporalController: MonoBehaviour,ITemporalController
    {
        private GameObject IO_System;

        // Declaring Input fields
        private GameObject InputFieldUpperBound;
        private GameObject InputFieldLowerBound;
        
        
        // Keep references to the model and view
        private static  ITemporalModel model;
        private static  ITemporalView view;


        private void Awake()
        {
            IO_System = GameObject.FindWithTag("IOSystem");
        }
        
        
        //Initialize Model, view and Listeners

        private void Start()
        {
            model = new TemporalModel();
            view = transform.GetChild(2).GetComponent<TemporalView>();

            // Listen to input from the view
            view.OnReceived += HandleInputReceived;
            view.MapBackButton += HandleBackButtonOnPress;
            LowerBoundAttribute.OnLowerBoundValueChanged += HandleLowerBound;
            UpperBoundAttribute.OnUpperBoundValueChanged += HandleUpperBound;
            // Listen to changes in the model
            model.OnYearchanged += HandleYearChanged;
            model.VisibilityChange += TextBoxStatusVisibility;
        }

        private void HandleUpperBound(object sender, SliderValueEventArgs e)
        {
            model.setUpperBound(e.value);
        }

        private void HandleLowerBound(object sender, SliderValueEventArgs e)
        {
            model.setLowerBound(e.value);
        }

        //Functions to call once an Event occurs

        private void HandleBackButtonOnPress(object sender, MapBackEventArgs e)
        {
            IMainMenuModel mainMenuModel = transform.GetComponent<MainMenuController>().GETMainMenuModel();
            model.ChangeVisibility(false);
            mainMenuModel.ChangeVisibility(true);
            
            IQueryMenuController IqueryMenuController = transform.GetComponent<QueryMenuController>();
            Vector3 pos = IqueryMenuController.getview().getInitQueryMenuPosition();
            IqueryMenuController.getview().setQueryMenuRadialPosition(pos, true);
        }

        //Handling views
        private void TextBoxStatusVisibility(object sender, TemporalMenuChangedEventArgs e)
        {
            view.MenuVisibility(e.flag);
        }

        private void HandleYearChanged(object sender, SearchYearChangedEventArgs e)
        {
            
        }

        //Handling models
        private void HandleInputReceived(object sender, YearChangeEventArgs e)
        {
            IMainMenuModel mainMenuModel = transform.GetComponent<MainMenuController>().GETMainMenuModel();
            model.ChangeVisibility(false);
            //mainMenuModel.setQueryData(model.LowerBound +" "+ model.UpperBound);
            
            POICoordinatesObject poiCoordinatesObject = new POICoordinatesObject(model.getLowerBound(),model.getUpperBound());
            poiCoordinatesObject.setName("From "+model.getLowerBound().ToString() +" to "+model.getUpperBound().ToString());
            mainMenuModel.ChangeVisibility(true);
            IQueryMenuController iqQueryMenuController = GetComponent<QueryMenuController>();
            iqQueryMenuController.addQuery(poiCoordinatesObject);

            Vector3 pos = iqQueryMenuController.getview().getInitQueryMenuPosition();
            iqQueryMenuController.getview().setQueryMenuRadialPosition(pos, true);
            
        }

        public ITemporalModel GETItTemporalModel()
        {
            return model;
        }
    }
}