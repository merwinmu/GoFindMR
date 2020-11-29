using System;
using Assets.HoloLens.Scripts.Model;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEngine;

public class DistanceAttribute : MonoBehaviour
{
    public int distance = 500;
    public static event EventHandler<DistanceEventArgs> OnDistanceChanged = (sender, e) => { };
    
    // Start is called before the first frame update
    public void updateDistanceValue(SliderEventData eventData)
    {
        distance = convertToDistance(eventData.NewValue);
        
        setText(distance.ToString());
        var EventArgs = new DistanceEventArgs();
        EventArgs.distance_value = distance;
        OnDistanceChanged(this, EventArgs);
    }
    
    public void setText(string value)
    {
        GetComponent<TextMeshPro>().SetText(value);
    }
    
    public int convertToDistance(float value)
    {
        return Convert.ToInt32(value*1000);
    }

    private void Start()
    {
        var EventArgs = new DistanceEventArgs();
        EventArgs.distance_value = distance;
        OnDistanceChanged(this, EventArgs);
    }
}

public class DistanceEventArgs : EventArgs
{
    public int distance_value;
}