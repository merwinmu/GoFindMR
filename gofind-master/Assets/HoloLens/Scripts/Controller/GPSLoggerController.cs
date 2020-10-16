using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public interface IGPSLoggerController
{
}
public class GPSLoggerController : MonoBehaviour, IGPSLoggerController
{
    

    // Keep references to the model and view
    private static  IGPSLoggerModel model;
    private static  IGPSLoggerView view;
    

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
    
    private void DisplayGPSdata()
    {
        
    }
}
