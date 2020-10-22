using System.Collections;
using System.Collections.Generic;
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
    
    //Initialize Model, view and Listeners
    void Start()
    {
        model = new ResultPanelModel();
        view = transform.GetChild(6).GetComponent<ResultPanelView>();

        // Listen to input from the view
         view.OnBackButton += HandleBack;
        //view.OnGeneratePin += HandleGeneratePin;
       
        // Listen to changes in the model
            
        model.OnResultVisibility += ResultStatusVisibility;
        model.OnUpdatePictures += HandleUpdatePicture;
        //model.OnMapPinGenerate += GenerateMapPins;
    }

    private void HandleUpdatePicture(object sender, UpdatePicturesEventArgs e)
    {
       view.setTextures(e.getPictureData());
    }

    private void HandleBack(object sender, ResultBackEventArgs e)
    {
        IMainMenuModel mainMenuModel = transform.GetComponent<MainMenuController>().GETMainMenuModel();
        model.ChangeResultVisibility(false);
        mainMenuModel.ChangeVisibility(true);
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