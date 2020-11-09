namespace Assets.HoloLens.Scripts.Properties
{
    public class Coordinates
    {
        private double lat;
        private double lon;
        public Coordinates(double lat, double lon)
        {
            this.lat = lat;
            this.lon = lon;
        }

        public double getLat()
        {
            return this.lat;
        }
        public double getLon()
        {
            return this.lon;
        }

        public override string ToString()
        {
            return this.lat.ToString() + " " + this.lon.ToString();
        }
    }
}