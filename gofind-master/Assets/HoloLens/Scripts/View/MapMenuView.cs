using System;
using Microsoft.MixedReality.Toolkit.UI;
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
        void MenuVisibility(bool flag);

    }
    
    public class MapMenuView : MonoBehaviour, IMapMenuView
    {
        public event EventHandler<BackOneEventArgs> OnOneBack = (sender, e) => { };
        public event EventHandler<GeneratePinEventArgs> OnGeneratePin= (sender, e) => { };

        private bool MainMenuStatus;
         
        //Buttons init
        private GameObject back_button;
        private Interactable back_interactable;
        
        private GameObject generate_button;
        private Interactable generate_interactable;

        private void Start()
        {
            
            transform.gameObject.SetActive(false);
            
            generate_button = transform.GetChild(2).GetChild(0).gameObject;
            generate_interactable = generate_button.GetComponent<Interactable>();
            generate_AddOnClick(generate_interactable);

            back_button = transform.GetChild(2).GetChild(3).gameObject;
            back_interactable = back_button.GetComponent<Interactable>();
            back_AddOnClick(back_interactable);
        }
        
        //Input actions from the user
        private void back_AddOnClick(Interactable back_interactable)
        {
            back_interactable.OnClick.AddListener((() => OnBackButtonLogic()));
        }
        
        private void generate_AddOnClick(Interactable back_interactable)
        {
            generate_interactable.OnClick.AddListener((() => OnGenerateButtonLogic()));
        }

        //funtions to handle User inputs
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
    }
}