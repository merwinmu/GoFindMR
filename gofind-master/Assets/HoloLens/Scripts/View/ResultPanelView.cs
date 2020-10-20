using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    void RenderPanels();

}
public class ResultPanelView : MonoBehaviour , IResultPanelView
{
    public event EventHandler<ResultBackEventArgs> OnBackButton;

    public Texture2D texture;
    private GameObject backButtonObject;
    private Interactable backInteractable;
    

    private MeshRenderer panelRenderer;
    private Texture panelTexture;

    private void Start()
    {
        panelRenderer = transform.GetChild(1).GetChild(1).GetComponent<MeshRenderer>();
        
        //Debug.Log(panelRenderer.material.mainTexture);

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

    public static async Task<Texture2D> GetRemoteTexture ( string url )
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
    public async void RenderPanels()
    {
        texture = await GetRemoteTexture( "https://windowsunited.de/wp-content/uploads/sites/3/2019/05/HoloLens2.jpg" );
        panelRenderer.material.mainTexture = texture;
    }

    public void Visibility(bool flag)
    {
        transform.gameObject.SetActive(flag);

    }
}
