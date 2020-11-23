using CineastUnityInterface.Runtime.Vitrivr.UnityInterface.CineastApi.Model.Data;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class ResultMarker : MonoBehaviour
    {
        public ObjectData result;

        public UIManager uiManager;

        public void HandleClicked()
        {
            Debug.Log("Clicked and showing" + result.Id);
            uiManager.ShowResultPanel(result);
        }
    }
}