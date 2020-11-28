using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.HoloLens.Scripts.Properties
{
    public class POICoordinatesObject
    {
        private bool mylocation;
        private float heading;
        private double latitude;
        private double longitude;
        private string name;
        private GameObject MapPin;
        private Coordinates coordinates;
        private string url;
        private Texture texture;
        private float upperbound;
        private float lowerbound;

        public POICoordinatesObject(double latitude, double longitude, string name, GameObject gameObject, string url)
        {
            this.coordinates = new Coordinates(latitude, longitude);
            this.name = name;
            this.MapPin = gameObject;
            this.url = url;
        }

        public POICoordinatesObject(double latitude, double longitude, GameObject gameObject,float heading)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.heading = heading;
            this.coordinates = new Coordinates(latitude,longitude);
            this.MapPin = gameObject;
        }

        public POICoordinatesObject(double latitude, double longitude, float heading)
        {
            this.latitude = latitude;
            this.longitude = longitude;
            this.heading = heading;
            this.coordinates = new Coordinates(latitude,longitude);
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
            return this.MapPin;
        }

        public float getHeading()
        {
            return this.heading;
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

        public void setMapPin(GameObject gameObject)
        {
            MapPin = gameObject;
        }

        public void setMyLocation(bool flag)
        {
            this.mylocation = true;
        }

        public bool getMyLocation()
        {
            return mylocation;
        }
        public override string ToString()
        {
            return name;
        }
    }
}