using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
* Various EventArgs has been created so that if changes in the Model has been made, a callback can be
* invoked to the controller which then sends it to the view
*/
public class UpdatePicturesEventArgs : EventArgs
{
}

public class ResultVisibilityEventArgs : EventArgs
{
    public bool flag { get; private set; }

    public ResultVisibilityEventArgs(bool flag)
    {
        this.flag = flag;
    }
}
/*
* Models are used to store information of different UI Menus.
* Model informations can changed by the controller.
* An Interface has been also implemented so that the controller han can access only the interface functions
*/
public interface IResultPanelModel
{
    // Dispatched when years changes
    event EventHandler<UpdatePicturesEventArgs> OnUpdatePictures;
    event EventHandler<ResultVisibilityEventArgs> OnResultVisibility;
    /*
                * Eventhandler is used to to send events
                 * This method is used for changing the visibility of the menu
                 */ 
    void ChangeResultVisibility(bool flag);

}
public class ResultPanelModel : IResultPanelModel
{
    public event EventHandler<UpdatePicturesEventArgs> OnUpdatePictures= (sender, e) => { };
    public event EventHandler<ResultVisibilityEventArgs> OnResultVisibility= (sender, e) => { };
    private bool showResult;

    
    public void ChangeResultVisibility(bool flag)
    {
        showResult = flag;
        var eventArgs = new ResultVisibilityEventArgs(showResult);
            
        // Dispatch the 'Result changed' event
        OnResultVisibility(this, eventArgs);
    }
}
