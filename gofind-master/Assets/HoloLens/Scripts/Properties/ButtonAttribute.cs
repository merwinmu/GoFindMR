using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Assets.HoloLens.Scripts.Properties;
using UnityEngine;

public class ButtonAttribute : MonoBehaviour
{
   private int id;
   private POICoordinatesObject poiCoordinatesObject;
   

   public POICoordinatesObject GETCoordinates()
   {
      return poiCoordinatesObject;
   }

   public int getID()
   {
      return id;
   }

   public void setID_coordinates(int id, POICoordinatesObject poiCoordinatesObject)
   {
      this.id = id;
      this.poiCoordinatesObject = poiCoordinatesObject;
   }
}
