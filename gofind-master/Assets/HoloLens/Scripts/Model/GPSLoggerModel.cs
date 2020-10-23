using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;

/*
 * Various EventArgs has been created so that if changes in the Model has been made, a callback can be
 * invoked to the controller which then sends it to the view
 */

// Dispatched when GPS coordinates changes
public class GPSCoordinatesChangedEventArgs : EventArgs
{
    public double latitude { get; private set; }
    public double longitude { get; private set; }
    public float heading { get; private set; }
    
    public GPSCoordinatesChangedEventArgs(double latitude, double longitude, float heading)
    {
        this.latitude = latitude;
        this.longitude = longitude;
        this.heading = heading;
        //Debugging
        //Debug.Log("Received event from GPS View");
    }
}

/*
 * Models are used to store information of different UI Menus.
 * Model informations can changed by the controller.
 * An Interface has been also implemented so that the controller han can access only the interface functions
 */

// Interface for the model
public interface IGPSLoggerModel
{
    // Dispatched when the position changes
    event EventHandler<GPSCoordinatesChangedEventArgs> OnGPSDataChanged;

    List<Double> getRawGPSCoordinates();
    string getStringGPSCoordinates();
    
    
    // GPS Position

    void SetGPSCoordinates(double lat, double lon, float hea); // Setting new GPS information in this model


}

// Implementation of the GPSLoggerModel model interface
public class GPSLoggerModel: IGPSLoggerModel
{
    // Backing field for the GPS Position
    private double latitude;
    private double longitude;
    private float heading;
    
/*
 * Eventhandler is used to to send events 
 */
    public event EventHandler<GPSCoordinatesChangedEventArgs> OnGPSDataChanged = (sender, e) => { }; 

    
    // Storing informations
    public double Latitude
    {
        get { return latitude; }
        set
        {
            // Only if the latitude changes
            if (latitude != value)
            {
                // Set new position
                latitude = value;
            }
        }
    }
    
    public double Longitude
    {
        get { return longitude; }
        set
        {
            if (longitude != value)
            {
                longitude = value;
            }
        }
    }
    public float Heading
    {
        get { return heading; }
        set
        {
            if (heading != value)
            {
                heading = value;
            }
        }
    }

    //Setting the information of this model
    public void SetGPSCoordinates(double lat, double lon, float hea)
    {
        latitude = lat;
        longitude = lon;
        heading = hea;
        var eventArgs = new GPSCoordinatesChangedEventArgs(latitude,longitude,heading);
        
        // Dispatch the 'position changed' event
        OnGPSDataChanged(this, eventArgs);
    }

    public string getStringGPSCoordinates()
    {
            return latitude.ToString() + " " + longitude.ToString();
    }

    public List<Double> getRawGPSCoordinates()
    {
        List<Double>coordinates = new List<double>();
        coordinates.Add(latitude);
        coordinates.Add(longitude);
        return coordinates;
    }
}
