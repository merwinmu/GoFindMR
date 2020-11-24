using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Assets.HoloLens.Scripts.Model;
using Assets.HoloLens.Scripts.Properties;
using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 * Views are primarly used for Input and Output. It is primarly a Monobehaviour class with the associate functions 
 * Input actions such as OnClick invoke an event to the controller which then executes a function to model
 * Output actions are in example rendering gameobjects etc.
 */



/*
 * Various EventArgs has been created so that if an Input occurs , a callback can be
 * invoked to the controller which then sends it to the model
 */

namespace Assets.HoloLens.Scripts.View
{
    public class BackOneEventArgs : EventArgs
    {
    }
    
    public class GeneratePinEventArgs : EventArgs
    {
    }
    

    public interface IMapMenuView
    {
        event EventHandler<BackOneEventArgs> OnOneBack;
        event EventHandler<GeneratePinEventArgs> OnGeneratePin;
        event EventHandler<RemoveQueryDataArgs> OnPOIRemove;
        event EventHandler<ZoomMapEventArgs> OnZoomMap;
        event EventHandler<BackOneEventArgs> OnJourney;
        event EventHandler<BackOneEventArgs> OnCancelJourney;
        event EventHandler<GPSDataReceivedEventArgs> OnDebugReceived;
        
        void MenuVisibility(bool flag);

        void createSelection(POICoordinatesObject poiCoordinatesObject);
        void RenderGameObject(POICoordinatesObject poiCoordinatesObject);
        void setCurrentPositionPin(double latitude, double longitude, float heading);
        void RemoveGameObject(POICoordinatesObject poiCoordinatesObject);

        void setPOIList(Dictionary<int, POICoordinatesObject> poiCoordinatesObjects);

        void SpatialExploration();

        void MiniMapRender();

    }

    public class ZoomMapEventArgs
    {
        private float data;

        public ZoomMapEventArgs(float data)
        {
            this.data = data;
        }

        public float get()
        {
            return this.data;
        }
    }

    public class MapMenuView : MonoBehaviour, IMapMenuView
    {
        public event EventHandler<BackOneEventArgs> OnOneBack = (sender, e) => { };
        public event EventHandler<GeneratePinEventArgs> OnGeneratePin= (sender, e) => { };
        public event EventHandler<RemoveQueryDataArgs> OnPOIRemove= (sender, e) => { };
        public event EventHandler<ZoomMapEventArgs> OnZoomMap= (sender, e) => { };
        public event EventHandler<BackOneEventArgs> OnJourney= (sender, e) => { };
        public event EventHandler<BackOneEventArgs> OnCancelJourney= (sender, e) => { };
        public event EventHandler<GPSDataReceivedEventArgs> OnDebugReceived= (sender, e) => { };

        Dictionary<POICoordinatesObject,GameObject> SpawnedObjects = new Dictionary<POICoordinatesObject,GameObject>();


        
        [SerializeField]
        private MapPinLayer _mapPinLayer;

        [SerializeField]
        private MapPin currentMapPin;

        public double currentLatitude = 47.5569389;

        public double currentLongitude = 7.5888067;

        public float currentheading = 250;
        
        
        
        

        private bool MainMenuStatus;
         
        //Buttons init
        private GameObject back_button;
        private Interactable back_interactable;
        
        private GameObject ResetBaselButton;
        private Interactable resetBaselInteractable;
        
        private GameObject generate_button;
        private Interactable generate_interactable;

        private GameObject query0_button;
        private Interactable query0_interactable;

        private GameObject journeyButton;
        private Interactable journeyInteractable;

        private GameObject mapMenuButtons;
        
        private GameObject stopARMenuButton;
        private Interactable stopARMenuButtonInteractable;
        
        private GameObject scrollObeObjectCollectionGameObject;
        private ScrollingObjectCollection scrollingObjectCollection;

        private GameObject slider;
        private GameObject POIQuery;
        private GameObject mapMenu;
        private GameObject cancelButton;
        private GameObject mainMenuButtons;
        
        private Interactable cancelInteractable;
        private GameObject miniMap;
        public MapRenderer MiniMapRenderer;
        public MapRenderer BigMapRenderer;

        private bool journeyStart;
        

