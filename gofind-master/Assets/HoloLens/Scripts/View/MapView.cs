using System;
using System.Globalization;
using Assets.HoloLens.Scripts.Properties;
using Microsoft.Geospatial;
using Microsoft.Maps.Unity;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using TMPro;
using UnityEngine;

/*
 * Views are primarly used for Input and Output. It is primarly a Monobehaviour class with the associate functions 
 * Input actions such as OnClick invoke an event to the controller which then executes a function to model
 * Output actions are in example rendering gameobjects etc.
 */



/*
 * Various EventArgs has been created so that if an Input occurs , a callback can be
 * invoked to the controller which then sends it to the model
 */

namespace Assets.HoloLens.Scripts.View
{
    public class MapInputEventArgs : EventArgs
    {
    }
    
    public interface IMapView
    {
        event EventHandler<MapInputEventArgs> OnMapInput;
        event EventHandler<POIEventArgs> OnPOI;
        
        //Use Class function using this interface functions
        void setGameObjectVisibility(bool flag);
        void setMapPosition(Vector3 pos);
        void setLocationPins();
        void RenderGenerateMapPins();
        void ZoomMap(float data);
        void setCurrentPositionPin(double latitude, double longitude, float heading);
        void removeLocationPins(MapPin gameObject);
        void ZoomIntoPoint(double lat, double lon);
        void EnableRadial();
        void DisableRadial();

    }
    public class MapView : MonoBehaviour, IMapView
    {
        public event EventHandler<MapInputEventArgs> OnMapInput  = (sender, e) => { };
        public event EventHandler<POIEventArgs> OnPOI  = (sender, e) => { };
        
        

        
        [SerializeField]
        private MapPinLayer _mapPinLayer;
        
        [SerializeField]
        private MapPin _mapPinPrefab;
        
        [SerializeField]
        private TextAsset _mapPinLocationsCsv;
        
        [SerializeField]
        private MapPin currentMapPin;

        private GameObject Map;
        private MapRenderer renderer;
        private MapPinLayer mapPinLayer;
        private Camera camera;
        private MapInteractionController mapInteractionController;
        

        private void Start()
        {
            camera = Camera.main;
            Map = transform.gameObject;
            Map.SetActive(false);
            
            CurrentPositionInit();
            renderer = GetComponent<MapRenderer>();
            mapPinLayer = GetComponent<MapPinLayer>();
            mapInteractionController = GetComponent<MapInteractionController>();
            mapInteractionController.OnDoubleTap.AddListener(((data ) => GenerateLatLonObject(data)));
        }

        private void GenerateLatLonObject(LatLonAlt data)
        {
            var mapPin = Instantiate(_mapPinPrefab);
            mapPin.Location = data.LatLon;
            _mapPinLayer.MapPins.Add(mapPin);
            POICoordinatesObject poiCoordinatesObject = new POICoordinatesObject(data.LatitudeInDegrees,data.LongitudeInDegrees,0);
            poiCoordinatesObject.setMapPin(mapPin);
            poiCoordinatesObject.setName(data.LatLon.LatitudeInDegrees.ToString()+" "+data.LongitudeInDegrees);
            var EventArgs = new POIEventArgs(poiCoordinatesObject);
            OnPOI(this, EventArgs);
        }

        public void CurrentPositionInit()
        {
            currentMapPin = Instantiate(currentMapPin);
            currentMapPin.transform.parent = transform;
        }
        public void setCurrentPositionPin(double latitude, double longitude, float heading)
        {
            currentMapPin.Location =  new LatLon(latitude,longitude);
        }

        public void ZoomIntoPoint(double lat, double lon)
        {
            var mapScene = new MapSceneOfLocationAndZoomLevel(new LatLon(lat,lon),  19f);
            renderer.SetMapScene(mapScene);
        }

        public void EnableRadial()
        {
            Map.GetComponent<RadialView>().enabled = true;
            Map.GetComponent<SolverHandler>().enabled = true;
        }

        public void DisableRadial()
        {
            Map.GetComponent<RadialView>().enabled = false;
            Map.GetComponent<SolverHandler>().enabled = false;
        }

        public void setGameObjectVisibility(bool flag)
        {
            transform.gameObject.SetActive(flag);
        }

        public void setMapPosition(Vector3 pos)
        {
            transform.position = pos;
        }

        public void setLocationPins()
        {
            throw new NotImplementedException();
        }

        public void removeLocationPins(MapPin mapPin)
        {
            mapPinLayer.MapPins.Remove(mapPin);
            // foreach (Transform transform in transform.GetChild(1))
            // {
            //     if (transform.gameObject == gameObject)
            //     {
            //         Debug.Log("Removed MapPin "+transform.gameObject);
            //         break;
            //     }
            // }
        }


        public void ZoomMap(float zoomdata)
        {
            renderer = GetComponent<MapRenderer>();
            renderer.ZoomLevel = zoomdata * 20f;
        }

        //Output action triggered by the Controller
        public void RenderGenerateMapPins()
        {
            var lines = _mapPinLocationsCsv.text.Split(new [] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var csvLine in lines)
            {
                var csvEntries = csvLine.Split(',');

                var mapPin = Instantiate(_mapPinPrefab);
                mapPin.Location =
                    new LatLon(
                        double.Parse(csvEntries[0], NumberStyles.Number, CultureInfo.InvariantCulture),
                        double.Parse(csvEntries[1], NumberStyles.Number, CultureInfo.InvariantCulture));
                _mapPinLayer.MapPins.Add(mapPin);

                mapPin.GetComponentInChildren<TextMeshPro>().text = csvEntries[2].ToLower() == "null" ? "" : csvEntries[2];
            }
        }

        public void getRayCastCoordinate()
        {
            
        }

        private double temps;
        private bool click;
        private bool longClickDone;
        
        
        //Debug
        public void getMouseCoordinate()
        {
            var ray = camera.ScreenPointToRay(Input.mousePosition);

            if ( Input.GetMouseButtonDown (0) )
            {
                temps = Time.time ;
                click = true ;
                longClickDone = false ;
            }
            
 
            if ( Input.GetMouseButtonUp(0) )
            {
                click = false ;
                if ( (Time.time - temps) < 0.2 )
                {
                    if (renderer.Raycast(ray, out MapRendererRaycastHit hitInfo))
                    {
                        var hitpoint = hitInfo.Point;
                        
                        Debug.Log(renderer.TransformWorldPointToLatLon(hitpoint));

                        LatLonAlt latLonAlt = new LatLonAlt(renderer.TransformWorldPointToLatLon(hitpoint).LatitudeInDegrees,renderer.TransformWorldPointToLatLon(hitpoint).LongitudeInDegrees,0);

                        GenerateLatLonObject(latLonAlt);
                    }
                }
            }
        }

        private void Update()
        {
            getMouseCoordinate();
        }
    }
}