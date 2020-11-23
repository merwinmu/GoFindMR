using CineastUnityInterface.Runtime.Vitrivr.UnityInterface.CineastApi.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class SettingsDialog : MonoBehaviour
    {
        private InputField cineastInput;
        private InputField imagesInput;

        private void Awake()
        {
            cineastInput = GameObject.Find("CineastInputField").GetComponent<InputField>();
            imagesInput = GameObject.Find("ImagesInputField").GetComponent<InputField>();

            var okBtn = transform.Find("BottomContainer/OkButton").gameObject.GetComponent<Button>();
            var cancelBtn = transform.Find("BottomContainer/CancelButton").gameObject.GetComponent<Button>();

            okBtn.onClick.RemoveAllListeners();
            cancelBtn.onClick.RemoveAllListeners();
            okBtn.onClick.AddListener(Store);
            cancelBtn.onClick.AddListener(Cancel);
        }

        public void Init()
        {
            cineastInput.text = CineastConfigManager.Instance.Config.cineastHost;
            imagesInput.text = CineastConfigManager.Instance.Config.mediaHost;
        }

        public void Store()
        {
            // TODO Regex for real ip check

            CineastConfigManager.Instance.Config.cineastHost = cineastInput.text;
            CineastConfigManager.Instance.Config.mediaHost = imagesInput.text;
            CineastConfigManager.Instance.StoreConfig();
            Debug.Log("Stored config");

            UIManager.Instance.panelManager.ShowNext();
        }

        public void Cancel()
        {
            Init();
            UIManager.Instance.panelManager.ShowPrevious();
        }
    }
}