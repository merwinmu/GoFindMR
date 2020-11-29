using System;
using System.Collections.Generic;
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
    public class RemoveQueryDataArgs : EventArgs
    {
        private int id;

        public RemoveQueryDataArgs(int id)
        {
            this.id = id;
        }

        public int getID()
        {
            return id;
        }
    }
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
    
    public class QueryOptionEventArgs : EventArgs
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
        event EventHandler<QueryOptionEventArgs> OnCPositionSelect;
        event EventHandler<TemporalEventArgs> OnTemporalSelect;
        event EventHandler<SearchEventArgs> OnSearchSelect;
        event EventHandler<RemoveQueryDataArgs> OnRemove;
        event EventHandler<BackEventArgs> OnShow;
        
        void setViewproperties();
        void HideQueryOption(bool flag);
        void activateShow(bool flag);
        List<GameObject> getQueryButtons();

        void SetMainMenuVisibility(bool flag);
        void updateQueryButtonData(string getData);
    }
    
    public class MainMenuView : MonoBehaviour, IMainMenuView 
    {
       
        public event EventHandler<InputDataEventArgs> OnInputDataReceived = (sender, e) => { };
        public event EventHandler<CameraEventArgs> OnCameraSelect= (sender, e) => { };
        public event EventHandler<SpatialEventArgs> OnSpatialSelect= (sender, e) => { };
        public event EventHandler<QueryOptionEventArgs> OnCPositionSelect= (sender, e) => { };
        public event EventHandler<TemporalEventArgs> OnTemporalSelect= (sender, e) => { };
        public event EventHandler<SearchEventArgs> OnSearchSelect= (sender, e) => { };
        public event EventHandler<RemoveQueryDataArgs> OnRemove= (sender, e) => { };
        
        public event EventHandler<BackEventArgs> OnShow= (sender, e) => { };


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

        private GameObject Show;
        private GameObject Show_Button;
        private Interactable Show_Interactable;


        private GameObject query0_button;
        private Interactable query0_interactable;
        
        private GameObject query1_button;
        private Interactable query1_interactable;
        
        private GameObject query2_button;
        private Interactable query2_interactable;

        private List<GameObject> QueryButtonList;
        private GameObject OptionBackground;

        private void Awake()
        {
            QueryButtonList = new List<GameObject>();
            
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
            
            Show = transform.GetChild(5).gameObject;
            Show_Button = transform.GetChild(5).GetChild(1).GetChild(0).gameObject;
            Show_Interactable = Show_Button.GetComponent<Interactable>();
            Show_button_AddOnClick(Show_Interactable);
            
            query0_button = transform.GetChild(2).GetChild(5).gameObject;
            query0_interactable = query0_button.GetComponent<Interactable>();
            query1_AddOnClick(query0_interactable);
            QueryButtonList.Add(query0_button);
            
            query1_button = transform.GetChild(2).GetChild(6).gameObject;
            query1_interactable = query1_button.GetComponent<Interactable>();
            query2_AddOnClick(query1_interactable);
            QueryButtonList.Add(query1_button);
            
            query2_button = transform.GetChild(2).GetChild(7).gameObject;
            query2_interactable = query2_button.GetComponent<Interactable>();
            query3_AddOnClick(query2_interactable);
            QueryButtonList.Add(query2_button);

            foreach (var VARIABLE in QueryButtonList)
            {
                VARIABLE.SetActive(false);
            }

            OptionBackground = transform.GetChild(4).gameObject;
            OptionBackground.SetActive(false);
        }

        private void Show_button_AddOnClick(Interactable showInteractable)
        {
            showInteractable.OnClick.AddListener((() => ShowAgain()));
        }

        private void ShowAgain()
        {
            var eventArgs = new BackEventArgs();
            OnShow(this, eventArgs);
        }

        public void activateShow(bool flag)
        {
            Show.SetActive(flag);
        }


        //INPUT actions from the user
        
        private void query3_AddOnClick(Interactable query3Interactable)
        {
            query3Interactable.OnClick.AddListener((() => RemoveQueryButton2()));
        }

        private void query2_AddOnClick(Interactable query2Interactable)
        {
            query2Interactable.OnClick.AddListener((() => RemoveQueryButton1()));
        }

        private void query1_AddOnClick(Interactable query1Interactable)
        {
            query1Interactable.OnClick.AddListener((() => RemoveQueryButton0()));
        }
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
        
        private void RemoveQueryButton2()
        {
            var eventArgs = new RemoveQueryDataArgs(2);
            OnRemove(this, eventArgs);
            query2_button.SetActive(false);
        }
        private void RemoveQueryButton1()
        {
            var eventArgs = new RemoveQueryDataArgs(1);
            OnRemove(this, eventArgs);
            query1_button.SetActive(false);

        }
        
        private void RemoveQueryButton0()
        {
            var eventArgs = new RemoveQueryDataArgs(0);
            OnRemove(this, eventArgs);
            query0_button.SetActive(false);
            
            OptionBackground.SetActive(false); // Set the background panel to false once no options are active
        }

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
            var eventArgs = new QueryOptionEventArgs();
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

        public void updateQueryButtonData(string data)
        {
            foreach (var VARIABLE in getQueryButtons())
            {
                if(!VARIABLE.activeSelf)
                {
                    VARIABLE.transform.GetChild(3).GetChild(0).GetComponent<TextMeshPro>().text = data;
                    VARIABLE.SetActive(true);
                    return;
                }
            }
        }

        public List<GameObject> getQueryButtons()
        {
            return QueryButtonList;
        }

        public void HideQueryOption(bool flag)
        {
            OptionBackground.SetActive(flag);
        }
    }
}