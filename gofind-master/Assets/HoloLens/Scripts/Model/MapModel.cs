using System;
using System.Collections.Generic;

namespace Assets.HoloLens.Scripts.Model
{

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

    public interface IMapModel
    {
        event EventHandler<GetPictureLocationEventArgs> OnAddPictureLocationChanged;
        void AddPictureLocation(double latitude, double longitude);

    }
    
    public class MapModel : IMapModel
    {
        public event EventHandler<GetPictureLocationEventArgs> OnAddPictureLocationChanged= (sender, e) => { };
        
        private List<Location> Pin_locations = new List<Location>();

        
        public void AddPictureLocation(double latitude, double longitude)
        {
            Pin_locations.Add(new Location(latitude,longitude));
            // Dispatch the photochanged event
        }
    }
}