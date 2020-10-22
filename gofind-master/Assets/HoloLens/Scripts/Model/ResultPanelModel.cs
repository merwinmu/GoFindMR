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

    public PictureData(int id, string url)
    {
        this.objectID = id;
        this.url = url;
    }

    public void setData(Texture texture)
    {
        this.texture = texture;
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
}

public class ResultPanelModel : IResultPanelModel
{
    public event EventHandler<UpdatePicturesEventArgs> OnUpdatePictures = (sender, e) => { };
    public event EventHandler<ResultVisibilityEventArgs> OnResultVisibility = (sender, e) => { };
    private bool showResult;

    private List<PictureData> pictureDataList;


    public void renderPicture()
    {
        pictureDataList = new List<PictureData>();
        
        pictureDataList.Add(new PictureData(1,"https://cdn.pixabay.com/photo/2016/10/18/21/22/california-1751455__340.jpg"));
        pictureDataList.Add(new PictureData(2,"https://cdn.pixabay.com/photo/2018/01/14/23/12/nature-3082832__340.jpg"));
        pictureDataList.Add(new PictureData(3,"https://cdn.pixabay.com/photo/2015/05/15/14/21/architecture-768432__340.jpg"));
        pictureDataList.Add(new PictureData(4,"https://cdn.pixabay.com/photo/2016/10/20/18/35/sunrise-1756274__340.jpg"));
        pictureDataList.Add(new PictureData(5,"https://cdn.pixabay.com/photo/2013/10/02/23/03/dog-190056__340.jpg"));
        pictureDataList.Add(new PictureData(6,"https://cdn.pixabay.com/photo/2015/04/19/08/32/rose-729509__340.jpg"));
        pictureDataList.Add(new PictureData(7,"https://cdn.pixabay.com/photo/2013/10/02/23/03/dawn-190055__340.jpg"));
        pictureDataList.Add(new PictureData(8,"https://cdn.pixabay.com/photo/2014/09/07/16/53/hands-437968__340.jpg"));
        pictureDataList.Add(new PictureData(9,"https://cdn.pixabay.com/photo/2017/03/13/10/25/hummingbird-2139279__340.jpg"));
        pictureDataList.Add(new PictureData(10,"https://cdn.pixabay.com/photo/2014/09/14/18/04/dandelion-445228__340.jpg"));
        pictureDataList.Add(new PictureData(11,"https://cdn.pixabay.com/photo/2015/12/01/20/28/fall-1072821__340.jpg"));
        pictureDataList.Add(new PictureData(12,"https://cdn.pixabay.com/photo/2018/09/23/18/30/drop-3698073__340.jpg"));
        pictureDataList.Add(new PictureData(13,"https://cdn.pixabay.com/photo/2017/12/10/15/16/white-horse-3010129__340.jpg"));

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
}