using System;
using System.Collections.Generic;
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

        public double lat = 47.5569389;

        public double lon = 7.5888067;
        
        

        private bool MainMenuStatus;
         
        //Buttons init
        private GameObject back_button;
        private Interactable back_interactable;
        
        private GameObject generate_button;
        private Interactable generate_interactable;

        private GameObject query0_button;
        private Interactable query0_interactable;
        
        private GameObject journeyButton;
        private Interactable journeyInteractable;
        
        private GameObject scrollObeObjectCollectionGameObject;
        private ScrollingObjectCollection scrollingObjectCollection;

        private GameObject slider;
        private GameObject POIQuery;

        private GameObject cancelButton;
        private Interactable cancelInteractable;
        private GameObject miniMap;
        

        private void Start()
        {
            
            transform.gameObject.SetActive(false);
            
            generate_button = transform.GetChild(2).GetChild(0).gameObject;
            generate_interactable = generate_button.GetComponent<Interactable>();
            generate_AddOnClick(generate_interactable);

            back_button = transform.GetChild(2).GetChild(1).gameObject;
            back_interactable = back_button.GetComponent<Interactable>();
            back_AddOnClick(back_interactable);

            journeyButton = transform.GetChild(6).gameObject;
            journeyInteractable = journeyButton.GetComponent<Interactable>();
            journey_AddOnClick(journeyInteractable);
            
            cancelButton = transform.GetChild(7).gameObject;
            cancelInteractable = cancelButton.GetComponent<Interactable>();
            cancel_AddOnClick(cancelInteractable);

            scrollObeObjectCollectionGameObject = transform.GetChild(4).GetChild(1).gameObject;
            scrollingObjectCollection = scrollObeObjectCollectionGameObject.GetComponent<ScrollingObjectCollection>();
            
            slider = transform.GetChild(5).gameObject;
            POIQuery = transform.GetChild(4).gameObject;
            miniMap = transform.GetChild(8).gameObject;


            
            ZoomSliderInit();
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
        
        public async void RenderGameObject(POICoordinatesObject poiCoordinatesObject)
        {
            GameObject ShowPicture = transform.GetChild(9).gameObject;
            ShowPicture = Instantiate(ShowPicture);
            ShowPicture.transform.position = transform.GetChild(9).position;
            ShowPicture.SetActive(true);
            //GameObject ShowPictureOption = (GameObject)Resources.Load("Prefab/ShowResult",typeof(GameObject)) ;
            //ShowPictureOption = Instantiate(ShowPictureOption);
            Texture texture =  await ResultPanelView.GetRemoteTexture(poiCoordinatesObject.getURL());

            ShowPicture.GetComponent<Renderer>().sharedMaterial.mainTexture = texture;
            ShowPicture.transform.SetParent(transform);
          
            SpawnedObjects.Add(poiCoordinatesObject,ShowPicture);
        }

        public void RemoveGameObject(POICoordinatesObject poiCoordinatesObject)
        {
            GameObject gameObject = SpawnedObjects[poiCoordinatesObject];

            foreach (Transform VARIABLE in transform)
            {
                if (VARIABLE.gameObject == gameObject)
                {
                    Destroy(VARIABLE);
                    break;
                }
            }
            
            SpawnedObjects.Remove(poiCoordinatesObject);
        }

        public void debugCoordinates()
        {
            MapRenderer mapRenderer = miniMap.GetComponent<MapRenderer>();

            currentMapPin.Location = new LatLon(lat,lon);
            Debug.Log("Here its: " +lat+lon);
            
            var mapScene = new MapSceneOfLocationAndZoomLevel(currentMapPin.Location,  mapRenderer.ZoomLevel + 0f);
            //_map.SetMapScene(mapScene);

            mapRenderer.SetMapScene(mapScene);
            
            var eventArgs = new GPSDataReceivedEventArgs(lat,lon,0f);
            
            OnDebugReceived(this, eventArgs);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                debugCoordinates();
            }

        }
    }
}