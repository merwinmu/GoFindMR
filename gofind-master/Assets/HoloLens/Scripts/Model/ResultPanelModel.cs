using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/*
* Various EventArgs has been created so that if changes in the Model has been made, a callback can be
* invoked to the controller which then sends it to the view
*/
public class UpdatePicturesEventArgs : EventArgs
{
    private List<PictureData> pictureDatas;
    public UpdatePicturesEventArgs(List<PictureData> pictureDatas)
    {
        this.pictureDatas = pictureDatas;
    }

    public List<PictureData> getPictureData()
    {
        return pictureDatas;
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

public class PictureData
{
    private string url;
    private int objectID;
    private Texture texture;
    private double lat;
    private double lon;

    public PictureData(int id, string url)
    {
        this.objectID = id;
        this.url = url;
    }
    
    public PictureData(int id, string url,double lat, double lon)
    {
        this.objectID = id;
        this.url = url;
        this.lat = lat;
        this.lon = lon;
    }

    public void setData(Texture texture)
    {
        this.texture = texture;
    }

    public void setLatLon(double lat, double lon)
    {
        this.lat = lat;
        this.lon = lon;
    }

    public double getLat()
    {
        return this.lat;
    }

    public double getLon()
    {
        return this.lon;
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

    /*
                * Eventhandler is used to to send events
                 * This method is used for changing the visibility of the menu
                 */
    void ChangeResultVisibility(bool flag);
    void renderPicture();

    void SetCurrentPicture(PicturePointerData pointerData);
}

public class ResultPanelModel : IResultPanelModel
{
    public event EventHandler<UpdatePicturesEventArgs> OnUpdatePictures = (sender, e) => { };
    public event EventHandler<ResultVisibilityEventArgs> OnResultVisibility = (sender, e) => { };
    private bool showResult;

    private List<PictureData> pictureDataList;

    private PicturePointerData CurrentPicture;


    public void renderPicture()
    {
        pictureDataList = new List<PictureData>();
        
        pictureDataList.Add(new PictureData(0,"https://cdn.pixabay.com/photo/2016/10/18/21/22/california-1751455__340.jpg",21.42039,24.28500));
        pictureDataList.Add(new PictureData(1,"https://cdn.pixabay.com/photo/2018/01/14/23/12/nature-3082832__340.jpg",-28.35779,66.10798));
        pictureDataList.Add(new PictureData(2,"https://cdn.pixabay.com/photo/2015/05/15/14/21/architecture-768432__340.jpg",-23.69705,-142.92103));
        pictureDataList.Add(new PictureData(3,"https://cdn.pixabay.com/photo/2016/10/20/18/35/sunrise-1756274__340.jpg",52.08303,-28.59140));
        pictureDataList.Add(new PictureData(4,"https://cdn.pixabay.com/photo/2013/10/02/23/03/dog-190056__340.jpg",34.3313,17.20796));
        pictureDataList.Add(new PictureData(5,"https://cdn.pixabay.com/photo/2015/04/19/08/32/rose-729509__340.jpg",23.64371,-39.94751));
        pictureDataList.Add(new PictureData(6,"https://cdn.pixabay.com/photo/2013/10/02/23/03/dawn-190055__340.jpg",69.48288,-117.56180));
        pictureDataList.Add(new PictureData(7,"https://cdn.pixabay.com/photo/2014/09/07/16/53/hands-437968__340.jpg",64.95757,70.36632));
        pictureDataList.Add(new PictureData(8,"https://cdn.pixabay.com/photo/2017/03/13/10/25/hummingbird-2139279__340.jpg", -18.70780,22.63778));
        pictureDataList.Add(new PictureData(9,"https://cdn.pixabay.com/photo/2014/09/14/18/04/dandelion-445228__340.jpg",39.53518,36.22254));
        pictureDataList.Add(new PictureData(10,"https://cdn.pixabay.com/photo/2015/12/01/20/28/fall-1072821__340.jpg",18.02662,39.71201));
        pictureDataList.Add(new PictureData(11,"https://cdn.pixabay.com/photo/2018/09/23/18/30/drop-3698073__340.jpg",-40.42041,-170.03909));
        pictureDataList.Add(new PictureData(12,"https://cdn.pixabay.com/photo/2017/12/10/15/16/white-horse-3010129__340.jpg", -51.71954,73.71774));

        var eventArgs = new UpdatePicturesEventArgs(pictureDataList);

        // Dispatch the 'Result changed' event
        OnUpdatePictures(this, eventArgs);
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