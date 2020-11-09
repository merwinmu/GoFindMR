// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using Assets.HoloLens.Scripts.Model;
using Assets.HoloLens.Scripts.Properties;
using Microsoft.Maps.Unity;
using TMPro;
using UnityEngine;

/// <summary>
/// Zooms in towards the <see cref="MapPin"/> when clicked.
/// </summary>
[RequireComponent(typeof(MapPin))]
public class ZoomToMapPin : MonoBehaviour
{
    private MapRenderer _map;
    private MapPin _mapPin;
    
    public static event EventHandler<POIEventArgs> OnMapObject  = (sender, e) => { };


    void Start()
    {
        _map = GameObject.Find("Map").GetComponent<MapRenderer>();
        Debug.Assert(_map != null);
        _mapPin = GetComponent<MapPin>();
        Debug.Assert(_mapPin != null);
    }

    public void Zoom()
    {
        //var mapScene = new MapSceneOfLocationAndZoomLevel(_mapPin.Location, _map.ZoomLevel + 1.01f);
        //_map.SetMapScene(mapScene);
        string name = _mapPin.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshPro>().text;
        POICoordinatesObject poiCoordinatesObject = new POICoordinatesObject(_mapPin.Location.LatitudeInDegrees,_mapPin.Location.LongitudeInDegrees,name,transform.gameObject,"https://cdn.pixabay.com/photo/2018/09/23/18/30/drop-3698073_960_720.jpg");
        Debug.Log("zoom: " + poiCoordinatesObject.getCoordinates());
        var eventArgs = new POIEventArgs(poiCoordinatesObject);
        
        // Dispatch the 'position changed' event
        OnMapObject(this, eventArgs);
    }
}

public class POIEventArgs : EventArgs
{
    private GameObject gameObject;
    private POICoordinatesObject poiCoordinatesObject;
    public POIEventArgs(POICoordinatesObject poiCoordinatesObject)
    {
        this.poiCoordinatesObject = poiCoordinatesObject;
    }

    public POICoordinatesObject GETPoiCoordinatesObject()
    {
        return this.poiCoordinatesObject;
    }
}
