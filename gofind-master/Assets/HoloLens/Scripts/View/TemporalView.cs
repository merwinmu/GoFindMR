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
    }
    
    
    public class TemporalView: MonoBehaviour, ITemporalView
    {
        private static GameObject temporalview;
        private static TMP_InputField lowerboundinput;
        private static TMP_InputField upperboundinput;
        private Button OKbutton;
        
        public event EventHandler<YearChangeEventArgs> OnReceived  = (sender, e) => { };

        public void setGameObject(GameObject gameObject)
        {
            temporalview = gameObject;
            GameObject lo = temporalview.transform.GetChild(1).GetChild(0).gameObject;
            GameObject up = temporalview.transform.GetChild(1).GetChild(1).gameObject;
            lowerboundinput = lo.GetComponent<TMP_InputField>();
            upperboundinput = up.GetComponent<TMP_InputField>();
            
            setOKButton(temporalview.transform.GetChild(1).GetChild(2).gameObject);
            
            // Component[] components = inputField.GetComponents(typeof(Component));
            // foreach(Component component in components) {
            //     Debug.Log(component.ToString());
            // }
        }
        
        public void setOKButton(GameObject gameObject)
        {
            //get Only the listener of the Button
            Interactable interactable = gameObject.GetComponent<Interactable>();
            AddOnClick(interactable);
        }

        private void Update()
        {
            // If the primary mouse button was pressed this frame
            if (Input.GetMouseButtonDown(0))
            {
                //Debug
                //Debug.Log("MouseInput");
                var eventArgs = new YearChangeEventArgs("1995","2020");
                OnReceived(this, eventArgs);
            }
        }
        
        //INPUT
        public void AddOnClick(Interactable interactable)
        {
            interactable.OnClick.AddListener((() => debug_input_button()));
        }

        public void debug_input_button()
        {
            var eventArgs = new YearChangeEventArgs(upperboundinput.text,lowerboundinput.text);
            OnReceived(this, eventArgs);
        }

        void Destroy()
        {
            //https://answers.unity.com/questions/938496/buttononclickaddlistener.html
        }
    }
    
}