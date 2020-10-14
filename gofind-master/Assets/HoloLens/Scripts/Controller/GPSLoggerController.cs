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
    private GameObject IO_System;

    // Keep references to the model and view
    private static  IGPSLoggerModel model;
    private static  IGPSLoggerView view;

    private void Awake()
    {
        IO_System = GameObject.FindWithTag("IOSystem");
    }

    private void Start()
    {
        model = new GPSLoggerModel();
        view = IO_System.GetComponent<GPSLoggerView>();
        
        view.setTextMeshPro(GetComponent<TextMeshPro>());
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
        model.Latitude = e.latitude;
        model.Longitude = e.longitude;
        model.Heading = e.heading;
        Debug.Log("Event changed "+model.Longitude);
    }

    // Called when the model's GPS data changed
    private void HandleGPSChanged(object sender, GPSCoordinatesChangedEventArgs e)
    {
        DisplayGPSdata();
    }
    
    private void DisplayGPSdata()
    {
        //Debug.Log("Event changed "+model.Longitude);
        view.setGPSTextMesh(model.Latitude,model.Longitude,model.Heading);
    }
}
