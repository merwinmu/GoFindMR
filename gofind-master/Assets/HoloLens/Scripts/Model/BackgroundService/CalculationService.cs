using System;
using Assets.HoloLens.Scripts.Properties;
using UnityEngine;

namespace Assets.HoloLens.Scripts.Model.BackgroundService
{
    public class CalculationService: ThreadedJob
    {
        
        public event EventHandler<SpawnObjectOnCoordinateEventArgs> OnResult = (sender, e) => { };
        public double CurrentLatitude;
        public double CurrentLongitude;
        public POICoordinatesObject poi;
        public SpawnObjectOnCoordinateEventArgs eventArgs;
        private double distance = 100;

        public void calculateRadius(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            const double r = 6371; // meters
            var sdlat = Math.Sin((latitude2 - latitude1) / 2);
            var sdlon = Math.Sin((longitude2 - longitude1) / 2);
            var q = sdlat * sdlat + Math.Cos(latitude1) * Math.Cos(latitude2) * sdlon * sdlon;
            var d = 2 * r * Math.Asin(Math.Sqrt(q));
            distance = d;
        }
        
        
        protected override void ThreadFunction()
        {
            // Do your threaded task. DON'T use the Unity API here
            
            double lat = poi.getCoordinates().getLat();
            double lon = poi.getCoordinates().getLon();
             // 100km radius
            double inboundthreshold = 2;
            double outboundthreshold = 4;

            double currentlat = CurrentLatitude;
            double currentlon = CurrentLongitude;
            
            Debug.Log(poi.GetHashCode()+" Thread started");
            while (distance > inboundthreshold)
            {
                calculateRadius(lat,lon,currentlat,currentlon);
                Debug.Log("Thread ID: "+poi.GetHashCode()+"  Current Position: "+CurrentLatitude+" "+CurrentLongitude + " Destinaton: "+lat+" "+lon+" Distance to Dest "+distance.ToString());
            }
            eventArgs = new SpawnObjectOnCoordinateEventArgs(poi);
            

            // while (outboundthreshold > distance)
            // {
            //     distance = calculateRadius(lat,lon,CurrentLatitude,CurrentLongitude);
            //     
            // }
            // Debug.Log("Remove Gameobject");
            // OnRemoveObject(this, eventArgs);
        }
        
        protected override void OnFinished()
        {
            // This is executed by the Unity main thread when the job is finished
            OnResult(this, eventArgs);
            Debug.Log("Reached the Destination");
        }
        
    }
}