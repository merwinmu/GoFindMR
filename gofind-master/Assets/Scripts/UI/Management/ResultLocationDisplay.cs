using Assets.GoFindMap.Scripts;
using Assets.Scripts.Core;
using Assets.Scripts.UI;
using CineastUnityInterface.Runtime.Vitrivr.UnityInterface.CineastApi.Model.Data;
using CineastUnityInterface.Runtime.Vitrivr.UnityInterface.CineastApi.Utils;
using UnityEngine;
using UnityEngine.UI;

public class ResultLocationDisplay : MonoBehaviour {

    public Text infoLabel;
    public Text titleLabel;
    public ImagePresenter imagePresenter;
    public ToggleButton toggleButton;

    public Controller controller;
    public UIManager uiManager;

    private ObjectData active;

    void Start() {
        toggleButton.SetToggleHandler(HandleToggling);
    }

    public void Present(ObjectData mmo) {
        DisplayInfo(mmo);
        imagePresenter.LoadImage(TemporaryCompatUtils.GetThumbnailUrl(mmo));
        active = mmo;
        titleLabel.text = "Result " + mmo.Id; // TODO re-introduce rank
    }

    private void DisplayInfo(ObjectData mmo) {
        var lat = MetadataUtils.GetLatitude(mmo.Metadata);
        var lon = MetadataUtils.GetLongitude(mmo.Metadata);
        string dist = uiManager.GetRoundedDist(new GeoLocation(lat, lon),
            controller.GetLastKnownLocation());
        infoLabel.text = string.Format("Dist: {0}m\nDate: {1}", dist, UIManager.FormatDate(mmo));
    }

    private void HandleToggling(bool toggled) {
        if (toggled) {
            controller.AddToActiveList(active);
        } else {
            controller.RemoveFromActiveList(active);
        }
    }
}