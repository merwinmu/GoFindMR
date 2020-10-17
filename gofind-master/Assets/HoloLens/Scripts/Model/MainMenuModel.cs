using System;
using System.Collections;
using System.Collections.Generic;
using Assets.HoloLens.Scripts.Model;
using UnityEngine;

/*
 * Various EventArgs has been created so that if changes in the Model has been made, a callback can be
 * invoked to the controller which then sends it to the view
 */

public class DataChangedOutputEventArgs : EventArgs
{
    public double data { get; private set; }

    public DataChangedOutputEventArgs(double data)
    {
        this.data = data;
        //Debugging
        //Debug.Log("Received event from GPS View");
    }
}

public class MainMainChangedEventArgs : EventArgs
{
    public bool flag { get; private set; }

    public MainMainChangedEventArgs(bool flag)
    {
        this.flag = flag;
    }
}

/*
 * Models are used to store information of different UI Menus.
 * Model informations can changed by the controller.
 * An Interface has been also implemented so that the controller han can access only the interface functions
 */


// Interface for the model
public interface IMainMenuModel
{
    // Dispatched when the data changes
    event EventHandler<DataChangedOutputEventArgs> DataOutput;
    event EventHandler<MainMainChangedEventArgs> VisibilityChange;
    void setData(double data);
    void ChangeVisibility(bool flag);
    void Camera_query();
    void Temporal_query();
    void CPosition_query();
    void Spatial_query();
    void Search_query();
}

public class MainMenuModel: IMainMenuModel
{
    private double Data;
    private bool showHideMenu = true;
    
    // Dispatched when the position changes
    public event EventHandler<DataChangedOutputEventArgs> DataOutput;
    public event EventHandler<MainMainChangedEventArgs> VisibilityChange;

    double data
    {
        get { return data; }
        set
        {
            // Only if the latitude changes
            if (data != value)
            {
                // Set new position
                data = value;
            }
        }
    }
    
    public void setData(double data)
    {
        Data = data;
    }

    public void ChangeVisibility(bool flag)
    {
        showHideMenu = flag;
        var eventArgs = new MainMainChangedEventArgs(showHideMenu);
        
        // Dispatch the 'position changed' event
        VisibilityChange(this, eventArgs);
    }

    
    /*
     * Calling these functions from the controller
     */
    public void Camera_query()
    {
        Debug.Log("Reached the Model");
    }

    public void Temporal_query()
    {
        Debug.Log("Reached the Model");
    }

    public void CPosition_query()
    {
        Debug.Log("Reached the Model");
    }

    public void Spatial_query()
    {
        Debug.Log("Reached the Model");
    }

    public void Search_query()
    {
        Debug.Log("Reached the Model");
    }
}
