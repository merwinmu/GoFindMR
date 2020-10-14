using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Dispatched when GPS coordinates changes
public class GPSCoordinatesChangedEventArgs : EventArgs
{
    
}

// Interface for the model
public interface IGPSLoggerModel
{
    // Dispatched when the position changes
    event EventHandler<GPSCoordinatesChangedEventArgs> OnGPSDataChanged;
    
    // GPS Position
    double Latitude { get; set; }
    double Longitude { get; set; }
    float Heading { get; set; }
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
            
            // Dispatch the 'position changed' event
            var eventArgs = new GPSCoordinatesChangedEventArgs();
            OnGPSDataChanged(this, eventArgs);
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
            
            var eventArgs = new GPSCoordinatesChangedEventArgs();
            OnGPSDataChanged(this, eventArgs);
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
            var eventArgs = new GPSCoordinatesChangedEventArgs();
            OnGPSDataChanged(this, eventArgs);
        }
    }
}