        private void Start()
        {
            
            transform.gameObject.SetActive(false);
            
            generate_button = transform.GetChild(2).GetChild(0).gameObject;
            generate_interactable = generate_button.GetComponent<Interactable>();
            generate_AddOnClick(generate_interactable);

            back_button = transform.GetChild(2).GetChild(1).gameObject;
            back_interactable = back_button.GetComponent<Interactable>();
            back_AddOnClick(back_interactable);

            ResetBaselButton = transform.GetChild(2).GetChild(2).gameObject;
            resetBaselInteractable = ResetBaselButton.GetComponent<Interactable>();
            resetBaselAddOnClick(resetBaselInteractable);

            journeyButton = transform.GetChild(6).gameObject;
            journeyInteractable = journeyButton.GetComponent<Interactable>();
            journey_AddOnClick(journeyInteractable);
            
            cancelButton = transform.GetChild(7).gameObject;
            cancelInteractable = cancelButton.GetComponent<Interactable>();
            cancel_AddOnClick(cancelInteractable);

            stopARMenuButton = transform.GetChild(10).gameObject;
            stopARMenuButtonInteractable = stopARMenuButton.GetComponent<Interactable>();
            stopAR_AddOnclick(stopARMenuButtonInteractable);
            
            

            scrollObeObjectCollectionGameObject = transform.GetChild(4).GetChild(1).gameObject;
            scrollingObjectCollection = scrollObeObjectCollectionGameObject.GetComponent<ScrollingObjectCollection>();
            
            slider = transform.GetChild(5).gameObject;
            POIQuery = transform.GetChild(4).gameObject;
            miniMap = transform.GetChild(8).gameObject;
            MiniMapRenderer = miniMap.GetComponent<MapRenderer>();
            miniMap.SetActive(false);
            BigMapRenderer = transform.parent.GetChild(5).GetComponent<MapRenderer>();
            mapMenu = transform.gameObject;
            mainMenuButtons = transform.GetChild(2).gameObject;
            
            
            ZoomSliderInit();
        }

        private void stopAR_AddOnclick(Interactable interactable)
        {
            interactable.OnClick.AddListener(() => stopSpatialExploration());
        }
        


        private void cancel_AddOnClick(Interactable interactable)
        {
            cancelInteractable.OnClick.AddListener(() => OnCancel());
        }

        private void OnCancel()
        {
            var eventArgs = new BackOneEventArgs();
            OnCancelJourney(this, eventArgs);
        }

        private void journey_AddOnClick(Interactable interactable)
        {
            journeyInteractable.OnClick.AddListener(() => OnJStartLogic());
        }
        
        private void OnJStartLogic()
        {
            slider.SetActive(false);
            POIQuery.SetActive(false);
            cancelButton.SetActive(true);
            journeyButton.SetActive(false);
            
            miniMap.SetActive(true);
            
            CurrentPositionInit();

            
            var eventArgs = new BackOneEventArgs();
            OnJourney(this, eventArgs);
            
        }
        
        public void CurrentPositionInit()
        {
            currentMapPin = Instantiate(currentMapPin);
            currentMapPin.transform.parent = miniMap.transform;
        }
        
        public void setCurrentPositionPin(double latitude, double longitude, float heading)
        {
            currentMapPin.Location =  new LatLon(latitude,longitude);
            this.currentLatitude = latitude;
            this.currentLongitude = longitude;
            this.currentheading = heading;
        }

        //Input actions from the user
   
        private void back_AddOnClick(Interactable back_interactable)
        {
            back_interactable.OnClick.AddListener((() => OnBackButtonLogic()));
        }
        
        private void generate_AddOnClick(Interactable generate_interactable)
        {
            generate_interactable.OnClick.AddListener((() => OnGenerateButtonLogic()));
        }

        private void resetBaselAddOnClick(Interactable resetBaselInteractable)
        {
            resetBaselInteractable.OnClick.AddListener((() => OnResetBasel()));
        }

        private void OnResetBasel()
        {
            var mapScene = new MapSceneOfLocationAndZoomLevel(new LatLon(47.559601,7.588576),  14.74f);
            BigMapRenderer.SetMapScene(mapScene);
        }

        //funtions to handle User inputs

