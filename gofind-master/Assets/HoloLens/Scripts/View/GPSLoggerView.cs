using System;
using System.Collections.Generic;
using Assets.HoloLens.Scripts.Properties;
using TMPro;
using UnityEngine;
#if WINDOWS_UWP
using System;
using Windows.Devices.Bluetooth.Advertisement;
using System.Runtime.InteropServices.WindowsRuntime;
#endif

/*
 * Views are primarly used for Input and Output. It is primarly a Monobehaviour class with the associate functions 
 * Input actions such as OnClick invoke an event to the controller which then executes a function to model
 * Output actions are in example rendering gameobjects etc.
 */



/*
 * Various EventArgs has been created so that if an Input occurs , a callback can be
 * invoked to the controller which then sends it to the model
 */

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
    //
    void setGPSTextMesh(double latitude, double longitude, float heading);



}
// Implementation of the GPSLoggerView view
public class GPSLoggerView : MonoBehaviour, IGPSLoggerView
{
    private static ushort ANDROID_ID = 24;
    
    private TextMeshPro BLT_Text;
    

    public double currentLatitude = 0;
    public double currentLongitude = 0;
    public float currentheading = 0;

    private double previousLatitude = 0;
    private double previousLongitude = 0;
    private float previousheading = 0;
    
    private List<Coordinates> currentCoordinates;
    private int currentCoordinatesSize;
    public event EventHandler<GPSDataReceivedEventArgs> OnReceived = (sender, e) => { };
    
    
#if WINDOWS_UWP
    BluetoothLEAdvertisementWatcher watcher; //Loading Bluetooth Low Energy Advertisment Driver
#endif
    private void Awake()
    {
        BLT_Text =  GetComponent<TextMeshPro>(); 
        currentCoordinates = new List<Coordinates>();

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
                currentLatitude = BitConverter.ToDouble(data, 0);
                currentLongitude = BitConverter.ToDouble(data,8);
                currentheading = BitConverter.ToSingle(data,16);
                //var eventArgs = new GPSDataReceivedEventArgs(BitConverter.ToDouble(data, 0),BitConverter.ToDouble(data,8),BitConverter.ToSingle(data,16));
                //Debug.Log(BitConverter.ToDouble(data, 0));
                currentCoordinates.Add(new Coordinates(currentLatitude,currentLongitude,currentheading));
    }
#endif

    //OUTPUT


    //Nasty way of updating BLT textmeshpro
    private bool update_BLT_text;
    private string Latitide_text;
    private string Longitude_text;
    private string heading_text;
    
    public void setGPSTextMesh(double latitude, double longitude, float heading)
    {
        Latitide_text = latitude.ToString();
        Longitude_text = longitude.ToString();
        heading_text = heading.ToString();

        update_BLT_text = true;
        
        //Debug.Log("SETTING TEXTMESH TO THIS" + latitude );
    }
    
    // Start is called before the first frame update

    
    
    //INPUT
    // Update is called once per frame
    void Update()
    {

        if (update_BLT_text)
        {
            BLT_Text.text = " Latitude: " +  Latitide_text + " Longitude: " + Longitude_text + " Heading: " + heading_text;
            update_BLT_text = false;

        }
        // If the primary mouse button was pressed this frame
        if (Input.GetMouseButtonDown(0))
        {
            //Debug
            var eventArgs = new GPSDataReceivedEventArgs(currentLatitude,currentLongitude,currentheading);
            OnReceived(this, eventArgs);
        }

        if (currentCoordinates.Count != 0)
        {
            this.currentLatitude = this.currentCoordinates[currentCoordinates.Count-1].getLat();
            this.currentLongitude = this.currentCoordinates[currentCoordinates.Count-1].getLon();
            this.currentheading = this.currentCoordinates[currentCoordinates.Count-1].gethead();

            var eventArgs = new GPSDataReceivedEventArgs(this.currentLatitude,this.currentLongitude,this.currentheading);
            OnReceived(this, eventArgs);
            if (currentCoordinates.Count > 2)
            {
                Debug.Log(currentCoordinates.ToString());
                currentCoordinates.RemoveAt(0);
            }
        }
    }
}
