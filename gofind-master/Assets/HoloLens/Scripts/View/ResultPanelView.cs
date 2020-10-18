using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using UnityEngine.UI;

/*
 * Views are primarly used for Input and Output. It is primarly a Monobehaviour class with the associate functions 
 * Input actions such as OnClick invoke an event to the controller which then executes a function to model
 * Output actions are in example rendering gameobjects etc.
 */



/*
 * Various EventArgs has been created so that if an Input occurs , a callback can be
 * invoked to the controller which then sends it to the model
 */

public class ResultBackEventArgs : EventArgs
{
}

public interface IResultPanelView
{
    event EventHandler<ResultBackEventArgs> OnBackButton;
    void Visibility(bool flag);

}
public class ResultPanelView : MonoBehaviour , IResultPanelView
{
    public event EventHandler<ResultBackEventArgs> OnBackButton;
    
    
    private GameObject backButtonObject;
    private Interactable backInteractable;

    private void Start()
    {
        backButtonObject = transform.GetChild(0).GetChild(2).GetChild(3).gameObject;
        backInteractable = backButtonObject.GetComponent<Interactable>();
        BackButton_AddOnClick(backInteractable);
        transform.gameObject.SetActive(false);

    }

    //Input action from the user
    private void BackButton_AddOnClick(Interactable interactable)
    {
        interactable.OnClick.AddListener((() => OnBackButtonLogic()));
    }
    
    private void OnBackButtonLogic()
    {
        var eventArgs = new ResultBackEventArgs();
        OnBackButton(this, eventArgs);
    }

    public void Visibility(bool flag)
    {
        transform.gameObject.SetActive(flag);

    }
}
