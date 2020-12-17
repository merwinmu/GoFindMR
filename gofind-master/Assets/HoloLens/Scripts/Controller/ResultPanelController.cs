using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Assets.HoloLens.Scripts.Model;
using Assets.HoloLens.Scripts.View;
using UnityEngine;


/*
 * Controllers are used for controlling models and views of various classes
 * Events are registered in Controllers, once a event occurs, the controller will trigger the associate functions.
 * Every event must and should be registered in the Controller.
 * Controllers are can also access other controller functions eg. Interface functions
 */
namespace Assets.HoloLens.Scripts.Controller
{
public interface IResultPanelController
{
    IResultPanelModel GETResultPanelModel();
    IResultPanelView GETResultPanelView();
}
public class ResultPanelController : MonoBehaviour, IResultPanelController
{
    // Keep references to the model and view
    private static IResultPanelModel model;
    private static IResultPanelView view;
    
    public IResultPanelModel GETResultPanelModel()
    {
        return model;
    }

    public IResultPanelView GETResultPanelView()
    {
        return view;
    }

    //Initialize Model, view and Listeners
    void Start()
    {
        model = new ResultPanelModel();
        view = transform.GetChild(6).GetComponent<ResultPanelView>();

        // Listen to input from the view
         view.OnBackButton += HandleBack;
         view.OnSelectPicture += HandlePictureSelect;
         view.OnShowOnMap += ShowMap;
         view.OnMapHide += HideMap;
         view.OnARClick += ARMode;
         view.OnResetObject += DeleteFromQueryMenu;
        //view.OnGeneratePin += HandleGeneratePin;
       
        // Listen to changes in the model
            
        model.OnResultVisibility += ResultStatusVisibility;
        model.OnUpdatePictures += HandleUpdatePicture;
        model.OnReset += HandleReset;
        model.OnEDialog += ShowDialog;
        //model.OnMapPinGenerate += GenerateMapPins;
    }

    private void ShowDialog(object sender, BackEventArgs e)
    {
        view.setAllResultMenuVisibility(false);
        IQueryMenuController controller = gameObject.GetComponent<QueryMenuController>();
        controller.getview().setVisibility(true);
        controller.getview().error();
        controller.getview().setErrorDialogVisibility(true);
        Debug.Log("Error Window showing");
    }

    private void DeleteFromQueryMenu(object sender, ResetObject e)
    {
        IQueryMenuController controller = gameObject.GetComponent<QueryMenuController>();
        controller.getview().Reset();
    }

    private void HandleReset(object sender, ResetEventArgs e)
    {
        view.reset();
    }

    private void ARMode(object sender, GetPOILocationListEventArgs e)
    {
        IMapMenuView mapmenu = GetComponent<MapMenuController>().GETMapMenuView();
        mapmenu.setPOIList(e.poiLocations);
        mapmenu.SpatialExploration();
    }

    private void HideMap(object sender, CancelEventArgs e)
    {
        IMapController mapController = GetComponent<MapController>();
        mapController.GETMapView().setGameObjectVisibility(false);
        mapController.GETMapView().DisableRadial();
    }

    private void ShowMap(object sender, GPSDataReceivedEventArgs e)
    {
        IMapController mapController = GetComponent<MapController>();
        mapController.GETMapView().setGameObjectVisibility(true);
        mapController.GETMapView().EnableRadial();
        mapController.GETMapView().ZoomIntoPoint(e.latitude,e.longitude);
    }
    

    private void HandlePictureSelect(object sender, SelectResultPictureDataArgs e)
    {
        model.SetCurrentPicture(e.get());
    }

    private bool second;
    private void HandleUpdatePicture(object sender, UpdatePicturesEventArgs e)
    {
        if (!second)
        {
            view.setTextures(e.getPictureData());
            second = true;
        }
        
        else
        {
            view.setActiveTextures(e.getKeys());
        }
        
    }

    private void HandleBack(object sender, ResultBackEventArgs e)
    {
        IMainMenuModel mainMenuModel = transform.GetComponent<MainMenuController>().GETMainMenuModel();
        model.ChangeResultVisibility(false);
        mainMenuModel.ChangeVisibility(true);
        IMainMenuView mainMenuView = transform.GetComponent<MainMenuController>().GETMainMenuView();
        mainMenuView.activateShow(true);
        
        Debug.Log("Search clicked");
    }

    //Functions to call once an Event occurs
    
    //UI Hide
    private void ResultStatusVisibility(object sender, ResultVisibilityEventArgs e)
    {
        view.Visibility(e.flag);
    }
}
}