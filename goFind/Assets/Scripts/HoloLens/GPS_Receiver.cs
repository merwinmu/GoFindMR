using System;
using TMPro;
using UnityEngine;
#if WINDOWS_UWP
using Windows.Devices.Bluetooth.Advertisement;
using System.Runtime.InteropServices.WindowsRuntime;
#endif

public class GPS_Receiver : MonoBehaviour
{
    public double latitude = 0;

#if WINDOWS_UWP
    BluetoothLEAdvertisementWatcher watcher;
    public static ushort BEACON_ID = 24;
#endif
   
    private TextMeshPro tmp;

    void Awake()
    {
        tmp = GetComponent<TextMeshPro>();
        tmp.text = "RUNNING";
#if WINDOWS_UWP
        watcher = new BluetoothLEAdvertisementWatcher();
        var manufacturerData = new BluetoothLEManufacturerData
        {
        CompanyId = BEACON_ID
        };
        watcher.AdvertisementFilter.Advertisement.ManufacturerData.Add(manufacturerData);
        watcher.Received += Watcher_Received;
        watcher.Start();
#endif
    }
#if WINDOWS_UWP
    private async void Watcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
    {
        ushort identifier = args.Advertisement.ManufacturerData[0].CompanyId;
        byte[] data = args.Advertisement.ManufacturerData[0].Data.ToArray();
        // Updates to Unity UI don't seem to work nicely from this callback so just store a reference to the data for later processing.
        latitude = BitConverter.ToDouble(data, 0);
        
    }
#endif

    private void Update()
    {
        tmp.text = latitude.ToString();
    }
}