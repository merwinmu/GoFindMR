using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEditor;
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

public class SelectResultPictureDataArgs : EventArgs
{
    private PicturePointerData pointerData;
    public SelectResultPictureDataArgs(PicturePointerData pointerData)
    {
        this.pointerData = pointerData;
    }

    public PicturePointerData get()
    {
        return this.pointerData;
    }
}

public interface IResultPanelView
{
    event EventHandler<ResultBackEventArgs> OnBackButton;
    event EventHandler<SelectResultPictureDataArgs> OnSelectPicture;
    void Visibility(bool flag);

    void setTextures(List<PictureData> pictureDatasList);

}
public class ResultPanelView : MonoBehaviour , IResultPanelView 
{
    public event EventHandler<ResultBackEventArgs> OnBackButton = (sender, e) => { };
    public event EventHandler<SelectResultPictureDataArgs> OnSelectPicture = (sender, e) => { };
    public Texture2D texture;
    private GameObject backButtonObject;
    private Interactable backInteractable;

    public GameObject resultObject;
    private List<PictureData> textureDatas;

    private Texture panelTexture;

    private GameObject scrollObeObjectCollectionGameObject;
    private ScrollingObjectCollection scrollingObjectCollection;

    private GameObject ShowPictureObject;

    private GameObject result0;
    private GameObject result1;
    private GameObject result2;
    private void Start()
    {
        scrollObeObjectCollectionGameObject = transform.GetChild(1).GetChild(0).GetChild(1).gameObject;
        scrollingObjectCollection = scrollObeObjectCollectionGameObject.GetComponent<ScrollingObjectCollection>();

       // result0 = transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetChild(0).gameObject;
       // result1 = transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetChild(0).gameObject;
       // result2 = transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetChild(0).gameObject;

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

        GameObject Container = scrollingObjectCollection.transform.GetChild(0).gameObject;

        
        
       // List<GameObject> ObjectList = new List<GameObject>();
        List<PicturePointerData> picturePointerDatasList = new List<PicturePointerData>();

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
            // ObjectList.Add(Instantiate(resultObject, new Vector3(xpos, ypos, zpos), Quaternion.identity));
            picturePointerDatasList.Add(new PicturePointerData(Instantiate(resultObject, new Vector3(xpos, ypos, zpos), Quaternion.identity),counter));
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
                picturePointerDatasList.Add(new PicturePointerData(Instantiate(resultObject, new Vector3(xpos, ypos, zpos), Quaternion.identity),counter));
               // ObjectList.Add(Instantiate(resultObject, new Vector3(xpos, ypos, zpos), Quaternion.identity));
                counter++;
            }
            
            int listcount = 0;
            foreach (var VARIABLE in textureDatas)
            {
               // ObjectList[listcount].GetComponent<Renderer>().material.mainTexture = VARIABLE.getData();
                picturePointerDatasList[listcount].getGameObject().GetComponent<Renderer>().material.mainTexture = VARIABLE.getData();
                listcount++;
            }

            foreach (var VARIABLE in picturePointerDatasList)
            {
                VARIABLE.getGameObject().transform.localScale = new Vector3(-0.4f,-0.2f,0.01f);
                VARIABLE.getGameObject().gameObject.transform.parent =
                    transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).transform;
                
                VARIABLE.getGameObject().AddComponent<PictureAttribute>();
                VARIABLE.getGameObject().GetComponent<PictureAttribute>().ID = VARIABLE.getID();
                VARIABLE.getGameObject().GetComponent<PictureAttribute>().PointerData = VARIABLE;
                
