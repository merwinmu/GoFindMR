using System;
using System.Collections.Generic;
using Assets.HoloLens.Scripts.Properties;
using Assets.HoloLens.Scripts.View;
using Assets.Scripts.Core;
using CineastUnityInterface.Runtime.Vitrivr.UnityInterface.CineastApi.Model.Data;
using CineastUnityInterface.Runtime.Vitrivr.UnityInterface.CineastApi.Utils;
using UnityEngine;
using Object = System.Object;

/*
* Various EventArgs has been created so that if changes in the Model has been made, a callback can be
* invoked to the controller which then sends it to the view
*/
public class UpdatePicturesEventArgs : EventArgs
{
    private List<PictureData> pictureDatas;
    private List<int> keys;
   

    public UpdatePicturesEventArgs(List<PictureData> pictureDatas)
    {
        this.pictureDatas = pictureDatas;
    }
    
    public UpdatePicturesEventArgs(List<int> keys)
    {
        this.keys = keys;
    }

    public List<PictureData> getPictureData()
    {
        return pictureDatas;
    }

    public List<int> getKeys()
    {
        return this.keys;
    }
}

public class ResultVisibilityEventArgs : EventArgs
{
    public bool flag { get; private set; }

    public ResultVisibilityEventArgs(bool flag)
    {
        this.flag = flag;
    }
}

public class ResetEventArgs : EventArgs
{
    
}

public class PictureData
{
    private string url;
    private int objectID;
    private Texture texture;
    private double lat;
    private double lon;
    private float heading;
    private string quasitime;
    public PictureData(int id, string url)
    {
        this.objectID = id;
        this.url = url;
    }
    
    public PictureData(int id, string url,double lat, double lon, float heading)
    {
        this.objectID = id;
        this.url = url;
        this.lat = lat;
        this.lon = lon;
        this.heading = heading;
    }
    
    public PictureData(int id, string url,double lat, double lon, float heading,string quasitime)
    {
        this.objectID = id;
        this.url = url;
        this.lat = lat;
        this.lon = lon;
        this.heading = heading;
        this.quasitime = quasitime;
    }
    public void setData(Texture texture)
    {
        this.texture = texture;
    }

    public void setLatLon(double lat, double lon, float heading)
    {
        this.lat = lat;
        this.lon = lon;
        this.heading = heading;
    }

    public string getQuasiTime()
    {
        return this.quasitime;
    }

    public double getLat()
    {
        return this.lat;
    }

    public double getLon()
    {
        return this.lon;
    }

    public float gethea()
    {
        return this.heading;
    }

    public void setID(int id)
    {
        this.objectID = objectID;
    }

    public Texture getData()
    {
        return texture;
    }

    public int getID()
    {
        return this.objectID;
    }

    public string getURL()
    {
        return this.url;
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
    event EventHandler<ResetEventArgs> OnReset;
    event EventHandler<BackEventArgs> OnEDialog;


    /*
                * Eventhandler is used to to send events
                 * This method is used for changing the visibility of the menu
                 */
    void ChangeResultVisibility(bool flag);
    void renderDebugPicture();

    void populateAndRender(List<ObjectData> list, DateTime upperbound, DateTime lowerbound, bool activate, List<POICoordinatesObject> poilist);

    void reset();
  
    void SetCurrentPicture(PicturePointerData pointerData);

    void setDistance(float value);

    void setLatLon(double lat, double lon);

    void populateFetchedList(DateTime upperbound, DateTime lowerbound, bool activate_temp,
        List<POICoordinatesObject> poilist);
}

public class ResultPanelModel : IResultPanelModel
{
    public event EventHandler<UpdatePicturesEventArgs> OnUpdatePictures = (sender, e) => { };
    public event EventHandler<ResultVisibilityEventArgs> OnResultVisibility = (sender, e) => { };
    public event EventHandler<ResetEventArgs> OnReset = (sender, e) => { };
    public event EventHandler<BackEventArgs> OnEDialog = (sender, e) => { };


    private bool showResult;

    private List<PictureData> pictureDataList;

