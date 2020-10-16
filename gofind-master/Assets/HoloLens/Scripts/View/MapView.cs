using System;
using UnityEngine;

namespace Assets.HoloLens.Scripts.View
{
    public class MapInputEventArgs : EventArgs
    {
        
    }
    
    public interface IMapView
    {
        event EventHandler<MapInputEventArgs> OnMapInput;
        
        //Use Class function using this interface functions
        void setGameObjectVisibility(bool flag);

    }
    public class MapView : MonoBehaviour, IMapView
    {
        public event EventHandler<MapInputEventArgs> OnMapInput  = (sender, e) => { };

        private void Start()
        {
            throw new NotImplementedException();
        }

        public void setGameObjectVisibility(bool flag)
        {
            throw new NotImplementedException();
        }
    }
}