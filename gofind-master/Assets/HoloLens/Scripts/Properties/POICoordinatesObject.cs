using UnityEngine;
using UnityEngine.Networking;

namespace Assets.HoloLens.Scripts.Properties
{
    public class POICoordinatesObject
    {
        private double latitude;
        private double longitude;
        private string name;
        private GameObject gameObject;

        public POICoordinatesObject(double latitude, double longitude,string name, GameObject gameObject)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.name = name;
            this.gameObject = gameObject;
        }

        public override string ToString()
        {
            return name;
        }
    }
    
}