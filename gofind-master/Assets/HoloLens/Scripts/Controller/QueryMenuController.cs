using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.HoloLens.Scripts.Properties;
using Assets.HoloLens.Scripts.View;
using Assets.Scripts.Core;
using CineastUnityInterface.Runtime.Vitrivr.UnityInterface.CineastApi;
using CineastUnityInterface.Runtime.Vitrivr.UnityInterface.CineastApi.Model.Data;
using CineastUnityInterface.Runtime.Vitrivr.UnityInterface.CineastApi.Model.Registries;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Utilities;
using Org.Vitrivr.CineastApi.Model;
using UnityEngine;

namespace Assets.HoloLens.Scripts.Controller
{
    public interface IQueryMenuController
    {
        void addQuery(POICoordinatesObject poiCoordinatesObject);
        IQueryMenuView getview();
        void Reset();
        void accessPhotoQuery();
        void setTemporal(DateTime upperBound, DateTime lowerbound, bool activate_temp);
        void setAndSearchTemporal(DateTime upperBound, DateTime lowerbound, bool activate_temp);

    }
       public class QueryMenuController: MonoBehaviour, IQueryMenuController
    {
        
        // Keep references to the model and view
        
        private static  IQueryMenuView view;
        private List<POICoordinatesObject> querylist;
        private List<Coordinates> PointOfInterests;
        private List<ObjectData> activeMmos;
        
        private static IResultPanelModel resultmodel;
        private Coordinates myLocation;
       
        private DateTime  upperBound;
        private DateTime lowerbound;
        private float dist_radius;
        private bool activate_temp;
        private DistanceAttribute distanceAttribute;

        private void Awake()
        {
            AotHelper.EnsureType<StringEnumConverter>();
            var falseJc = new Newtonsoft.Json.JsonConstructorAttribute();
        }

        //Initialize Model, view and Listeners

        private void Start()
        {
            querylist = new List<POICoordinatesObject>();
            view = transform.GetChild(7).GetComponent<QueryMenuView>();
            resultmodel = GetComponent<ResultPanelController>().GETResultPanelModel();
            distanceAttribute = transform.GetChild(7).GetChild(6).GetChild(0).GetChild(1)
                .GetComponent<DistanceAttribute>();
            PointOfInterests = new List<Coordinates>();
            myLocation = new Coordinates(0,0);
            
            // Listen to input from the view
            view.OnRemove += RemoveFromDatabase;
            view.OnReceived += SearchQuery;
            //view.OnSearch += SearchDebug;
            view.OnSearch += SearchClicked;
            view.OnErrorBack += HandleError;
            DistanceAttribute.OnDistanceChanged += UpdateDistance;


            // Listen to changes in the model
        }

     
        private void HandleError(object sender, BackEventArgs e)
        {
            view.setErrorDialogVisibility(false);
            IMainMenuController mainMenuController = GetComponent<MainMenuController>();
            mainMenuController.GETMainMenuView().SetMainMenuVisibility(true);
        }

        private void UpdateDistance(object sender, DistanceEventArgs e)
        {
            dist_radius = e.distance_value;
            resultmodel.setDistance(dist_radius);
        }

        private void SearchClicked(object sender, BackEventArgs e)
        {
            Debug.Log(myLocation);
            
            var query = CineastUnityInterface.Runtime.Vitrivr.UnityInterface.CineastApi.Utils.QueryBuilder
                .BuildSpatialSimilarityQuery(myLocation.getLat(), myLocation.getLon());
             QueryCineastAndProcess(query);
             
             IResultPanelModel resultPanelModel = transform.GetComponent<ResultPanelController>().GETResultPanelModel();
             IMainMenuController menuController = GetComponent<MainMenuController>();

             IResultPanelView resultPanelView = transform.GetComponent<ResultPanelController>().GETResultPanelView();
             Vector3 newpos = new Vector3(Camera.main.transform.position.x ,Camera.main.transform.position.y,Camera.main.transform.position.z+1.3f);
             resultPanelView.setResultPosition(newpos);
             
             menuController.GETMainMenuModel().ChangeVisibility(false);
             view.setVisibility(false);
             resultPanelModel.ChangeResultVisibility(true);

             IMapController mapController = GetComponent<MapController>();
             mapController.GETMapView().setGameObjectVisibility(false);

             IMapMenuController mapMenuController = GetComponent<MapMenuController>();
             mapMenuController.GETMapMenuView().MenuVisibility(false);

             ITemporalController temporalController = GetComponent<TemporalController>();
             temporalController.GETItTemporalView().MenuVisibility(false);
        }

