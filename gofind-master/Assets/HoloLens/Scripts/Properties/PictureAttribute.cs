using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PictureAttribute: MonoBehaviour
{
    public int ID;
    public PicturePointerData PointerData;
    public double latitude;
    public double longitude;

    public void setlatlon(double lat, double lon)
    {
        this.latitude = lat;
        this.longitude = lon;
    }
}
