using global::System;
using System.Collections;
using System.Collections.Generic;
using Assets.GoFindMap.Scripts;
using Assets.Modules.SimpleLogging;
using Assets.Plugins.LocationAwarenessPlugin;
using Assets.Scripts.Core;
using Assets.Scripts.UI;
using Assets.Scripts.UI.Management;
using CineastUnityInterface.Runtime.Vitrivr.UnityInterface.CineastApi.Model.Data;
using CineastUnityInterface.Runtime.Vitrivr.UnityInterface.CineastApi.Utils;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    private ObjectData activeMmo;

    //public PanelSwitcher panelSwitcher;

    public TransparentRawImage alphaController;

    public CamToggleButton camToggler;

    public Text choiceTitle;

    public Controller controller;

    public CompassHeadingDisplay headingDisplay;

    public Text infoText;
    private GeoLocation initialGeoLocation;

    private LocationInfo initLocation;

    public MapTouchController mapTouchController;

    public PanelManager panelManager;

    public GameObject prefab;

    public ImagePresenter presenter;

    public Button resetSessionButton;

    private ResultLocationDisplay resultDisplay;

    public Transform scrollContent;

    public TemporalSliderHandler temporalSlider;
    public Button viewChoiceHomeBtn;

    public Button viewDisplayHomeBtn;

    public Image watchAllIndication;

    public static UIManager Instance;
    
    public void SetInitialLocation(LocationInfo initInfo) {
        initLocation = initInfo;
    }

    public void SetInitialGeoLocation(GeoLocation loc) {
        initialGeoLocation = loc;
    }

    private void Awake() {
        logger = LogManager.GetInstance().GetLogger(GetType());
        panelManager = GetComponent<PanelManager>();
        if (panelManager == null) {
            Debug.LogError("No PanelManager attached to UIManager. This is B A D");
        }

        mapTouchController = GameObject.Find("MapPlane").GetComponent<MapTouchController>();
        Instance = this;
    }

    // Use this for initialization
    private void Start() {
        InitializePanels();
        alphaController.SetAlpha(0f);

        if (controller.IsSessionRunning()) {
            // do nothing
        } else {
            watchAllIndication.gameObject.SetActive(false);
            resetSessionButton.gameObject.SetActive(false);
        }
    }

    public void ShowWatchAll(bool show) {
        watchAllIndication.gameObject.SetActive(show);
    }

    private void InitializePanels() {
        GameObject homePanelObj = GameObject.Find("HomePanel");
        GameObject choicePanelObj = GameObject.Find("ChoicePanel");
        GameObject displayPanelObj = GameObject.Find("DisplayImagePanel");
        GameObject mapPanelObj = GameObject.Find("MapPanel");
        GameObject mapShowPanelObj = GameObject.Find("MapShowPanel");
        
        GameObject historicMapPanelObj = GameObject.Find("HistoricMapPanel");
        GameObject historicalPanelObj = GameObject.Find("HistoricalMapsPanel");
        GameObject timelinePanelObj = GameObject.Find("TimelinePanel");
        GameObject equidistantTimelinePanelObj = GameObject.Find("EquidistantTimelinePanel");
        GameObject merian3DPanelObj = GameObject.Find("Merian3DPanel");
        
        GameObject customizePanelObj = GameObject.Find("CustomizeSearchPanel");
        GameObject resultsPanelObj = GameObject.Find("ResultLocationDisplay");
        resultDisplay = resultsPanelObj.GetComponent<ResultLocationDisplay>();
        GameObject filterPanelObj = GameObject.Find("FilterPanel");
        GameObject waitingPanelObj = GameObject.Find("WaitingPanel");
        GameObject queryImagePanelObj = GameObject.Find("QueryImagePanel");
        GameObject calibrationPanelObj = GameObject.Find("CalibrationUI");
        GameObject arDisplayPnaelObj = GameObject.Find("ARDisplayPanel");
        GameObject arMerianPanaelObj = GameObject.Find("ARMerianPanel");
        GameObject settingsPanelObj = GameObject.Find("SettingsPanel");


        //Panels
        var homePanel = new PanelManager.Panel("home", homePanelObj);
        var choicePanel = new PanelManager.Panel("choice", choicePanelObj);
        var displayPanel = new PanelManager.Panel("display", displayPanelObj);
        var mapPanel = new PanelManager.Panel("map", mapPanelObj);
        var historicMapPanel = new PanelManager.Panel("historicMap", historicMapPanelObj);
        var historicalPanel = new PanelManager.Panel("historical", historicalPanelObj);
        var timelinePanel = new PanelManager.Panel("timeline", timelinePanelObj);
        var equidistantTimelinePanel = new PanelManager.Panel("equidistantTimeline", equidistantTimelinePanelObj);
        var merian3DPanel = new PanelManager.Panel("merian3D", merian3DPanelObj);
        var customizePanel = new PanelManager.Panel("customize", customizePanelObj);
        var resultPanel = new PanelManager.Panel("result", resultsPanelObj);
        var filterPanel = new PanelManager.Panel("filter", filterPanelObj);
        var waitingPanel = new PanelManager.Panel("waiting", waitingPanelObj);
        var mapShowPanel = new PanelManager.Panel("mapShow", mapShowPanelObj);
        var queryImagePanel = new PanelManager.Panel("queryImage", queryImagePanelObj);
        var calibrationPanel = new PanelManager.Panel("calibration", calibrationPanelObj);
        var arDisplayPanel = new PanelManager.Panel("ar-display", arDisplayPnaelObj);
        var arMerianPanel = new PanelManager.Panel("ar-merian", arMerianPanaelObj);
        var settingsPanel = new PanelManager.Panel("settings", settingsPanelObj);

        homePanel.next = choicePanel;
        choicePanel.previous = homePanel;
        choicePanel.next = displayPanel;
        displayPanel.previous = homePanel;
        mapPanel.previous = homePanel;
        mapPanel.visibilityChangedHandler = HandleMapVisibility;
        historicMapPanel.previous = historicalPanel;
        historicMapPanel.visibilityChangedHandler = HandleMapVisibility;
        historicalPanel.previous = homePanel;
        historicalPanel.visibilityChangedHandler = HandleMapVisibility;
        timelinePanel.visibilityChangedHandler = HandleMapVisibility;
        timelinePanel.previous = historicalPanel;
        equidistantTimelinePanel.visibilityChangedHandler = HandleMapVisibility;
        equidistantTimelinePanel.previous = historicalPanel;
        merian3DPanel.previous = historicalPanel;
        merian3DPanel.visibilityChangedHandler = HandleMapVisibility;
        merian3DPanel.visibilityChangedHandler = Handle3DView;
        merian3DPanel.previous = historicalPanel;
        filterPanel.previous = choicePanel;
        resultPanel.previous = mapPanel;
        mapShowPanel.previous = choicePanel;
        mapShowPanel.visibilityChangedHandler = HandleMapShowVisibility;
        arDisplayPanel.visibilityChangedHandler = HandleARVisibility;
        arMerianPanel.visibilityChangedHandler = HandleARVisibility;
        arMerianPanel.previous = historicalPanel;

        customizePanel.previous = homePanel;
        resultPanel.previous = mapPanel;

        settingsPanel.next = homePanel;
        settingsPanel.previous = homePanel;
        settingsPanel.visibilityChangedHandler = showing => { if(showing){settingsPanel.obj.GetComponent<SettingsDialog>().Init();}};

        panelManager.RegisterAll(new[] {
            homePanel, choicePanel, displayPanel, mapPanel, historicMapPanel, historicalPanel, timelinePanel, equidistantTimelinePanel, merian3DPanel, customizePanel, resultPanel, filterPanel, waitingPanel,
            mapShowPanel, queryImagePanel, calibrationPanel, arDisplayPanel, arMerianPanel, settingsPanel 
        }); 
        panelManager.SetInitial(homePanel);
        panelManager.ShowPanel("home");
    }

    private void HandleARVisibility(bool visible) {
        if (!visible) {
            // Disable ARCore's First Person Camera, Enable Camera for Map
            SwitchCams(false);
            // Reset ARMapper, so that calibration is needed again. //TODO This is only for debugging
            controller.ResetARSession();
        }
        
    }

    private void HandleMapShowVisibility(bool visible) {
        HandleMapVisibility(visible);
        if (mapTouchController != null) {
            mapTouchController.EnableDoubleTap(false);
        }
    }

    private void HandleMapVisibility(bool visible) {
        if (mapTouchController != null) {
            mapTouchController.EnableTouch(visible);
            mapTouchController.EnableDoubleTap(visible);
            mapTouchController.camera = GameObject.Find("MapCamera").GetComponent<Camera>();
            mapTouchController.EnableCamera(visible);
            controller.EnableARCam(!visible);
        }
    }

    private void Handle3DView(bool visible)
    {
        if (mapTouchController != null && GameObject.Find("Merian3DCamera") != null)
        {
            mapTouchController.camera = GameObject.Find("Merian3DCamera").GetComponent<Camera>();
        }
        HandleMapVisibility(false);
    }

    private Assets.Modules.SimpleLogging.Logger logger;

    public void Present(ObjectData mmo, bool loadInRange = true) {
        Debug.Log("Presenting " + mmo.Id);
        //panelSwitcher.SwitchToDisplay();

        panelManager.ShowPanel("display");

        presenter.LoadImage(TemporaryCompatUtils.GetImageUrl(mmo));
        logger.Debug("URL: " + TemporaryCompatUtils.GetImageUrl(mmo));


        alphaController.SetAlpha(0.5f);

        if (controller.GetHeading(mmo) != -1) {
            headingDisplay.targetHeading = controller.GetHeading(mmo);
        }

        infoText.text = string.Format("ID: {0}\nDate: {1}", mmo.Id, FormatDate(mmo));
        activeMmo = mmo;

        if (loadInRange) {
            Debug.Log("LOAD IN RANGE");
            List<ObjectData> inRangeList = controller.GetInRange(activeMmo);
            temporalSlider.Setup(inRangeList, activeMmo); // ArgumentNull in DatetimeParser, parameter name s
        }

        controller.StopLocationServices();
        // TODO Remove activeMMO and restart location service
    }

    public void StoreHeadingData() {
        controller.StoreHeading(activeMmo, CompassHeadingDisplay.RoundOff(Input.compass.magneticHeading));
    }

    public static string FormatDate(ObjectData mmo) {
        DateTime dt = DateTime.MaxValue;
        try
        {
            var datetime = MetadataUtils.GetDateTime(mmo.Metadata);
            dt = DateTime.Parse(datetime);
        } catch (ArgumentNullException ex) {
            // Silently ignoring
        } catch (FormatException e) {
            // silently ignoring
        }

        return dt != DateTime.MaxValue ? dt.ToString("dd. MMM yyyy") : "";
    }

    public void SetAndPopulateList(List<ObjectData> mmos) {
        UpdateChoiceTitle("" + mmos.Count);
        ClearList();
        StartCoroutine(PopulateScrollView(mmos));
    }

    public void ClearList() {
        while (scrollContent.childCount > 0) {
            GameObject go = scrollContent.GetChild(0).gameObject;
            go.transform.SetParent(null);
            Destroy(go);
        }
    }

    private IEnumerator PopulateScrollView(List<ObjectData> mmos) {
        ClearList();
        foreach (ObjectData mmo in mmos) {
            var exception = true;
            var tryies = 0;
            while (exception && (tryies <= 30)) {
                try {
                    Debug.Log("Trying to add to scroll list. Attempt: " + tryies);
                    AddObjectToScroll(mmo);
                    exception = false;
                } catch (NullReferenceException e) {
                    Debug.Log("Caught NRE");
                    exception = true;
                }

                if (exception) {
                    yield return new WaitForSeconds(0.5f);
                    Debug.Log("Had an exception, waited and trying again");
                    tryies++;
                }
            }

            yield return mmo;
        }
    }


    private void AddObjectToScroll(ObjectData mmo) {
        Debug.Log(":AddObjectToScroll " + (mmo != null ? JsonUtility.ToJson(mmo) : "null"));
        GameObject panel = Instantiate(prefab);
        Debug.Log("panel: " + (panel == null ? "null" : "found"));
        panel.transform.SetParent(scrollContent, false);

        var ctrl = panel.GetComponent<DisplayController>();
        Debug.Log("ctrl " + (ctrl == null ? "Null" : "received"));
        ctrl.SetTitle(string.Format("Result: {0}", mmo.Id));
        Debug.Log("initalGeoLoc: " + (initialGeoLocation == null ? "null" : "present"));
        var lat = MetadataUtils.GetLatitude(mmo.Metadata);
        var lon = MetadataUtils.GetLongitude(mmo.Metadata);
        double dist = Utilities.HaversineDistance(lat, lon, initialGeoLocation.latitude,
            initialGeoLocation.longitude);
        string footerText = string.Format("Distance: {0}m\nDate: {1}", Round(dist), FormatDate(mmo));
        ctrl.SetFooter(footerText);
        ctrl.LoadImageFromWeb(TemporaryCompatUtils.GetThumbnailUrl(mmo));
        ctrl.Mmo = mmo;
        ctrl.UiManager = this;
        ctrl.Controller = controller;
    }

    public string GetRoundedDist(GeoLocation mmoLoc, GeoLocation targetLoc) {
        double dist = Utilities.HaversineDistance(mmoLoc.latitude, mmoLoc.longitude, targetLoc.latitude,
            targetLoc.longitude);
        return Round(dist);
    }


    public static string Round(double d, int digits = 2) {
        return "" + Math.Round(d, digits, MidpointRounding.AwayFromZero);
    }

    public void ShowResultPanel(ObjectData mmo) {
        panelManager.ShowPanel("result");
        resultDisplay.Present(mmo);
    }

    private void UpdateChoiceTitle(string count) {
        choiceTitle.text = choiceTitle.text.Replace("#", "" + count);
    }

    public static T GetScriptComponentByName<T>(string name) {
        var containerObject = GameObject.Find(name);
        if (containerObject != null) {
            if (containerObject.GetComponent<T>() != null) {
                return containerObject.GetComponent<T>();
            }
        }

        return default(T);
    }

    public void SwitchCams(bool arActive) {
        controller.EnableARCam(arActive);
        EnableMapCam(!arActive);
    }
    
    public void EnableMapCam(bool b) {
        mapTouchController.EnableCamera(b);
        mapTouchController.gameObject.SetActive(b);
    }
    
    public static void ShowAndroidToastMessage(string message) {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                    message, 0);
                toastObject.Call("show");
            }));
        }
    }
}