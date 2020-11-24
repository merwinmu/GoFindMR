using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureAttribute: MonoBehaviour
{
    public int ID;
    public PicturePointerData PointerData;
    public double latitude;
    public double longitude;
    public float heading;
    public void setlatlon(double lat, double lon, float heading)
    {
        this.latitude = lat;
        this.longitude = lon;
        this.heading = heading;
    }
}