    private PicturePointerData CurrentPicture;

    private float distance_value;
    private double currentlat;
    private double currentlon;


    public void renderDebugPicture()
    {
        pictureDataList = new List<PictureData>();
        
        pictureDataList.Add(new PictureData(0,"https://cdn.pixabay.com/photo/2016/10/18/21/22/california-1751455__340.jpg",21.42039,24.28500,0));
        pictureDataList.Add(new PictureData(1,"https://cdn.pixabay.com/photo/2018/01/14/23/12/nature-3082832__340.jpg",-28.35779,66.10798,0));
        pictureDataList.Add(new PictureData(2,"https://cdn.pixabay.com/photo/2015/05/15/14/21/architecture-768432__340.jpg",-23.69705,-142.92103,0));
        pictureDataList.Add(new PictureData(3,"https://cdn.pixabay.com/photo/2016/10/20/18/35/sunrise-1756274__340.jpg",52.08303,-28.59140,0));
        pictureDataList.Add(new PictureData(4,"https://cdn.pixabay.com/photo/2013/10/02/23/03/dog-190056__340.jpg",34.3313,17.20796,0));
        pictureDataList.Add(new PictureData(5,"https://cdn.pixabay.com/photo/2015/04/19/08/32/rose-729509__340.jpg",23.64371,-39.94751,0));
        pictureDataList.Add(new PictureData(6,"https://cdn.pixabay.com/photo/2013/10/02/23/03/dawn-190055__340.jpg",69.48288,-117.56180,0));
        pictureDataList.Add(new PictureData(7,"https://cdn.pixabay.com/photo/2014/09/07/16/53/hands-437968__340.jpg",64.95757,70.36632,0));
        pictureDataList.Add(new PictureData(8,"https://cdn.pixabay.com/photo/2017/03/13/10/25/hummingbird-2139279__340.jpg", -18.70780,22.63778,0));
        pictureDataList.Add(new PictureData(9,"https://cdn.pixabay.com/photo/2014/09/14/18/04/dandelion-445228__340.jpg",39.53518,36.22254,0));
        pictureDataList.Add(new PictureData(10,"https://cdn.pixabay.com/photo/2015/12/01/20/28/fall-1072821__340.jpg",18.02662,39.71201,0));
        pictureDataList.Add(new PictureData(11,"https://cdn.pixabay.com/photo/2018/09/23/18/30/drop-3698073__340.jpg",-40.42041,-170.03909,0));
        pictureDataList.Add(new PictureData(12,"https://cdn.pixabay.com/photo/2017/12/10/15/16/white-horse-3010129__340.jpg", -51.71954,73.71774,0));

        var eventArgs = new UpdatePicturesEventArgs(pictureDataList);

        // Dispatch the 'Result changed' event
        OnUpdatePictures(this, eventArgs);
    }

    public void setDistance(float value)
    {
        distance_value = value;
        Debug.Log("Distance Radius set to "+value);
    }

    public void setLatLon(double lat, double lon)
    {
        this.currentlat = lat;
        this.currentlon = lon;
    }

    public void populateAndRender(List<ObjectData> list, DateTime upperbound, DateTime lowerbound, bool activate_temp, List<POICoordinatesObject> poilist)
    {
        this.upperbound = upperbound;
        this.lowerbound = lowerbound;
        this.activate_temp = activate_temp;
        parseToPictureData(list,poilist);
        Debug.Log("COunt"+list.Count);

        
        if (pictureDataList.Count != 0)
        {
            Debug.Log("Parsed the mmo Object to PictureData");
            var eventArgs = new UpdatePicturesEventArgs(pictureDataList);
            // Dispatch the 'Result changed' event

            OnUpdatePictures(this, eventArgs);
        }
        else
        {
            Debug.Log("No Results found");
            var eventArgs = new BackEventArgs();
            OnEDialog(this,eventArgs);
        }
    }

