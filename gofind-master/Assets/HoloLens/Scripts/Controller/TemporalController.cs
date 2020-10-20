using System;
using Assets.HoloLens.Scripts.Model;
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
            // Listen to changes in the model
            model.OnYearchanged += HandleYearChanged;
            model.VisibilityChange += TextBoxStatusVisibility;
        }

        //Functions to call once an Event occurs

        private void HandleBackButtonOnPress(object sender, MapBackEventArgs e)
        {
            IMainMenuModel mainMenuModel = transform.GetComponent<MainMenuController>().GETMainMenuModel();
            model.ChangeVisibility(false);
            mainMenuModel.ChangeVisibility(true);
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
            model.LowerBound = e.lowerbound;
            model.UpperBound = e.upperbound;
            
            IMainMenuModel mainMenuModel = transform.GetComponent<MainMenuController>().GETMainMenuModel();
            model.ChangeVisibility(false);
            mainMenuModel.ChangeVisibility(true);
            
            //Debug.Log("Temporal input: "+model.LowerBound+" "+model.UpperBound);
        }

        public ITemporalModel GETItTemporalModel()
        {
            return model;
        }
    }
}