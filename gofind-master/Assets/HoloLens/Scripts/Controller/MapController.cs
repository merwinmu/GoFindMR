using Assets.HoloLens.Scripts.Model;
using UnityEngine;

namespace Assets.HoloLens.Scripts.Controller
{
    public interface IMapController
    {
    }
    
    public class MapController : MonoBehaviour, IMapController
    {
        
        // Keep references to the model and view
        private static IMapModel model;
        
    }
}