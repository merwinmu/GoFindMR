using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Various EventArgs has been created so that if changes in the Model has been made, a callback can be
 * invoked to the controller which then sends it to the view
 */

public class MapMenuDataChangedOutputEventArgs : EventArgs
{
    public bool flag { get; private set; }
    public MapMenuDataChangedOutputEventArgs(bool flag)
    {
        this.flag = flag;
    }
}

public class NotifyMapModel_GenerateEventArgs : EventArgs
{
}

/*
 * Models are used to store information of different UI Menus.
 * Model informations can changed by the controller.
 * An Interface has been also implemented so that the controller han can access only the interface functions
 */
public interface IMapMenuModel
{
    // Dispatched when the position changes
    event EventHandler<MapMenuDataChangedOutputEventArgs> VisibilityChange;
    event EventHandler<NotifyMapModel_GenerateEventArgs> OnMapPinGenerate;
    void ChangeVisibility(bool flag);
    void MapPinGenerate();

}

public class MapMenuModel: IMapMenuModel
{
    private bool showHideMenu;
    public event EventHandler<MapMenuDataChangedOutputEventArgs> VisibilityChange = (sender, e) => { };
    public event EventHandler<NotifyMapModel_GenerateEventArgs> OnMapPinGenerate = (sender, e) => { };

    /*
     * Eventhandler is used to to send events
     * This method is used for changing the visibility of the menu
     */ 

    //T
    public void ChangeVisibility(bool flag)
    {
        showHideMenu = flag;
        var eventArgs = new MapMenuDataChangedOutputEventArgs(showHideMenu);
            
        // Dispatch the 'position changed' event
        VisibilityChange(this, eventArgs);
    }

    public void MapPinGenerate()
    {
        var eventArgs = new NotifyMapModel_GenerateEventArgs();
        
        // Dispatch the 'position changed' event
        OnMapPinGenerate(this, eventArgs);

    }
}
