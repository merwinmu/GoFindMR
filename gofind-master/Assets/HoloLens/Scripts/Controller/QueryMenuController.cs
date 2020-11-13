using Assets.HoloLens.Scripts.Model;
using Assets.HoloLens.Scripts.Properties;
using Assets.HoloLens.Scripts.View;
using UnityEngine;

namespace Assets.HoloLens.Scripts.Controller
{

    public interface IQueryMenuController
    {
        void addQuery(POICoordinatesObject poiCoordinatesObject);
    }
       public class QueryMenuController: MonoBehaviour, IQueryMenuController
    {
        
        // Keep references to the model and view
        
        private static  IQueryMenuView view;
        
        private void Awake()
        {
           
        }

        //Initialize Model, view and Listeners

        private void Start()
        {
            
            view = transform.GetChild(7).GetComponent<QueryMenuView>();

            // Listen to input from the view
            view.OnRemove += RemoveFromDatabase;
            view.OnReceived += SearchQuery;

            // Listen to changes in the model

        }

        public void addQuery(POICoordinatesObject poiCoordinatesObject)
        {
            view.setVisibility(true);
            view.createSelection(poiCoordinatesObject);
        }

        private void SearchQuery(object sender, QueryCompleteEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void RemoveFromDatabase(object sender, QueryRemoveEventArgs e)
        {
            
        }

        //Functions to call once an Event occurs

        //Handling views


        //Handling models
    }
}