    public void populateFetchedList(DateTime upperbound, DateTime lowerbound, bool activate_temp,
        List<POICoordinatesObject> poilist)
    {
        this.upperbound = upperbound;
        this.lowerbound = lowerbound;
        this.activate_temp = activate_temp;
        List<int> finalkeys = parseActivePictureData(poilist);
        
        if (finalkeys.Count != 0)
        {
            Debug.Log("Parsed the keys ");
            var eventArgs = new UpdatePicturesEventArgs(finalkeys);
            // Dispatch the 'Result changed' event

            OnUpdatePictures(this, eventArgs);
        }
        else
        {
            Debug.Log("No Results found");
            var eventArgs = new BackEventArgs();
            OnEDialog(this,eventArgs);
        }
    }

    public List<int> parseActivePictureData(List<POICoordinatesObject> poilist)
    {
        List<int> keys = new List<int>();
        List<int> finalkeys = new List<int>();
        if (poilist.Count != 0)
        {
            keys = FilterFetchedToDistance(FetchedList,poilist);
        }

        else
        {
            keys = backupkeys;
        }

        foreach (var fetcheddata in FetchedList)
        {
            String time = fetcheddata.getQuasiTime();
            
            if (keys.Contains(fetcheddata.getID()))
            {
                
                
                if (time != "" && activate_temp)
                {
                    DateTime dateTime = DateTime.Parse(fetcheddata.getQuasiTime());
                    
                    if (lowerbound < dateTime && dateTime < upperbound)
                    {
                        finalkeys.Add(fetcheddata.getID());
                    }
                }
                else
                {
                    finalkeys.Add(fetcheddata.getID());
                }
            }
        }
        Debug.Log("No finalkeyslist "+finalkeys.Count);
        return finalkeys;
    }

    private DateTime upperbound;
    private DateTime lowerbound;
    private bool activate_temp;
    private List<PictureData>FetchedList = new List<PictureData>(10000);
    public void parseToPictureData(List<ObjectData> list, List<POICoordinatesObject> poilist)
    {
        List<ObjectData> newList = list;
        
        if (poilist.Count!=0)
        {
            newList = FilterToDistance(list, poilist);
        }
        
        pictureDataList = new List<PictureData>();

        int id = 0;
        string url;
        string time;
        double lat = 0;
        double lon = 0;
        float hea;

        foreach (var VARIABLE in newList)
        {
            url = TemporaryCompatUtils.GetImageUrl(VARIABLE);
            time = MetadataUtils.GetDateTime(VARIABLE.Metadata);
            lat = MetadataUtils.GetLatitude(VARIABLE.Metadata);
            lon = MetadataUtils.GetLongitude(VARIABLE.Metadata);
            hea = Convert.ToSingle(MetadataUtils.GetBearing(VARIABLE.Metadata));
            hea = 0;
            

                Debug.Log("THESE ARE TIME: "+time);

            if (url == "http://10.34.58.145/objects/Ans_05459-007-AL-FL.jpg")
            {
                hea = 160;
            }
            if (url == "http://10.34.58.145/objects/barfuesserplatz-parkplatz-lohnhof.png")
            {
                hea = 90;
            }
            if (url == "http://10.34.58.145/objects/barfuesserplatz-kohlenberg.png")
            {
                hea = 90;
            }
            if (url == "http://10.34.58.145/objects/Fel_008898-RE.jpg.png")
            {
                hea = 90;
            }
            if (url == "http://10.34.58.145/objects/Com_M06-0079-0004.jpg")
            {
                hea = 270;
            }
            if (url == "http://10.34.58.145/objects/barfuesserplatz-lohnhof-schnee.png")
            {
                hea = 90;
            }
            if (url == "http://10.34.58.145/objects/Ans_05395-02-001.jpg")
            {
                hea = 270;
            }
            if (url == "http://10.34.58.145/objects/Ans_05395-01-016.jpg")
            {
                hea = 0;
            }
            if (url == "http://10.34.58.145/objects/Barfuesserirche_ganz.jpg")
            {
                hea = 180;
            }
            if (url == "http://10.34.58.145/objects/Dia_287-01946.jpg")
            {
                hea = 300;
            }
            if (url == "http://10.34.58.145/objects/Ans_05395-01-003.jpg")
            {
                hea = 0;
            }
            if (url == "http://10.34.58.145/objects/Basel_2012-08_Mattes_1_(206).JPG")
            {
                hea = 90;
            }
            if (url == "http://10.34.58.145/objects/Ans_02112.jpg")
            {
                hea = 240;
            }
            if (url == "http://10.34.58.145/objects/Com_M06-0079-0002.jpg")
            {
                hea = 290;
            }
            if (url == "http://10.34.58.145/objects/Ans_05395-01-001.jpg")
            {
                hea = 200;
            }
            
            //Temporary fix
            
            if (time != "" && activate_temp)
            {
                DateTime dateTime = DateTime.Parse(MetadataUtils.GetDateTime(VARIABLE.Metadata));
                
                if (lowerbound < dateTime && dateTime < upperbound)
                {
                    Debug.Log("TIME DEBUG: "+MetadataUtils.GetDateTime(VARIABLE.Metadata) + " CURRENT: LOWER "+ lowerbound +" UP "+ upperbound );
                    pictureDataList.Add(new PictureData(id,url,lat,lon,hea,time));
                    FetchedList.Add(new PictureData(id,url,lat,lon,hea,time));
                }

                else
                {
                    continue;
                }
            }

            else
            {
                pictureDataList.Add(new PictureData(id,url,lat,lon,hea,time));
                FetchedList.Add(new PictureData(id, url, lat, lon, hea,time));
            }

            id++;
            backupkeys.Add(id);
            Debug.Log("No fetchedlid "+FetchedList.Count);

        }
    }

