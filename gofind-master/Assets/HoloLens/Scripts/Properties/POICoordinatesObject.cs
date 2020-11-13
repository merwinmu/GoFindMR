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
        private int upperbound;
        private int lowerbound;

        public POICoordinatesObject(double latitude, double longitude, string name, GameObject gameObject, string url)
        {
            this.coordinates = new Coordinates(latitude, longitude);
            this.name = name;
            this.gameObject = gameObject;
            this.url = url;
        }

        public POICoordinatesObject(double latitude, double longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }

        public POICoordinatesObject(string upperbound, string lowerbound)
        {
            
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

        public void setName(string name)
        {
            this.name = name;
        }


        public override string ToString()
        {
            return name;
        }
    }
}