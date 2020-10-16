using System.Collections;
using System.Collections.Generic;
using Assets.HoloLens.Scripts.View;
using UnityEngine;

namespace Assets.HoloLens.Scripts.Controller
{
    public interface IMapMenuController
    {
        IMapMenuModel GETMapMenuModel();
    }

    public class MapMenuController : MonoBehaviour, IMapMenuController
    {
        private static IMapMenuModel model;
        private static IMapMenuView view;


        //private static IMapMenuView view;
        // Start is called before the first frame update

        public IMapMenuModel GETMapMenuModel()
        {
            return model;
        }

        void Start()
        {
            model = new MapMenuModel();
            view = transform.GetChild(4).GetComponent<MapMenuView>();

            // Listen to input from the view
            view.OnOneBack += HandleBack;
            // Listen to changes in the model
            model.VisibilityChange += MenuStatusVisibility;

        }

        private void MenuStatusVisibility(object sender, MapMenuDataChangedOutputEventArgs e)
        {
            view.MenuVisibility(e.flag);
        }


        private void HandleBack(object sender, BackOneEventArgs e)
        {
            IMainMenuModel mainMenuModel = transform.GetComponent<MainMenuController>().GETMainMenuModel();
            model.ChangeVisibility(false);
            mainMenuModel.ChangeVisibility(true);
        }
    }
}