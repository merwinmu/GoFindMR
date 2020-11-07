using System;
using System.Collections.Generic;
using Assets.HoloLens.Scripts.Properties;
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
        
        
        void MenuVisibility(bool flag);

        void createSelection(POICoordinatesObject poiCoordinatesObject);
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


        private bool MainMenuStatus;
         
        //Buttons init
        private GameObject back_button;
        private Interactable back_interactable;
        
        private GameObject generate_button;
        private Interactable generate_interactable;

        private GameObject query0_button;
        private Interactable query0_interactable;
        
        private GameObject scrollObeObjectCollectionGameObject;
        private ScrollingObjectCollection scrollingObjectCollection;

        private List<GameObject> POIButtonList;
       
        

        private void Start()
        {
            POIButtonList = new List<GameObject>();
            
            transform.gameObject.SetActive(false);
            
            generate_button = transform.GetChild(2).GetChild(0).gameObject;
            generate_interactable = generate_button.GetComponent<Interactable>();
            generate_AddOnClick(generate_interactable);

            back_button = transform.GetChild(2).GetChild(1).gameObject;
            back_interactable = back_button.GetComponent<Interactable>();
            back_AddOnClick(back_interactable);

            scrollObeObjectCollectionGameObject = transform.GetChild(4).GetChild(1).gameObject;
            scrollingObjectCollection = scrollObeObjectCollectionGameObject.GetComponent<ScrollingObjectCollection>();
            
            ZoomSliderInit();
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
            transform.GetChild(4).gameObject.SetActive(true);
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
            GameObject slider = transform.GetChild(5).gameObject;
            slider.GetComponent<PinchSlider>().OnValueUpdated.AddListener((e) => zoomData(e.NewValue));
        }

        public void zoomData(float data)
        {
            var eventArgs = new ZoomMapEventArgs(data);
            OnZoomMap(this, eventArgs);
        }
    }
}