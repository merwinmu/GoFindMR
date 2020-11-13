using System;
using System.Collections;
using System.Collections.Generic;
using Assets.HoloLens.Scripts.Properties;
using Assets.HoloLens.Scripts.View;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;


public class QueryCompleteEventArgs : EventArgs
{
    private Dictionary<int, GameObject> querylist;
}

public class QueryRemoveEventArgs : EventArgs
{
    public GameObject RemoveObject;
}
public interface IQueryMenuView
{
    event EventHandler<QueryCompleteEventArgs> OnReceived;
    event EventHandler<QueryRemoveEventArgs> OnRemove;
    event EventHandler<QueryRemoveEventArgs> OnRemoveOnMap;

    void createSelection(POICoordinatesObject poiCoordinatesObject);
    void setVisibility(bool flag);
};
public class QueryMenuView : MonoBehaviour, IQueryMenuView
{
    public event EventHandler<QueryCompleteEventArgs> OnReceived= (sender, e) => { };
    public event EventHandler<QueryRemoveEventArgs> OnRemove= (sender, e) => { };
    public event EventHandler<QueryRemoveEventArgs> OnRemoveOnMap  = (sender, e) => { };


    private GameObject querymenu;
    private GameObject scrollObeObjectCollectionGameObject;
    private ScrollingObjectCollection scrollingObjectCollection;
   
    private Dictionary<int, GameObject> queryList;
    
    private int ButtonID;
    void Start()
    {
        querymenu = transform.gameObject; ;
        transform.gameObject.SetActive(false);
        queryList = new Dictionary<int, GameObject>();
        scrollObeObjectCollectionGameObject = querymenu.transform.GetChild(1).gameObject;
        scrollingObjectCollection = scrollObeObjectCollectionGameObject.GetComponent<ScrollingObjectCollection>();
    }
    public void createSelection(POICoordinatesObject poiCoordinatesObject)
    {
        GameObject POIButton = (GameObject)Resources.Load("Prefab/POIButton",typeof(GameObject)) ;
        POIButton = Instantiate(POIButton);
        POIButton.transform.rotation = transform.rotation;
        POIButton.GetComponent<ButtonConfigHelper>().MainLabelText = poiCoordinatesObject.ToString();
        POIButton.GetComponent<ButtonAttribute>().setID_coordinates(ButtonID,poiCoordinatesObject);
        POIButton.GetComponent<Interactable>().OnClick.AddListener((() => OnQueryRemoveButtonLogic(POIButton)));
        POIButton.gameObject.transform.parent = transform.GetChild(1).GetChild(0);
        queryList[ButtonID] = POIButton;
        ButtonID++;
        scrollingObjectCollection.UpdateCollection();
    }
    
    private void OnQueryRemoveButtonLogic(GameObject POIButton)
    {
        int ID = POIButton.GetComponent<ButtonAttribute>().getID();
        queryList.Remove(ID);
        int buttonID = POIButton.GetComponent<ButtonAttribute>().getID();
        Debug.Log("Destroying POIButton " + buttonID);

        scrollingObjectCollection.RemoveItem(POIButton);
        Destroy(POIButton);
        scrollingObjectCollection.UpdateCollection();
        var eventArgs = new QueryRemoveEventArgs();
        eventArgs.RemoveObject = POIButton;
        OnRemove(this, eventArgs);    
        //TODO OnRemoveMap
            


        // foreach (Transform VARIABLE in transform.GetChild(4).GetChild(1).GetChild(0))
        // {
        //     if (id == VARIABLE.gameObject.GetComponent<ButtonAttribute>().getID())
        //     {
        //         if (VARIABLE.gameObject != null)
        //         {
        //             Destroy(VARIABLE.gameObject);
        //             Invoke("updatePOIButtonCollection",0.5f);
        //         }
        //         return;
        //     }
        // }
    }
   

    public void setVisibility(bool flag)
    {
        querymenu.SetActive(flag);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
