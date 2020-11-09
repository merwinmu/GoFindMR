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
        private Coordinates coordinates;
        private string url;
        private Texture texture;

        public POICoordinatesObject(double latitude, double longitude,string name, GameObject gameObject, string url)
        {
            this.coordinates = new Coordinates(latitude,longitude);
            this.name = name;
            this.gameObject = gameObject;
            this.url = url;
        }

        public Coordinates getCoordinates()
        {
            return this.coordinates;
        }

        public string getURL()
        {
            return url;
        }

        public void setTexture(Texture texture)
        {
            this.texture = texture;
        }
        

        public override string ToString()
        {
            return name;
        }
    }
    
}