namespace Assets.HoloLens.Scripts.Properties
{
    public class Coordinates
    {
        private double lat;
        private double lon;
        private float head;
        public Coordinates(double lat, double lon)
        {
            this.lat = lat;
            this.lon = lon;
        }

        public Coordinates(double lat, double lon, float head)
        {
            this.lat = lat;
            this.lon = lon;
            this.head = head;
        }

        public double getLat()
        {
            return this.lat;
        }
        public double getLon()
        {
            return this.lon;
        }
        public float gethead()
        {
            return this.head;
        }

        public override string ToString()
        {
            return this.lat.ToString() + " " + this.lon.ToString();
        }
        
        public override bool Equals(object obj)
        {
            var item = obj as Coordinates;

            if (item == null)
            {
                return false;
            }

            return this.lat == item.getLat() && this.lon == item.getLon() && this.head == item.gethead();
        }

        public override int GetHashCode()
        {
            return this.lat.GetHashCode();
        }
    }
}