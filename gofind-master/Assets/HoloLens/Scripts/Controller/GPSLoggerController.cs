using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*
 * Controllers are used for controlling models and views of various classes
 * Events are registered in Controllers, once a event occurs, the controller will trigger the associate functions.
 * Every event must and should be registered in the Controller.
 * Controllers are can also access other controller functions eg. Interface functions
 */
public interface IGPSLoggerController
{
    IGPSLoggerModel GETGPSLoggerModel();
}
public class GPSLoggerController : MonoBehaviour, IGPSLoggerController
{
    

    // Keep references to the model and view
    private static  IGPSLoggerModel model;
    private static  IGPSLoggerView view;
    
    //Initialize Model, view and Listeners
    private void Start()
    {
        model = new GPSLoggerModel();
        view = transform.GetChild(0).GetComponent<GPSLoggerView>();

        // Listen to input from the view
        view.OnReceived += HandleGPSReceived;
        // Listen to changes in the model
        model.OnGPSDataChanged += HandleGPSChanged;
        
        // Set the view's initial state by synching with the model
        DisplayGPSdata();
    }

    // Called when GPSdata is received
    private void HandleGPSReceived(object sender, GPSDataReceivedEventArgs e)
    {
        // Updating the model
        model.SetGPSCoordinates(e.latitude,e.longitude,e.heading);
    }

    // Called when the model's GPS data changed
    private void HandleGPSChanged(object sender, GPSCoordinatesChangedEventArgs e)
    {
        view.setGPSTextMesh(e.latitude,e.longitude,e.heading);
        //Debug.Log("Display Event changed "+e.longitude);
    }
    
    //Not used function
    private void DisplayGPSdata()
    {
        
    }

    public IGPSLoggerModel GETGPSLoggerModel()
    {
        return model;
    }
}
