using System;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;

namespace Assets.HoloLens.Scripts.View
{
    
    public class BackOneEventArgs : EventArgs
    {
    }

    public interface IMapMenuView
    {
        event EventHandler<BackOneEventArgs> OnOneBack;
        void MenuVisibility(bool flag);

    }
    
    public class MapMenuView : MonoBehaviour, IMapMenuView
    {
        public event EventHandler<BackOneEventArgs> OnOneBack = (sender, e) => { };
        
        private bool MainMenuStatus;
         
        private GameObject back_button;
        private Interactable back_interactable;

        private void Start()
        {
            
            transform.gameObject.SetActive(false);

            back_button = transform.GetChild(2).GetChild(3).gameObject;
            back_interactable = back_button.GetComponent<Interactable>();
            back_AddOnClick(back_interactable);
        }
        
        private void back_AddOnClick(Interactable back_interactable)
        {
            back_interactable.OnClick.AddListener((() => OnBackButtonLogic()));
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