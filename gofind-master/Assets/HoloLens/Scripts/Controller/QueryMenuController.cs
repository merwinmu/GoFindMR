using System.Collections.Generic;
using Assets.HoloLens.Scripts.Model;
using Assets.HoloLens.Scripts.Properties;
using Assets.HoloLens.Scripts.View;
using CineastUnityInterface.Runtime.Vitrivr.UnityInterface.CineastApi;
using CineastUnityInterface.Runtime.Vitrivr.UnityInterface.CineastApi.Models;
using CineastUnityInterface.Runtime.Vitrivr.UnityInterface.CineastApi.Utils;
using UnityEngine;

namespace Assets.HoloLens.Scripts.Controller
{

    public interface IQueryMenuController
    {
        void addQuery(POICoordinatesObject poiCoordinatesObject);
        IQueryMenuView getview();
    }
       public class QueryMenuController: MonoBehaviour, IQueryMenuController
    {
        
        // Keep references to the model and view
        
        private static  IQueryMenuView view;
        private static IResultPanelModel resultmodel;
        private List<MultimediaObject> activeMmos;
        private List<MultimediaObject> mmoList;
        private CineastApi cineast;

        private void Awake()
        {
            this.gameObject.AddComponent<CineastApi>();
            cineast = this.GetComponent<CineastApi>();
        }

        //Initialize Model, view and Listeners

        private void Start()
        {
            
            view = transform.GetChild(7).GetComponent<QueryMenuView>();
            resultmodel = GetComponent<ResultPanelController>().GETResultPanelModel();
            // Listen to input from the view
            view.OnRemove += RemoveFromDatabase;
            view.OnReceived += SearchQuery;
            view.OnSearch += SearchClicked;

            
            // Listen to changes in the model

        }

        private void SearchClicked(object sender, BackEventArgs e)
        {
            
            double lat = 47.559601;
            double lon = 7.588576;
            
            DoCineastRequest(lat,lon);
        }

        public void addQuery(POICoordinatesObject poiCoordinatesObject)
        {
            view.setVisibility(true);
            view.createSelection(poiCoordinatesObject);
        }

        public IQueryMenuView getview()
        {
            return view;
        }

        private void SearchQuery(object sender, QueryCompleteEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void RemoveFromDatabase(object sender, QueryRemoveEventArgs e)
        {
            
        }
        public void DoCineastRequest(double latitude, double longitude) {
            Debug.Log("DoCineastRequest LInfo " + latitude + "," + longitude);
        
            Clean();

            //initialLocation = new LocationInfo(); // Why?
            //ChangeState(State.CINEAST_REQUEST);
            //cineast.RequestSimilarAndThen(initialLocation.latitude, initialLocation.longitude, HandleCineastResult);
            cineast.RequestSimilarAndThen(
                QueryFactory.BuildSpatialSimilarQuery(latitude,longitude),
                HandleCineastResult);
        }

        public void Clean()
        {
            //TODO
            //Clean the list
        }

        public void Reset()
        {
            //TODO
        }
    
        private void HandleCineastResult(List<MultimediaObject> list) {
            Debug.Log("HandleCineastResult");
            Debug.Log(list);
            //ChangeState(State.CINEAST_RESPONSE);
            mmoList = list;
            Debug.Log("Internal mmo list set to received one");

            // == SORT DISTANCE ==
            /*
            mmoList.Sort((mmo1, mmo2) => {
                double dist1 = Utilities.HaversineDistance(mmo1.latitude, mmo1.longitude, initialGeoLocation.latitude,
                    initialGeoLocation.longitude);
                double dist2 = Utilities.HaversineDistance(mmo2.latitude, mmo2.longitude, initialGeoLocation.latitude,
                    initialGeoLocation.longitude);
                return dist1.CompareTo(dist2);
            });
            logger.Debug("Sorted mmo list");
            */

            //mapController.CreateMarkers(mmoList);
            // mapController.ReloadAndCenter(0, 0, null, CreateMarkers());

            // DEBUG
            //InjectDebug();

            //uiManager.SetInitialLocation(initialLocation);
            //uiManager.SetInitialGeoLocation(initialGeoLocation);
            //uiManager.panelSwitcher.SwitchToChoice();

            //uiManager.panelManager.ShowPanel("choice");
            //uiManager.viewChoiceHomeBtn.gameObject.SetActive(true);
            activeMmos = list;
            //uiManager.SetAndPopulateList(activeMmos);
        }

        //Functions to call once an Event occurs

        //Handling views


        //Handling models
    }
}