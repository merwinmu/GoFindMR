using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

// Interface for the model
public interface IGPSLoggerModel
{
    // Dispatched when the position changes
    event EventHandler<GPSCoordinatesChangedEventArgs> OnGPSDataChanged;
    
    // GPS Position

    void SetGPSCoordinates(double lat, double lon, float hea);


}

// Implementation of the NearMenu model interface
public class GPSLoggerModel: IGPSLoggerModel
{
    // Backing field for the GPS Position
    private double latitude;
    private double longitude;
    private float heading;
    

    public event EventHandler<GPSCoordinatesChangedEventArgs> OnGPSDataChanged = (sender, e) => { };

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

    public void SetGPSCoordinates(double lat, double lon, float hea)
    {
        latitude = lat;
        longitude = lon;
        heading = hea;
        var eventArgs = new GPSCoordinatesChangedEventArgs(latitude,longitude,heading);
        
        // Dispatch the 'position changed' event
        OnGPSDataChanged(this, eventArgs);
    }
}
