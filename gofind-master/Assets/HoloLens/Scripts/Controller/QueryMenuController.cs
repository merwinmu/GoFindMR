using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.HoloLens.Scripts.Properties;
using Assets.HoloLens.Scripts.View;
using Assets.Scripts.Core;
using CineastUnityInterface.Runtime.Vitrivr.UnityInterface.CineastApi;
using CineastUnityInterface.Runtime.Vitrivr.UnityInterface.CineastApi.Model.Data;
using CineastUnityInterface.Runtime.Vitrivr.UnityInterface.CineastApi.Model.Registries;
using Org.Vitrivr.CineastApi.Model;
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
        private List<POICoordinatesObject> querylist;
        private static IResultPanelModel resultmodel;
        private List<ObjectData> activeMmos;
        private Coordinates myLocation;
        private List<Coordinates> PointOfInterests;

        private void Awake()
        {
          
        }

        //Initialize Model, view and Listeners

        private void Start()
        {
            querylist = new List<POICoordinatesObject>();
            view = transform.GetChild(7).GetComponent<QueryMenuView>();
            resultmodel = GetComponent<ResultPanelController>().GETResultPanelModel();
            PointOfInterests = new List<Coordinates>();
            myLocation = new Coordinates(0,0);
            
            // Listen to input from the view
            view.OnRemove += RemoveFromDatabase;
            view.OnReceived += SearchQuery;
            //view.OnSearch += SearchDebug;
            view.OnSearch += SearchClicked;


            // Listen to changes in the model
        }

        private void SearchClicked(object sender, BackEventArgs e)
        {
            Debug.Log(myLocation);
            var query = CineastUnityInterface.Runtime.Vitrivr.UnityInterface.CineastApi.Utils.QueryBuilder
                .BuildSpatialSimilarityQuery(myLocation.getLat(), myLocation.getLon());
             QueryCineastAndProcess(query);
             
             IResultPanelModel resultPanelModel = transform.GetComponent<ResultPanelController>().GETResultPanelModel();
             IMainMenuController menuController = GetComponent<MainMenuController>();
             menuController.GETMainMenuModel().ChangeVisibility(false);
             view.setVisibility(false);
             resultPanelModel.ChangeResultVisibility(true);
        }
        
        public async void QueryCineastAndProcess(SimilarityQuery query)
        {
            var res = await Task.Run(async () =>
            {
                var results = await Task.Run(async () => await CineastWrapper.ExecuteQuery(query, 100, 100));
                Debug.Log("Results received. Fetching objectdata");
                await ObjectRegistry.Initialize(false); // Works due to collection being less than 100 and 100 are prefetched
                Debug.Log("Objectdata Fetched");
                await ObjectRegistry.BatchFetchObjectMetadata(ObjectRegistry.Objects);
                Debug.Log("Metadata fetched");
                Debug.Log("Fetched resutls: "+ results.results.Count);
                return results;
            });
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
            
        }
        public void DoCineastRequest(double latitude, double longitude) {
            Debug.Log("DoCineastRequest LInfo " + latitude + "," + longitude);
        
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
            //TODO
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
            resultmodel.populateAndRender(mmos);
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