        private int ButtonID = 0;
        public void createSelection(POICoordinatesObject poiCoordinatesObject)
        {
            POIQueryVisibility();
            
            GameObject POIButton = (GameObject)Resources.Load("Prefab/POIButton",typeof(GameObject)) ;
            int id = ButtonID;
            POIButton = Instantiate(POIButton);
            POIButton.transform.rotation = transform.GetChild(4).transform.rotation;
            POIButton.GetComponent<ButtonConfigHelper>().MainLabelText = poiCoordinatesObject.ToString();
            POIButton.GetComponent<ButtonAttribute>().setID_coordinates(ButtonID,poiCoordinatesObject);
            POIButton.GetComponent<Interactable>().OnClick.AddListener((() => OnQueryRemoveButtonLogic(POIButton)));
            POIButton.gameObject.transform.parent = transform.GetChild(4).GetChild(1).GetChild(0).transform;

            
            ButtonID++;
            scrollingObjectCollection.UpdateCollection();
        }
        
        private void OnQueryRemoveButtonLogic(GameObject POIButton)
        {
            int buttonID = POIButton.GetComponent<ButtonAttribute>().getID();
            var eventArgs = new RemoveQueryDataArgs(buttonID);
            OnPOIRemove(this, eventArgs);
            Debug.Log("Destroying POIButton " + buttonID);

            scrollingObjectCollection.RemoveItem(POIButton);
            Destroy(POIButton);
            scrollingObjectCollection.UpdateCollection();
            
            


            // foreach (Transform VARIABLE in transform.GetChild(4).GetChild(1).GetChild(0))
            // {
            //     if (id == VARIABLE.gameObject.GetComponent<ButtonAttribute>().getID())
            //     {
            //         if (VARIABLE.gameObject != null)
            //         {
            //             Destroy(VARIABLE.gameObject);
            //             Invoke("updatePOIButtonCollection",0.5f);
            //         }
            //         return;
            //     }
            // }
        }

        public void POIQueryVisibility()
        {
            POIQuery.SetActive(true);
            journeyButton.SetActive(true);
        }
        
        
        private void OnGenerateButtonLogic()
        {
            var eventArgs = new GeneratePinEventArgs();
            OnGeneratePin(this, eventArgs);
        }

        private void OnBackButtonLogic()
        {
            
            var eventArgs = new BackOneEventArgs();
            OnOneBack(this, eventArgs);
        }

        public void MenuVisibility(bool flag)
        {
            transform.gameObject.SetActive(flag);
            stopARMenuButton.SetActive(false);
            mainMenuButtons.SetActive(true);
        }
        
        public void ZoomSliderInit()
        {
            slider.GetComponent<PinchSlider>().OnValueUpdated.AddListener((e) => zoomData(e.NewValue));
        }

        public void zoomData(float data)
        {
            var eventArgs = new ZoomMapEventArgs(data);
            OnZoomMap(this, eventArgs);
        }

        public float CalculateBearing(float pictureheading)
        {
            float diff = pictureheading - currentheading;
            return diff;
        }
        
        public async void RenderGameObject(POICoordinatesObject poiCoordinatesObject)
        {
            GameObject ShowPicture = poiCoordinatesObject.GETGameObject();
            ShowPicture = Instantiate(ShowPicture);
            ShowPicture.transform.localScale= new Vector3(-0.3f,-0.15f,0.004f);
            float difference = CalculateBearing(poiCoordinatesObject.getHeading());
            ShowPicture.transform.rotation = Camera.main.transform.rotation;
            ShowPicture.transform.parent = transform;
            ShowPicture.transform.position = transform.GetChild(9).position;
            ShowPicture.SetActive(true);
            //GameObject ShowPictureOption = (GameObject)Resources.Load("Prefab/ShowResult",typeof(GameObject)) ;
            //ShowPictureOption = Instantiate(ShowPictureOption);
            //Texture texture =  await ResultPanelView.GetRemoteTexture(poiCoordinatesObject.getURL());

            //ShowPicture.GetComponent<Renderer>().sharedMaterial.mainTexture = texture;
            //ShowPicture.transform.SetParent(transform);
          
            SpawnedObjects.Add(poiCoordinatesObject,ShowPicture);
        }

