using System.Runtime.CompilerServices;
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
        private float upperbound;
        private float lowerbound;

        public POICoordinatesObject(double latitude, double longitude, string name, GameObject gameObject, string url)
        {
            this.coordinates = new Coordinates(latitude, longitude);
            this.name = name;
            this.gameObject = gameObject;
            this.url = url;
        }

        public POICoordinatesObject(double latitude, double longitude, GameObject gameObject)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.coordinates = new Coordinates(latitude,longitude);
            this.gameObject = gameObject;
        }

        public POICoordinatesObject(string upperbound, string lowerbound)
        {
            
        }

        public POICoordinatesObject(float ub, float lb)
        {
            upperbound = ub;
            lowerbound = lb;
        }

        public Coordinates getCoordinates()
        {
            return this.coordinates;
        }

        public GameObject GETGameObject()
        {
            return this.gameObject;
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