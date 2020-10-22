using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using UnityEngine.Networking;
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

    void setTextures(List<PictureData> pictureDatasList);

}
public class ResultPanelView : MonoBehaviour , IResultPanelView
{
    public event EventHandler<ResultBackEventArgs> OnBackButton;

    public Texture2D texture;
    private GameObject backButtonObject;
    private Interactable backInteractable;

    public GameObject resultObject;
    private List<PictureData> textureDatas;

    private Texture panelTexture;

    private GameObject scrollObeObjectCollectionGameObject;
    private ScrollingObjectCollection scrollingObjectCollection;

    private void Start()
    {
        scrollObeObjectCollectionGameObject = transform.GetChild(1).GetChild(0).GetChild(1).gameObject;
        scrollingObjectCollection = scrollObeObjectCollectionGameObject.GetComponent<ScrollingObjectCollection>();

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
    
    
    //Output Actions

    public static async Task<Texture> GetRemoteTexture ( string url )
    {
        using( UnityWebRequest www = UnityWebRequestTexture.GetTexture( url ) )
        {
            //begin requenst:
            var asyncOp = www.SendWebRequest();

            //await until it's done: 
            while( asyncOp.isDone==false )
            {
                await Task.Delay( 1000/30 );//30 hertz
            }

            //read results:
            if( www.isNetworkError || www.isHttpError )
            {
                //log error:
#if DEBUG
                Debug.Log( $"{ www.error }, URL:{ www.url }" );
#endif

                //nothing to return on error:
                return null;
            }
            else
            {
                //return valid results:
                return DownloadHandlerTexture.GetContent( www );
            }
        }
    }
    public async void setTextures(List<PictureData> pictureDatasList)
    {
        textureDatas = pictureDatasList;
        int size = textureDatas.Count;
        string url;

        foreach (var VARIABLE in textureDatas)
        {
            url = VARIABLE.getURL();
            VARIABLE.setData(await GetRemoteTexture(url));
        }
        createResultObjects(size);
    }
    
    public void createResultObjects(int v_size)
    {
        List<GameObject> ObjectList = new List<GameObject>();

        int horizontal_size = 3;
        //int vertical_size = t_size/horizontal_size;
        
        float init_xpos = -0.4f;
        
        float xpos = -1f;
        float ypos = 0.8f;
        float zpos = 0.2f;
        
        float xpos_offset = 0.4f;
        float ypos_offset = -0.3f;

        // int count = 0;
        //
        //     for (float y = 0; y < vertical_size; y++)
        //     {
        //         for (float x = 0; x < horizontal_size; x++)
        //         {
        //             ObjectList.Add(Instantiate(resultObject, new Vector3(xpos, ypos, 0), Quaternion.identity));
        //             xpos = xpos + xpos_offset;
        //             count++;
        //         }
        //         ypos = ypos + ypos_offset;
        //         xpos = init_xpos;
        //     }


            int counter = 0;
            ObjectList.Add(Instantiate(resultObject, new Vector3(xpos, ypos, zpos), Quaternion.identity));
            //tempObject.gameObject.transform.parent = ScrollableResult.transform.GetChild(0).GetChild(0).transform;

            counter++;
            while (counter < v_size)
            {
                if (counter % horizontal_size != 0)
                {
                    xpos = xpos + xpos_offset; // Moving to right with spaces
                }

                if (counter % horizontal_size == 0) // once horizonal limit reached go to next line and reset xpos
                {
                    ypos = ypos + ypos_offset;
                    xpos = init_xpos;
                }
                ObjectList.Add(Instantiate(resultObject, new Vector3(xpos, ypos, zpos), Quaternion.identity));
                counter++;
            }
            
            int listcount = 0;
            foreach (var VARIABLE in textureDatas)
            {
                ObjectList[listcount].GetComponent<Renderer>().material.mainTexture = VARIABLE.getData();
                listcount++;
            }

            foreach (var VARIABLE in ObjectList)
            {
                VARIABLE.transform.localScale = new Vector3(-0.4f,-0.2f,0.01f);
                VARIABLE.gameObject.transform.parent =
                    transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).transform;
                var touchable = VARIABLE.AddComponent<NearInteractionTouchableVolume>();
                touchable.EventsToReceive = TouchableEventType.Touch;
                
                var touchhandler = VARIABLE.AddComponent<TouchHandler>();
                touchhandler.OnTouchCompleted.AddListener((e) => Debug.Log("Touched Photo"));
            }
            
            scrollingObjectCollection.UpdateCollection();
            Debug.Log("achso");

    }

    public void Visibility(bool flag)
    {
        transform.gameObject.SetActive(flag);
    }
    
}