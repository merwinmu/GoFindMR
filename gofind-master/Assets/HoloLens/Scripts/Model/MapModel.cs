using System;
using System.Collections.Generic;

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
        void ChangeVisibility(bool flag);
        void GenerateMapPins();
        void AddPictureLocation(double latitude, double longitude);
    }
    
    public class Location
    {
        private double latitude;
        private double longitude;
        
        public Location(double latitude, double longitude)
        {
            this.latitude = latitude;
            this.longitude = longitude;
        }
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

        private List<Location> Pin_locations = new List<Location>();

        /*
          * Eventhandler is used to to send events
          * This method is used for changing the visibility of the menu
          */ 

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

        public void AddPictureLocation(double latitude, double longitude)
        {
            Pin_locations.Add(new Location(latitude,longitude));
            // Dispatch the photochanged event
        }
        
        
    }
}