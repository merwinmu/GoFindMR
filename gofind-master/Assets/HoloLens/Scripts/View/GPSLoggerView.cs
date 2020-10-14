using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
#if WINDOWS_UWP
using System;
using Windows.Devices.Bluetooth.Advertisement;
using System.Runtime.InteropServices.WindowsRuntime;
#endif

// Dispatched when GPS received
public class GPSDataReceivedEventArgs : EventArgs
{
    public double latitude { get; private set; }
    public double longitude { get; private set; }
    public float heading { get; private set; }
    
    public GPSDataReceivedEventArgs(double latitude, double longitude, float heading)
    {
        this.latitude = latitude;
        this.longitude = longitude;
        this.heading = heading;
        
        //Debugging
        //Debug.Log("Received event from GPS View");
    }
}

// Interface for the Near menu view
public interface IGPSLoggerView
{
    event EventHandler<GPSDataReceivedEventArgs> OnReceived;

    void setTextMeshPro(TextMeshPro gameObject);
    void setGPSTextMesh(double latitude, double longitude, float heading);



}
// Implementation of the GPSLoggerView view
public class GPSLoggerView : MonoBehaviour, IGPSLoggerView
{
    private static ushort ANDROID_ID = 24;
    private TextMeshPro gps_log;
  
    public event EventHandler<GPSDataReceivedEventArgs> OnReceived = (sender, e) => { };
    
    
#if WINDOWS_UWP
    BluetoothLEAdvertisementWatcher watcher; //Loading Bluetooth Low Energy Advertisment Driver
#endif
    private void Awake()
    {

#if WINDOWS_UWP
        watcher = new BluetoothLEAdvertisementWatcher(); //Instating BLE Driver
        var manufacturerData = new BluetoothLEManufacturerData  // Matching ID with ANDROID Device
        {
        CompanyId = ANDROID_ID 
        };
        watcher.AdvertisementFilter.Advertisement.ManufacturerData.Add(manufacturerData); // Filtering Manuf. Data with ID
        watcher.ScanningMode = BluetoothLEScanningMode.Active; // Function required for HOLOLENS2 
        watcher.Received += Watcher_Received; 
        watcher.Start();
        Debug.Log("Started watching");
#endif
    }
    
    
#if WINDOWS_UWP
    /*
     * Reading Data and translating to required values.
     */
    private async void Watcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
    {
        byte[] data = args.Advertisement.ManufacturerData[0].Data.ToArray();
        
 // Dispatch the 'OnReceived' event
                var eventArgs = new GPSDataReceivedEventArgs(BitConverter.ToDouble(data, 0),BitConverter.ToDouble(data,8),BitConverter.ToSingle(data,16));
                OnReceived(this, eventArgs);
    }
#endif

    //OUTPUT
    public void setTextMeshPro(TextMeshPro tmp)
    {
        gps_log = tmp;
    }

    public void setGPSTextMesh(double latitude, double longitude, float heading)
    {
        gps_log.text = " Latitude: " +  latitude.ToString() + " Longitude: " + longitude.ToString() + " Heading: " + heading;
    }
    
    // Start is called before the first frame update

    
    //INPUT
    // Update is called once per frame
    void Update()
    {
        // If the primary mouse button was pressed this frame
        if (Input.GetMouseButtonDown(0))
        {
            //Debug
            var eventArgs = new GPSDataReceivedEventArgs(4.3453,5.34676,12.5f);
            OnReceived(this, eventArgs);
        }
    }
}
