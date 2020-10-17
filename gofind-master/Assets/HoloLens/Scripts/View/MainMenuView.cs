using System;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEngine;

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
    public class InputDataEventArgs : EventArgs
    {
        public double data { get; private set; }

        public InputDataEventArgs(double data)
        {
            this.data = data;
            //Debugging
            //Debug.Log("Received event from GPS View");
        }
    }
    
    // EventArgs for each Query options
    public class CameraEventArgs : EventArgs
    {
    }
    
    public class SpatialEventArgs : EventArgs
    {
    }
    
    public class CPositionEventArgs : EventArgs
    {
    }
    
    public class TemporalEventArgs : EventArgs
    {
    }
    
    public class SearchEventArgs : EventArgs
    {
    }
    
    
    
    public interface IMainMenuView
    {
        event EventHandler<InputDataEventArgs> OnInputDataReceived;
        event EventHandler<CameraEventArgs> OnCameraSelect;
        event EventHandler<SpatialEventArgs> OnSpatialSelect;
        event EventHandler<CPositionEventArgs> OnCPositionSelect;
        event EventHandler<TemporalEventArgs> OnTemporalSelect;
        event EventHandler<SearchEventArgs> OnSearchSelect;


        void setViewproperties();

        void SetMainMenuVisibility(bool flag);
    }
    
    public class MainMenuView : MonoBehaviour, IMainMenuView 
    {
       
        public event EventHandler<InputDataEventArgs> OnInputDataReceived = (sender, e) => { };
        public event EventHandler<CameraEventArgs> OnCameraSelect= (sender, e) => { };
        public event EventHandler<SpatialEventArgs> OnSpatialSelect= (sender, e) => { };
        public event EventHandler<CPositionEventArgs> OnCPositionSelect= (sender, e) => { };
        public event EventHandler<TemporalEventArgs> OnTemporalSelect= (sender, e) => { };
        public event EventHandler<SearchEventArgs> OnSearchSelect= (sender, e) => { };
        
        //Init GameObjects
        private GameObject Camera_button;
        private Interactable Camera_interactable;
        
        private GameObject Spatial_button;
        private Interactable Spatial_interactable;
        
        private GameObject CPosition_button;
        private Interactable CPosition_interactable;
       
        private GameObject Temporal_button;
        private Interactable Temporal_interactable;
        
        private GameObject Search_button;
        private Interactable Search_interactable;


        private void Awake()
        {
            Camera_button = transform.GetChild(2).GetChild(0).gameObject;
            Camera_interactable = Camera_button.GetComponent<Interactable>();
            Camera_button_AddOnClick(Camera_interactable);
            
            Spatial_button = transform.GetChild(2).GetChild(1).gameObject;
            Spatial_interactable = Spatial_button.GetComponent<Interactable>();
            Spatial_button_AddOnClick(Spatial_interactable);
            
            CPosition_button = transform.GetChild(2).GetChild(2).gameObject;
            CPosition_interactable = CPosition_button.GetComponent<Interactable>();
            CPosition_button_AddOnClick(CPosition_interactable);
            
            Temporal_button = transform.GetChild(2).GetChild(3).gameObject;
            Temporal_interactable = Temporal_button.GetComponent<Interactable>();
            Temporal_button_AddOnClick(Temporal_interactable);
            
            Search_button = transform.GetChild(2).GetChild(4).gameObject;
            Search_interactable = Search_button.GetComponent<Interactable>();
            Search_button_AddOnClick(Search_interactable);
        }
        

        //INPUT actions from the user
        private void Camera_button_AddOnClick(Interactable cameraInteractable)
        {
            cameraInteractable.OnClick.AddListener((() => OnCameraButtonLogic()));
        }
        
        private void Spatial_button_AddOnClick(Interactable spatialInteractable)
        {
            spatialInteractable.OnClick.AddListener((() => OnSpatialButtonLogic()));
        }
        
        private void CPosition_button_AddOnClick(Interactable cPositionInteractable)
        {
            cPositionInteractable.OnClick.AddListener((() => OnCPositionButtonLogic()));
        }
        
        private void Temporal_button_AddOnClick(Interactable temporalInteractable)
        {
            temporalInteractable.OnClick.AddListener((() => OnTemporalButtonLogic()));
        }
        
        private void Search_button_AddOnClick(Interactable searchInteractable)
        {
            searchInteractable.OnClick.AddListener((() => OnSearchButtonLogic()));
        }
        
        //funtions to handle User inputs

        public void OnCameraButtonLogic()
        {
            var eventArgs = new CameraEventArgs();
            OnCameraSelect(this, eventArgs);
            
        }
        
        public void OnSpatialButtonLogic()
        {
            var eventArgs = new SpatialEventArgs();
            OnSpatialSelect(this, eventArgs);
        }
        
        public void OnCPositionButtonLogic()
        {
            var eventArgs = new CPositionEventArgs();
            OnCPositionSelect(this, eventArgs);
        }
        
        public void OnTemporalButtonLogic()
        {
            var eventArgs = new TemporalEventArgs();
            OnTemporalSelect(this, eventArgs);
        }
        
        public void OnSearchButtonLogic()
        {
            var eventArgs = new SearchEventArgs();
            OnSearchSelect(this, eventArgs);
        }
        
        
        public void setViewproperties()
        {
            throw new NotImplementedException();
        }

        public void SetMainMenuVisibility(bool flag)
        {
            this.transform.gameObject.SetActive(flag);
        }
    }
}