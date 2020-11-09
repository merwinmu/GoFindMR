using System;
using System.Collections.Generic;
using System.Threading;
using Assets.HoloLens.Scripts.Properties;
using UnityEngine;

namespace Assets.HoloLens.Scripts.Model
{
    /*
 * Various EventArgs has been created so that if changes in the Model has been made, a callback can be
 * invoked to the controller which then sends it to the view
 */
    public class MapVisibilityEventArgs : EventArgs
    {
        public bool flag { get; private set; }

        public MapVisibilityEventArgs(bool flag)
        {
            this.flag = flag;
        }
    }
    
    public class SpawnObjectOnCoordinateEventArgs : EventArgs
    {
        private bool empty;
        
        private POICoordinatesObject poiCoordinatesObject;
        public SpawnObjectOnCoordinateEventArgs(POICoordinatesObject poiCoordinatesObject)
        {
            this.poiCoordinatesObject = poiCoordinatesObject;
        }

        public bool getBool()
        {
            return this.empty;
        }

        public POICoordinatesObject get()
        {
            return poiCoordinatesObject;
        }
    }

    public class RemoveSpawnObjectEventArgs : EventArgs
    {
        
    }


    public class GetPictureLocationEventArgs : EventArgs
         {
             public double latitude { get; private set; }
             public double longitude { get; private set; }
     
             public GetPictureLocationEventArgs(double latitude, double longitude)
             {
                 this.latitude = latitude;
                 this.longitude = longitude;
             }
         }

    public class GeneratePinEventArgs : EventArgs
    {
    }
    public interface IMapModel
    {
        event EventHandler<GetPictureLocationEventArgs> OnAddPictureLocationChanged;
        event EventHandler<MapVisibilityEventArgs> MapVisibility;
        event EventHandler<GeneratePinEventArgs> GeneratePinMap;
        event EventHandler<SpawnObjectOnCoordinateEventArgs> OnSpawnCoordinate;

        event EventHandler<SpawnObjectOnCoordinateEventArgs> OnRemoveObject;
        void ChangeVisibility(bool flag);
        void GenerateMapPins();
        void AddPOILocation(POICoordinatesObject location);

        void RemovePOI(int id);

        void setCurrentLocation(double latitude, double longitude, float heading);

        void SpatialExploration();
    }
    
/*
 * Models are used to store information of different UI Menus.
 * Model informations can changed by the controller.
 * An Interface has been also implemented so that the controller han can access only the interface functions
 */
    public class MapModel : IMapModel 
    {
        private bool showHideMenu;

        public event EventHandler<GetPictureLocationEventArgs> OnAddPictureLocationChanged = (sender, e) => { };
        public event EventHandler<MapVisibilityEventArgs> MapVisibility = (sender, e) => { };
        public event EventHandler<GeneratePinEventArgs> GeneratePinMap = (sender, e) => { };
        public event EventHandler<SpawnObjectOnCoordinateEventArgs> OnSpawnCoordinate = (sender, e) => { };
        public event EventHandler<SpawnObjectOnCoordinateEventArgs> OnRemoveObject = (sender, e) => { };


        Dictionary<int, POICoordinatesObject> poiLocations = new Dictionary<int, POICoordinatesObject>();

        private double CurrentLongitude;
        private double CurrentLatitude;
        private double currentHeading;
        List<SpawnObjectOnCoordinateEventArgs> objectspawnList = new List<SpawnObjectOnCoordinateEventArgs>();        

        /*
          * Eventhandler is used to to send events
          * This method is used for changing the visibility of the menu
          */

        public void setCurrentLocation(double latitude, double longitude, float heading)
        {
            this.CurrentLatitude = latitude;
            this.CurrentLongitude = longitude;
            this.currentHeading = heading;
        }
        public void ChangeVisibility(bool flag)
        {
            showHideMenu = flag;
            var eventArgs = new MapVisibilityEventArgs(showHideMenu);
            
            // Dispatch the 'position changed' event
            MapVisibility(this, eventArgs);
        }

        public void GenerateMapPins()
        {
            var eventArgs = new GeneratePinEventArgs();

            GeneratePinMap(this, eventArgs);
        }

        private int POIGameObjectID = 0;
        public void AddPOILocation(POICoordinatesObject poiCoordinatesObject)
        {
            poiLocations.Add(POIGameObjectID,poiCoordinatesObject);
            POIGameObjectID++;
        }

        public void RemovePOI(int id)
        {
            poiLocations.Remove(id);
            Debug.Log(poiLocations);
        }

        private double distance;
        public double calculateRadius(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            const double r = 6371; // meters

            var sdlat = Math.Sin((latitude2 - latitude1) / 2);
            var sdlon = Math.Sin((longitude2 - longitude1) / 2);
            var q = sdlat * sdlat + Math.Cos(latitude1) * Math.Cos(latitude2) * sdlon * sdlon;
            var d = 2 * r * Math.Asin(Math.Sqrt(q));

            return d;
        }

        public void calulateSpatialDistance(object threadInput)
        {
            var poi = (POICoordinatesObject) threadInput;
            double lat = poi.getCoordinates().getLat();
            double lon = poi.getCoordinates().getLon();
            double distance = 100; // 100km radius
            double inboundthreshold = 2;
            double outboundthreshold = 4;

            double currentlat = CurrentLatitude;
            double currentlon = CurrentLongitude;
            
            Debug.Log(threadInput.GetHashCode()+" Thread started");
            while (distance > inboundthreshold)
            {
                distance = calculateRadius(lat,lon,currentlat,currentlon);
                Debug.Log("Thread ID: "+poi.GetHashCode()+"  Current Position: "+CurrentLatitude+" "+CurrentLongitude + " Destinaton: "+lat+" "+lon+" Distance to Dest "+distance.ToString());
            }
            var eventArgs = new SpawnObjectOnCoordinateEventArgs(poi);
            OnSpawnCoordinate(this, eventArgs);
            Debug.Log("Reached the Destination, Obejct size "+ objectspawnList.Count);

            // while (outboundthreshold > distance)
            // {
            //     distance = calculateRadius(lat,lon,CurrentLatitude,CurrentLongitude);
            //     
            // }
            // Debug.Log("Remove Gameobject");
            // OnRemoveObject(this, eventArgs);
        }

        public void SpatialExploration()
        {
            Debug.Log("Size of "+poiLocations.Count);
            
            foreach (var VARIABLE in poiLocations)
            {
                var thread = new Thread(calulateSpatialDistance);
                thread.IsBackground = true;
                var threadInput = VARIABLE.Value;
                Debug.Log(VARIABLE.Value.getCoordinates());
                thread.Start(threadInput);
                Debug.Log("Starting Distance measuring Threads");
            }
        }

        public void stopJourney()
        {
            
        }
        
    }
}