    List<int>backupkeys = new List<int>(1000);
    public List<int> FilterFetchedToDistance(List<PictureData> fetchedList,List<POICoordinatesObject> poilist)
    {
        List<int> fileredKeys = new List<int>();
        double lat;
        double lon;
        double dis;

        foreach (var pictureData in fetchedList)
        {
            lat = pictureData.getLat();
            lon = pictureData.getLon();

            foreach (var poi in poilist)
            {
                dis = MapMenuView.calculateRadius(lat,lon,poi.getCoordinates().getLat(), poi.getCoordinates().getLon()) * 1000;

                if ((dis) <= distance_value)
                {
                    fileredKeys.Add(pictureData.getID());
                    Debug.Log("ADDED    Distance to Picture: "+ dis + " " + lat + " " + lon + " MY LOC: "+ currentlat + " " + currentlon + " "+ distance_value);
                }
            }
        }

        return fileredKeys;

    }

    public List<ObjectData> FilterToDistance(List<ObjectData> list, List<POICoordinatesObject> poilist)
    {
        Debug.Log("Size of POILIST:"+ poilist.Count);
        
        List<ObjectData> filterdList = new List<ObjectData>();
        double lat;
        double lon;
        double dis;

        foreach (var VARIABLE in list)
        {
            lat = MetadataUtils.GetLatitude(VARIABLE.Metadata);
            lon = MetadataUtils.GetLongitude(VARIABLE.Metadata);
            
            foreach (var poi in poilist)
            {
                dis = MapMenuView.calculateRadius(lat,lon,poi.getCoordinates().getLat(), poi.getCoordinates().getLon()) * 1000;
                if ((dis) <= distance_value)
                {
                    filterdList.Add(VARIABLE);
                    Debug.Log("ADDED    Distance to Picture: "+ dis + " " + lat + " " + lon + " MY LOC: "+ currentlat + " " + currentlon + " "+ distance_value);
                }
            }
        }
        return filterdList;
    }
    

    public void reset()
    {
        activate_temp = false;
        //pictureDataList.Clear();
        OnReset(this,new ResetEventArgs());
    }

    public void ChangeResultVisibility(bool flag)
    {
        showResult = flag;
        var eventArgs = new ResultVisibilityEventArgs(showResult);

        // Dispatch the 'Result changed' event
        OnResultVisibility(this, eventArgs);
    }

    public void SetCurrentPicture(PicturePointerData pointerData)
    {
        this.CurrentPicture = pointerData;
    }
}