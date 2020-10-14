using System;
using Assets.HoloLens.Scripts.Model;
using Assets.HoloLens.Scripts.View;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.HoloLens.Scripts.Controller
{
    public interface ITemporalController
    {
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

        private void Start()
        {
            model = new TemporalModel();
            view = IO_System.GetComponent<TemporalView>();
            
            view.setGameObject(transform.gameObject);

            // Listen to input from the view
            view.OnReceived += HandleInputReceived;
            // Listen to changes in the model
            model.OnYearchanged += HandleYearChanged;
        }

        private void HandleYearChanged(object sender, SearchYearChangedEventArgs e)
        {
            
        }

        private void HandleInputReceived(object sender, YearChangeEventArgs e)
        {
            model.LowerBound = e.lowerbound;
            model.UpperBound = e.upperbound;
            Debug.Log(model.LowerBound+" "+model.UpperBound);
        }
    }
}