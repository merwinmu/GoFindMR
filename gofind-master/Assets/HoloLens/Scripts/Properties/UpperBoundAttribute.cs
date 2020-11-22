using System;
using Assets.HoloLens.Scripts.Model;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEngine;

public class UpperBoundAttribute : MonoBehaviour
{
    public static event EventHandler<SliderValueEventArgs> OnUpperBoundValueChanged = (sender, e) => { };
    
    // Start is called before the first frame update
    public void updateTemporalValue(SliderEventData eventData)
    {
        DateTime dateTime = convertToYear(eventData.NewValue);
        
        setText(dateTime.Year.ToString());
        
        var EventArgs = new SliderValueEventArgs();
        EventArgs.value = eventData.NewValue;
        OnUpperBoundValueChanged(this, EventArgs);
    }
    
    public void setText(string value)
    {
        GetComponent<TextMeshPro>().SetText(value);
    }

    public DateTime convertToYear(float value)
    {
        float n_value = value * 16000000000000;
        double ticks = n_value;
        TimeSpan timeSpan = TimeSpan.FromMilliseconds(ticks);
        DateTime startdate = new DateTime(1500, 1, 1) + timeSpan;
        return startdate;
    }
}
