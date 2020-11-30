using System;
using System.Collections.Generic;
using Assets.HoloLens.Scripts.Properties;
using Assets.HoloLens.Scripts.View;
using Microsoft.Maps.Unity;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;


public class QueryCompleteEventArgs : EventArgs
{
    private Dictionary<int, GameObject> querylist;
}

public class QueryRemoveEventArgs : EventArgs
{
    public MapPin RemoveObject;
}
public interface IQueryMenuView
{
    event EventHandler<QueryCompleteEventArgs> OnReceived;
    event EventHandler<QueryRemoveEventArgs> OnRemove;
    event EventHandler<QueryRemoveEventArgs> OnRemoveOnMap;
    event EventHandler<BackEventArgs> OnErrorBack;
    event EventHandler<BackEventArgs> OnSearch;

    void createSelection(POICoordinatesObject poiCoordinatesObject);
    void setVisibility(bool flag);
    void setQueryMenuRadialPosition(Vector3 pos, bool flag);
    void setQueryMenuPosition(Vector3 pos);
    void Reset();
    Vector3 getInitQueryMenuPosition();
    void setErrorDialogVisibility(bool flag);
    void error();
};
public class QueryMenuView : MonoBehaviour, IQueryMenuView
{
    public event EventHandler<QueryCompleteEventArgs> OnReceived= (sender, e) => { };
    public event EventHandler<QueryRemoveEventArgs> OnRemove= (sender, e) => { };
    public event EventHandler<QueryRemoveEventArgs> OnRemoveOnMap  = (sender, e) => { };
    public event EventHandler<BackEventArgs> OnSearch  = (sender, e) => { };
    public event EventHandler<BackEventArgs> OnErrorBack  = (sender, e) => { };

    private GameObject searchButton;
    private GameObject querymenu;
    private GameObject scrollObeObjectCollectionGameObject;
    private GameObject ErrorDialog;
    private GameObject ErrorButton;
    private ScrollingObjectCollection scrollingObjectCollection;
    private Vector3 queryMenuPosition;
    private RadialView initRadialView;
   
    private Dictionary<int, GameObject> queryList;
    
    private int ButtonID;
    void Start()
    {
        querymenu = transform.gameObject; ;
        transform.gameObject.SetActive(false);
        queryMenuPosition = GetComponent<SolverHandler>().AdditionalOffset;
        initRadialView = GetComponent<RadialView>();
        queryList = new Dictionary<int, GameObject>();
        scrollObeObjectCollectionGameObject = querymenu.transform.GetChild(2).gameObject;
        scrollingObjectCollection = scrollObeObjectCollectionGameObject.GetComponent<ScrollingObjectCollection>();
        searchButton = transform.GetChild(3).gameObject;
        searchButton.GetComponent<Interactable>().OnClick.AddListener((() => searchClick()));
        ErrorDialog = transform.GetChild(8).gameObject;
        ErrorDialog.SetActive(false);
        ErrorButton = transform.GetChild(8).GetChild(2).GetChild(0).gameObject;
        ErrorButton.GetComponent<Interactable>().OnClick.AddListener((() => backClick()));
    }

    public void backClick()
    {
        var EventArgs = new BackEventArgs();
        OnErrorBack(this, EventArgs);
    }

    public void searchClick()
    {
        var EventArgs = new BackEventArgs();
        OnSearch(this, EventArgs);
    }
    public void createSelection(POICoordinatesObject poiCoordinatesObject)
    {
        GameObject POIButton = (GameObject)Resources.Load("Prefab/POIButton",typeof(GameObject)) ;
        POIButton = Instantiate(POIButton);
        POIButton.transform.rotation = transform.rotation;
        POIButton.GetComponent<ButtonConfigHelper>().MainLabelText = poiCoordinatesObject.ToString();
        POIButton.GetComponent<ButtonAttribute>().setID_coordinates(ButtonID,poiCoordinatesObject);
        POIButton.GetComponent<Interactable>().OnClick.AddListener((() => OnQueryRemoveButtonLogic(POIButton,poiCoordinatesObject)));
        POIButton.gameObject.transform.parent = transform.GetChild(2).GetChild(0);
        queryList[ButtonID] = POIButton;
        ButtonID++;
        scrollingObjectCollection.UpdateCollection();
    }
    
    private void OnQueryRemoveButtonLogic(GameObject POIButton, POICoordinatesObject poiCoordinatesObject)
    {
        int ID = POIButton.GetComponent<ButtonAttribute>().getID();
        queryList.Remove(ID);
        int buttonID = POIButton.GetComponent<ButtonAttribute>().getID();
        Debug.Log("Destroying POIButton " + buttonID);

        scrollingObjectCollection.RemoveItem(POIButton);
        Destroy(POIButton);
        scrollingObjectCollection.UpdateCollection();

        var eventArgs = new QueryRemoveEventArgs();
        eventArgs.RemoveObject = poiCoordinatesObject.getMapPin();
        OnRemove(this, eventArgs);
        
        
            
        
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

    public void Reset()
    {
        foreach (var VARIABLE in queryList)
        {
            Destroy(VARIABLE.Value);
        }
        queryList.Clear();
    }
    
    public void setQueryMenuRadialPosition(Vector3 pos, bool flag)
    {
        GetComponent<SolverHandler>().AdditionalOffset = pos;
        GetComponent<RadialView>().enabled = flag;
    }

    public void setErrorDialogVisibility(bool flag)
    {
        ErrorDialog.SetActive(flag);
    }

    public void setQueryMenuPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public Vector3 getInitQueryMenuPosition()
    {
        GetComponent<RadialView>().MaxDistance = initRadialView.MaxDistance;
        GetComponent<RadialView>().MinDistance = initRadialView.MinDistance;
        GetComponent<RadialView>().MaxViewDegrees = initRadialView.MaxViewDegrees;
        GetComponent<RadialView>().MinViewDegrees = initRadialView.MinViewDegrees;

        return queryMenuPosition;
    }
    public void setVisibility(bool flag)
    {
        querymenu.SetActive(flag);
        
        foreach (Transform transform in transform)
        {
            if (transform.gameObject != ErrorDialog)
            {
                transform.gameObject.SetActive(flag);
            }
        }
    }

    public void error()
    {
        foreach (Transform transform in transform)
        {
            if (transform.gameObject != ErrorDialog)
            {
                transform.gameObject.SetActive(false);
            }
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        if (this.queryList.Count == 0)
        {
            setVisibility(false);
        }
    }
}