        public void accessPhotoQuery()
        {
            IPhotoController photoController = GetComponent<PhotoController>();
            photoController.GETPhotoView().MenuVisibility(false);
            IResultPanelModel resultPanelModel = transform.GetComponent<ResultPanelController>().GETResultPanelModel();
            resultPanelModel.ChangeResultVisibility(true);
            var query = CineastUnityInterface.Runtime.Vitrivr.UnityInterface.CineastApi.Utils.QueryBuilder
                .BuildSpatialSimilarityQuery(myLocation.getLat(), myLocation.getLon());
            QueryCineastAndProcess(query);
        }
        

        public async void QueryCineastAndProcess(SimilarityQuery query)
        {
            bool f = false;
            
            var res = await Task.Run(async () =>
            {
                var results = await Task.Run(async () => await CineastWrapper.ExecuteQuery(query, 100, 100));
                Debug.Log("Results received. Fetching objectdata");
                await ObjectRegistry.Initialize(false); // Works due to collection being less than 100 and 100 are prefetched
                Debug.Log("Objectdata Fetched");
                await ObjectRegistry.BatchFetchObjectMetadata(ObjectRegistry.Objects);
                Debug.Log("Metadata fetched");
                Debug.Log("Fetched resutls: "+ results.results.Count);
                f = true;
                return results;
            });
            Debug.Log("Handling Result");

            while (f==false)
            {
                Debug.Log("waiting");
            }
            
            Debug.Log("finished");
            
            HandleCineastResult(ObjectRegistry.Objects); // TODO more sophisticated
        }

        private void SearchDebug(object sender, BackEventArgs e)
        {
            IResultPanelModel resultPanelModel = transform.GetComponent<ResultPanelController>().GETResultPanelModel();
            IMainMenuController menuController = GetComponent<MainMenuController>();
            menuController.GETMainMenuModel().ChangeVisibility(false);
            view.setVisibility(false);
            resultPanelModel.renderDebugPicture();
            resultPanelModel.ChangeResultVisibility(true);
        }

        public void addQuery(POICoordinatesObject poiCoordinatesObject)
        {
            view.setVisibility(true);
            view.createSelection(poiCoordinatesObject);
            querylist.Add(poiCoordinatesObject);
            FilterData(poiCoordinatesObject);
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
            MapController mapController = GetComponent<MapController>();
            mapController.GETMapView().removeLocationPins(e.RemoveObject);
            querylist.Remove(e.poi);
            PointOfInterests.Remove(e.poi.getCoordinates());
        }
        public void DoCineastRequest(double latitude, double longitude) {
            
            Clean();

            //initialLocation = new LocationInfo(); // Why?
            //ChangeState(State.CINEAST_REQUEST);
            //cineast.RequestSimilarAndThen(initialLocation.latitude, initialLocation.longitude, HandleCineastResult);
           
        }

        public void Clean()
        {
            //TODO
            //Clean the list
        }

        public void Reset()
        {
            querylist.Clear();
            activate_temp = false;
            activeMmos.Clear();
            PointOfInterests.Clear();
        }
    
        private void HandleCineastResult(List<ObjectData> list) {
            Debug.Log("HandleCineastResult");
            SetActiveList(list);
            //ChangeState(State.CINEAST_RESPONSE);
            

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
            //uiManager.SetAndPopulateList(activeMmos);
        }

        public void SetActiveList(List<ObjectData> mmos)
        {
            activeMmos = mmos;

            resultmodel.populateAndRender(mmos, upperBound, lowerbound, activate_temp,querylist);
        }

        public void RemoveFromActiveList(ObjectData mmo)
        {
            activeMmos.Remove(mmo);
        }

        // If already in list, ignored
        public void AddToActiveList(ObjectData mmo)
        {
            if (!activeMmos.Contains(mmo))
            {
                activeMmos.Add(mmo);
            }
        }

        public void setTemporal(DateTime upperBound, DateTime lowerbound, bool activate_temp)
        {
            this.upperBound = upperBound;
            this.lowerbound = lowerbound;
            this.activate_temp = activate_temp;
        }

        public void setAndSearchTemporal(DateTime upperBound, DateTime lowerbound, bool activate_temp)
        {
            this.upperBound = upperBound;
            this.lowerbound = lowerbound;
            this.activate_temp = activate_temp;
            SearchClicked(null,null);
        }
        

        public void FilterData(POICoordinatesObject poiCoordinatesObject)
        {
            if (poiCoordinatesObject.getMyLocation())
            {
                myLocation = poiCoordinatesObject.getCoordinates();
            }
            else
            {
                PointOfInterests.Add(poiCoordinatesObject.getCoordinates());
            }
        }
    }
}