                var touchable = VARIABLE.getGameObject().AddComponent<NearInteractionTouchableVolume>();
                touchable.EventsToReceive = TouchableEventType.Pointer;
                var touchhandler = VARIABLE.getGameObject().AddComponent<PointerHandler>();
                touchhandler.OnPointerUp.AddListener((e) => SearchIDProvider(touchhandler.gameObject,picturePointerDatasList));
            }
            
            scrollingObjectCollection.UpdateCollection();
            Debug.Log("achso");

    }

    public void HandlePictureCLick()
    {
       
    }

    public void Visibility(bool flag)
    {
        transform.gameObject.SetActive(flag);
    }
    
    
    private GameObject ShowObject;
    public void SearchIDProvider(GameObject gameObject, List<PicturePointerData> list)
    {
        PicturePointerData result = list.Find(x => x.getGameObject() == gameObject);
        Debug.Log(result.getID() +" ID ");
        
        var eventArgs = new SelectResultPictureDataArgs(result);
        OnSelectPicture(this, eventArgs);

        ShowObject = Instantiate(result.getGameObject());
        ShowObject.GetComponent<PointerHandler>().enabled = false; // Disabling it otherwise other Listener will get Events
        ShowObject.transform.position = result.getGameObject().transform.position; // This position used for a smooth transition from collection to view

        ShowPictureObject = (GameObject)Resources.Load("Prefab/ShowResult",typeof(GameObject)) ;
        ShowPictureObject = Instantiate(ShowPictureObject);
        ShowPictureObject.transform.parent = transform.parent;

        ShowPictureObject.transform.position = ShowObject.transform.position; // Get Original position from colloection for nice smooth transition

        ShowObject.transform.localScale = new Vector3(0.5f,-0.3f,0.0001f); // Setting size

        ShowObject.transform.parent = ShowPictureObject.transform;

        //ShowPictureObject.transform.GetChild(0).transform.position = result.getGameObject().transform.position;
        //ShowPictureObject.transform.GetChild(0).transform.localScale = new Vector3(0.5f,-0.3f,0.0001f);
        
        ShowPictureObject.GetComponent<SolverHandler>().enabled = true;
        ShowPictureObject.GetComponent<RadialView>().enabled = false;

        RadialView radialView = ShowPictureObject.GetComponent<RadialView>();
        setImageProperties(radialView);
        
        setCollectionVisibility(false);        
        initPictureObject();
        PictureMenuSuperImoseInit();
    }

    private GameObject close_button;
    private GameObject anchor_button;
    //private GameObject superimose_button;

    private Interactable closeInteractable;
    private Interactable anchorInteractable;
    //private Interactable superimoseInteractable;
    
    public void initPictureObject()
    {
         close_button = ShowPictureObject.transform.GetChild(0).GetChild(1).GetChild(0).gameObject;
         anchor_button = ShowPictureObject.transform.GetChild(0).GetChild(1).GetChild(1).gameObject;
         //superimose_button = ShowPictureObject.transform.GetChild(0).GetChild(1).GetChild(2).gameObject;
        
         closeInteractable = close_button.GetComponent<Interactable>();
         anchorInteractable = anchor_button.GetComponent<Interactable>();
         //superimoseInteractable = superimose_button.GetComponent<Interactable>();
        
        closeInteractable.OnClick.AddListener(PictureMenuCloseButton);
        anchorInteractable.OnClick.AddListener(PictureMenuAnchor);
        //superimoseInteractable.OnClick.AddListener(PictureMenuSuperImose);

    }
    public void PictureMenuCloseButton()
    {
        Debug.Log("Clicked Close Button");
        Destroy(ShowPictureObject);
        setCollectionVisibility(true);
    }

    public void PictureMenuAnchor()
    {
        if (ShowPictureObject.GetComponent<RadialView>().enabled)
        {
            ShowPictureObject.GetComponent<RadialView>().enabled = false;
        }
        else
        {
            ShowPictureObject.GetComponent<RadialView>().enabled = true;
        }
    }

    public void PictureMenuSuperImoseInit()
    {
        GameObject slider = ShowPictureObject.transform.GetChild(1).gameObject;
        Material material = initTransperancySettings();
        
        slider.GetComponent<PinchSlider>().OnValueUpdated.AddListener((e) => superimose(e.NewValue));
    }

    public Material initTransperancySettings()
    {
        GameObject Pic = ShowPictureObject.transform.GetChild(2).gameObject;
        Material m = Pic.GetComponent<Renderer>().sharedMaterial;
        m.SetFloat("_Mode", 2.0f);
        return m;
    }

    public void superimose(float value)
    {
        ShowPictureObject.transform.GetChild(2).gameObject.GetComponent<Renderer>().sharedMaterials[0].color = new Color(1,1,1,value);
        MaterialChanged(ShowPictureObject.transform.GetChild(2).gameObject.GetComponent<Renderer>().sharedMaterials[0],WorkflowMode.Metallic);
    }
    
    private enum WorkflowMode
    {
        Specular,
        Metallic,
        Dielectric
    }

    public enum BlendMode
    {
        Opaque,
        Cutout,
        Fade,   // Old school alpha-blending mode, fresnel does not affect amount of transparency
        Transparent // Physically plausible transparency mode, implemented as alpha pre-multiply
    }

    public enum SmoothnessMapChannel
    {
        SpecularMetallicAlpha,
        AlbedoAlpha,
    }
    
     public static void SetupMaterialWithBlendMode(Material material, BlendMode blendMode)
        {
            switch (blendMode)
            {
                case BlendMode.Opaque:
                    material.SetOverrideTag("RenderType", "");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = -1;
                    break;
                case BlendMode.Cutout:
                    material.SetOverrideTag("RenderType", "TransparentCutout");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.EnableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                    break;
                case BlendMode.Fade:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    break;
                case BlendMode.Transparent:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    break;
            }
        }
     
     static SmoothnessMapChannel GetSmoothnessMapChannel(Material material)
     {
         int ch = (int)material.GetFloat("_SmoothnessTextureChannel");
         if (ch == (int)SmoothnessMapChannel.AlbedoAlpha)
             return SmoothnessMapChannel.AlbedoAlpha;
         else
             return SmoothnessMapChannel.SpecularMetallicAlpha;
     }
     
     static void SetMaterialKeywords(Material material, WorkflowMode workflowMode)
     {
         // Note: keywords must be based on Material value not on MaterialProperty due to multi-edit & material animation
         // (MaterialProperty value might come from renderer material property block)
         SetKeyword(material, "_NORMALMAP", material.GetTexture("_BumpMap") || material.GetTexture("_DetailNormalMap"));
         if (workflowMode == WorkflowMode.Specular)
             SetKeyword(material, "_SPECGLOSSMAP", material.GetTexture("_SpecGlossMap"));
         else if (workflowMode == WorkflowMode.Metallic)
             SetKeyword(material, "_METALLICGLOSSMAP", material.GetTexture("_MetallicGlossMap"));
         SetKeyword(material, "_PARALLAXMAP", material.GetTexture("_ParallaxMap"));
         SetKeyword(material, "_DETAIL_MULX2", material.GetTexture("_DetailAlbedoMap") || material.GetTexture("_DetailNormalMap"));

         // A material's GI flag internally keeps track of whether emission is enabled at all, it's enabled but has no effect
         // or is enabled and may be modified at runtime. This state depends on the values of the current flag and emissive color.
         // The fixup routine makes sure that the material is in the correct state if/when changes are made to the mode or color.
         //MaterialEditor.FixupEmissiveFlag(material);
         bool shouldEmissionBeEnabled = (material.globalIlluminationFlags & MaterialGlobalIlluminationFlags.EmissiveIsBlack) == 0;
         SetKeyword(material, "_EMISSION", shouldEmissionBeEnabled);

         if (material.HasProperty("_SmoothnessTextureChannel"))
         {
             SetKeyword(material, "_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A", GetSmoothnessMapChannel(material) == SmoothnessMapChannel.AlbedoAlpha);
         }
     }
     
     
     static void MaterialChanged(Material material, WorkflowMode workflowMode)
     {
         SetupMaterialWithBlendMode(material, (BlendMode)material.GetFloat("_Mode"));

         SetMaterialKeywords(material, workflowMode);
     }

     static void SetKeyword(Material m, string keyword, bool state)
     {
         if (state)
             m.EnableKeyword(keyword);
         else
             m.DisableKeyword(keyword);
     }


    public void setCollectionVisibility(bool flag)
    {
        transform.GetChild(0).gameObject.SetActive(flag);
        transform.GetChild(1).gameObject.SetActive(flag);
    }

    public void setImageProperties(RadialView radialView)
    {
        radialView.MoveLerpTime = 0.5f;
        radialView.RotateLerpTime = 0.5f;
        radialView.MinDistance = 0.5f;
        radialView.MaxDistance = 0.6f;
        radialView.MaxViewDegrees = 0.0f;
    }

    public void showPicture(PicturePointerData picturePointerData)
    {
        GameObject result = picturePointerData.getGameObject();
        int id = picturePointerData.getID();
        Vector3 orginalPosition = result.transform.position;

        GameObject EndResult;
        
        GameObject ShowPictureCollection = transform.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetChild(0).gameObject;

        foreach (Transform VARIABLE in ShowPictureCollection.transform)
        {
            if (id == VARIABLE.gameObject.GetComponent<PictureAttribute>().ID)
            {
                 EndResult = VARIABLE.gameObject;
                 EndResult.AddComponent<RadialView>();
                 break;
            }
        }
    }
}

public class PicturePointerData
{
    private GameObject pictureGameObject;
    private int id;

    public PicturePointerData(int id)
    {
        this.id = id;
    }

    public PicturePointerData(GameObject gameObject)
    {
        this.pictureGameObject = gameObject;
    }
    public PicturePointerData(GameObject pictureGameObject, int id)
    {
        this.id = id;
        this.pictureGameObject = pictureGameObject;
    }

    public GameObject getGameObject()
    {
        return pictureGameObject;
    }

    public int getID()
    {
        return id;
    }
}