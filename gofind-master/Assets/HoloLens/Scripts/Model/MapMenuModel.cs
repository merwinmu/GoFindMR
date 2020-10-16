using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMenuDataChangedOutputEventArgs : EventArgs
{
    public bool flag { get; private set; }

    public MapMenuDataChangedOutputEventArgs(bool flag)
    {
        this.flag = flag;
    }
}

public interface IMapMenuModel
{
    // Dispatched when the position changes
    event EventHandler<MapMenuDataChangedOutputEventArgs> VisibilityChange;
    void ChangeVisibility(bool flag);

}

public class MapMenuModel: IMapMenuModel
{
    private bool showHideMenu;

    
    public event EventHandler<MapMenuDataChangedOutputEventArgs> VisibilityChange= (sender, e) => { };
    public void ChangeVisibility(bool flag)
    {
        showHideMenu = flag;
        var eventArgs = new MapMenuDataChangedOutputEventArgs(showHideMenu);
            
        // Dispatch the 'position changed' event
        VisibilityChange(this, eventArgs);
    }
}
