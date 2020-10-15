using System;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    
    public interface ITemporalView
    {
        event EventHandler<YearChangeEventArgs> OnReceived;
        void setGameObject(GameObject gameObject);
        void MenuVisibility(bool flag);
    }
    
    public class TemporalView: MonoBehaviour, ITemporalView
    {
        private static TMP_InputField lowerboundinput;
        private static TMP_InputField upperboundinput;
        
        private GameObject button0;
        private Button OKbutton;
        private Interactable interactable;
        
        public event EventHandler<YearChangeEventArgs> OnReceived  = (sender, e) => { };


        private void Awake()
        {
            GameObject lo = transform.GetChild(1).GetChild(0).gameObject;
            GameObject up = transform.GetChild(1).GetChild(1).gameObject;
            lowerboundinput = lo.GetComponent<TMP_InputField>();
            upperboundinput = up.GetComponent<TMP_InputField>();
            
            button0 = transform.GetChild(1).GetChild(2).gameObject;
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

        private void Update()
        {
            
        }
        
        //INPUT
        public void Button0_AddOnClick(Interactable interactable)
        {
            interactable.OnClick.AddListener((() => debug_input_button()));
        }

        public void debug_input_button()
        {
            var eventArgs = new YearChangeEventArgs(upperboundinput.text,lowerboundinput.text);
            OnReceived(this, eventArgs);

        }
        
        public void MenuVisibility(bool flag)
        {
            this.transform.gameObject.SetActive(flag);
        }

        void Destroy()
        {
            //https://answers.unity.com/questions/938496/buttononclickaddlistener.html
        }
    }
    
}