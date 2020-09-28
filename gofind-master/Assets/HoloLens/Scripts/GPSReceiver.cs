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

public class GPSReceiver : MonoBehaviour
{
    public double latitude = 0;
#if WINDOWS_UWP
    BluetoothLEAdvertisementWatcher watcher;
    public static ushort BEACON_ID = 24;
#endif
    public TextMeshPro tmp;

    private void Awake()
    {
        tmp = GetComponent<TextMeshPro>();
#if WINDOWS_UWP
        watcher = new BluetoothLEAdvertisementWatcher();
        var manufacturerData = new BluetoothLEManufacturerData
        {
        CompanyId = BEACON_ID
        };
        watcher.AdvertisementFilter.Advertisement.ManufacturerData.Add(manufacturerData);
        watcher.ScanningMode = BluetoothLEScanningMode.Active;
        watcher.Received += Watcher_Received;
        watcher.Start();
        latitude = 5;
        Debug.Log("Started watching");
#endif
    }

#if WINDOWS_UWP
    private async void Watcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
    {
        ushort identifier = args.Advertisement.ManufacturerData[0].CompanyId;
        byte[] data = args.Advertisement.ManufacturerData[0].Data.ToArray();
        // Updates to Unity UI don't seem to work nicely from this callback so just store a reference to the data for later processing.
        latitude = BitConverter.ToDouble(data, 0);
        Debug.Log(latitude.ToString());
    }
#endif

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        tmp.text = latitude.ToString();
    }
}