        public void RemoveGameObject(POICoordinatesObject poiCoordinatesObject)
        {
            GameObject gameObject = SpawnedObjects[poiCoordinatesObject];

            foreach (Transform VARIABLE in transform)
            {
                if (VARIABLE.gameObject == gameObject)
                {
                    Destroy(VARIABLE.gameObject);
                    break;
                }
            }
            Debug.Log("Deleted Object");
        }

        public Dictionary<int, POICoordinatesObject> PoiCoordinatesObjects;
        private List<int> keys;


        public void setPOIList(Dictionary<int, POICoordinatesObject> poiCoordinatesObjects)
        {
            this.PoiCoordinatesObjects = poiCoordinatesObjects;
            keys = new List<int>(poiCoordinatesObjects.Keys);
        }
        
        public void MiniMapRender()
        {
            MiniMapRenderer = miniMap.GetComponent<MapRenderer>();

            currentMapPin.Location = new LatLon(currentLatitude,currentLongitude);
            Debug.Log("Here its: " +currentLatitude+currentLongitude);
            
            var mapScene = new MapSceneOfLocationAndZoomLevel(currentMapPin.Location,  MiniMapRenderer.ZoomLevel + 0f);
            //_map.SetMapScene(mapScene);

            MiniMapRenderer.SetMapScene(mapScene);
            //
        }
        
        
        public double calculateRadius(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            const double r = 6371; // meters

            var sdlat = Math.Sin((latitude2 - latitude1) / 2);
            var sdlon = Math.Sin((longitude2 - longitude1) / 2);
            var q = sdlat * sdlat + Math.Cos(latitude1) * Math.Cos(latitude2) * sdlon * sdlon;
            var d = 2 * r * Math.Asin(Math.Sqrt(q));

            return d;
        }
        

        public void SpatialExploration()
        {
            this.journeyStart = true;
            mapMenu.SetActive(true);
            miniMap.SetActive(true);
            CurrentPositionInit();
            mainMenuButtons.SetActive(false);
            slider.SetActive(false);
            stopARMenuButton.SetActive(true);
            Debug.Log(PoiCoordinatesObjects.Count);
            Debug.Log("Journey Started");
        }

        public void stopSpatialExploration()
        {
            this.journeyStart = false;
            miniMap.SetActive(false);
            flushPOIList();
            var EventArgs = new BackOneEventArgs();
            OnCancelJourney(this, EventArgs);
        }

        public void flushPOIList()
        {
            if (SpawnedObjects.Count != 0)
            {
                foreach (var VARIABLE in SpawnedObjects)
                {
                    RemoveGameObject(VARIABLE.Key);
                }
            }
            //PoiCoordinatesObjects.Clear();
        }
        
        
        double distance = 100; // 100km radius
        double inboundthreshold = 2;
        double outboundthreshold = 4;

        private double lat1;
        private double lon1;
        private float heading1;
        private double lat2;
        private double lon2;
        private float myheading;

        private int deleteKey;
        private bool arrived;
        private void Update()
        {
            if (journeyStart)
            {
                foreach (int key in keys)
                {
                    lat1 = PoiCoordinatesObjects[key].getCoordinates().getLat();
                    lon1 = PoiCoordinatesObjects[key].getCoordinates().getLon();
                    heading1 = PoiCoordinatesObjects[key].getHeading();

                    distance = calculateRadius(lat1, lon1, currentLatitude, currentLongitude);
                    DelayLoadLevel();
                    Debug.Log("Key ID: "+key+"  Current Position: "+currentLatitude+" "+currentLongitude + " Destinaton: "+lat1+" "+lon1+" Distance to Dest "+distance.ToString());
                    if (distance < inboundthreshold)
                    {
                        Debug.Log("Arrived");
                        RenderGameObject(PoiCoordinatesObjects[key]);
                        PoiCoordinatesObjects.Remove(key);
                        distance = 100;
                        deleteKey = key;
                        arrived = true;
                    }
                }

                if (arrived)
                {
                    keys.Remove(deleteKey);
                    arrived = false;
                }
            }
        }
        
        private IEnumerator DelayLoadLevel()
        {
            yield return new WaitForSeconds(1f);
        }
    }
}