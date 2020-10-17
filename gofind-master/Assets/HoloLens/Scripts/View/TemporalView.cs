using System;
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
    // Dispatched when Years received
    public class YearChangeEventArgs : EventArgs
    {
        public string upperbound { get; private set; }
        public string lowerbound { get; private set; }
       
    
        public YearChangeEventArgs(string upperbound, string lowerbound)
        {
            this.upperbound = upperbound;
            this.lowerbound = lowerbound;

            //Debugging
            //Debug.Log("Received event from Temporal View");
        }
    }
    
    public class MapBackEventArgs : EventArgs
    {
    }

    
    public interface ITemporalView
    {
        event EventHandler<YearChangeEventArgs> OnReceived;
        event EventHandler<MapBackEventArgs> MapBackButton;

        void setGameObject(GameObject gameObject);
        
        //Use Class function using this interface functions
        void MenuVisibility(bool flag);
    }
    
    public class TemporalView: MonoBehaviour, ITemporalView
    {
        private static TMP_InputField lowerboundinput;
        private static TMP_InputField upperboundinput;
        
        private GameObject button0;
        private Button OKbutton;
        private Interactable interactable;
        
        private GameObject button3;
        private Button backButton;
        private Interactable backInteractable;
        public event EventHandler<YearChangeEventArgs> OnReceived  = (sender, e) => { };
        public event EventHandler<MapBackEventArgs> MapBackButton  = (sender, e) => { };



        private void Awake()
        {
            GameObject lo = transform.GetChild(0).GetChild(0).gameObject;
            GameObject up = transform.GetChild(0).GetChild(1).gameObject;
            lowerboundinput = lo.GetComponent<TMP_InputField>();
            upperboundinput = up.GetComponent<TMP_InputField>();
            
            button3 = transform.GetChild(0).GetChild(3).gameObject;
            backInteractable = button3.GetComponent<Interactable>();
            Button3_AddOnClick(backInteractable);
            
            button0 = transform.GetChild(0).GetChild(2).gameObject;
            interactable = button0.GetComponent<Interactable>();
            Button0_AddOnClick(interactable);
            
            this.transform.gameObject.SetActive(false);
        }



        public void setGameObject(GameObject gameObject)
        {
            // Component[] components = inputField.GetComponents(typeof(Component));
            // foreach(Component component in components) {
            //     Debug.Log(component.ToString());
            // }
        }

        
        //INPUT action from the user
        private void Button3_AddOnClick(Interactable interactable)
        {
            interactable.OnClick.AddListener((() => OnBackButtonLogic()));
        }
        
        public void Button0_AddOnClick(Interactable interactable)
        {
            interactable.OnClick.AddListener((() => debug_input_button()));
        }

        public void debug_input_button()
        {
            var eventArgs = new YearChangeEventArgs(upperboundinput.text,lowerboundinput.text);
            OnReceived(this, eventArgs);

        }
        
        private void OnBackButtonLogic()
        {
            var eventArgs = new MapBackEventArgs();
            MapBackButton(this, eventArgs);
        }
        
        public void MenuVisibility(bool flag)
        {
            transform.gameObject.SetActive(flag);
        }

        void Destroy()
        {
            //https://answers.unity.com/questions/938496/buttononclickaddlistener.html
        }
    }
    
}