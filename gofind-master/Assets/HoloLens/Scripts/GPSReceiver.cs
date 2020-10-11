using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine;
#if WINDOWS_UWP
using System;
using Windows.Devices.Bluetooth.Advertisement;
using System.Runtime.InteropServices.WindowsRuntime;
#endif

/*
 * Official Microsoft DOC
 * https://docs.microsoft.com/en-us/windows/uwp/devices-sensors/ble-beacon
 */

public class GPSReceiver : MonoBehaviour
{
    public static ushort ANDROID_ID = 24;
    public double latitude = 0;
    public double longitude = 0;
    public double heading = 0f;
    private TextMeshPro BLE_Text;
#if WINDOWS_UWP
    BluetoothLEAdvertisementWatcher watcher; //Loading Bluetooth Low Energy Advertisment Driver
#endif
   

    private void Awake()
    {
        BLE_Text = GetComponent<TextMeshPro>(); 

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
        latitude = BitConverter.ToDouble(data, 0);
        longitude = BitConverter.ToDouble(data,8);
        heading  = BitConverter.ToFloat(data,16);
        Debug.Log(latitude.ToString());
    }
#endif

    void Start()
    {
        BLE_Text.text = "Waiting for GPS and heading Advertisment from Android_ID: " + ANDROID_ID;
    }

    // Update is called once per frame
    void Update()
    {
        BLE_Text.text =" Latitude: " +  latitude.ToString() + " Longitude: " + longitude.ToString() + " Heading: " + heading;
    }
}
