using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

#if !UNITY_EDITOR
using Windows.Devices.Bluetooth.Advertisement;
#endif

public class test : MonoBehaviour
{
    
    TextMeshPro textMeshPro;
    public string latitude;
    public string log;

#if !UNITY_EDITOR    
    BluetoothLEAdvertisementWatcher scanner;
#endif
    // Start is called before the first frame update
    void Start()
    {
#if !UNITY_EDITOR

        scanner = new BluetoothLEAdvertisementWatcher
            {
                ScanningMode = BluetoothLEScanningMode.Passive
            };

        var manufacturerData = new BluetoothLEManufacturerData()
        {
            CompanyId =  24 
        };
        scanner.AdvertisementFilter.Advertisement.ManufacturerData.Clear();
        scanner.AdvertisementFilter.Advertisement.ManufacturerData.Add(manufacturerData);


       

        ////set some thresholds
        //scanner.SignalStrengthFilter.InRangeThresholdInDBm = -50;
        //scanner.SignalStrengthFilter.OutOfRangeThresholdInDBm = -100;
        scanner.SignalStrengthFilter.OutOfRangeTimeout = TimeSpan.FromMilliseconds(2000);

        //setup callbacks
            scanner.Received += OnBeaconFound;
       // scanner.Stopped += OnScannerStopped;

        scanner.Start();

#endif
    }
    
#if !UNITY_EDITOR

        private void OnBeaconFound(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {

            try
            {
                UnityEngine.WSA.Application.InvokeOnAppThread (() =>
                {
                    
                    
                    {
                        string RSSI = args.RawSignalStrengthInDBm.ToString();
                    };


                    //check for manufacturer data section
                    var beaconDataString = string.Empty;

                    var beaconDataSections = args.Advertisement.ManufacturerData;
                    if (beaconDataSections.Count > 0)
                    {
                        //print first data section for now
                        var beaconData = beaconDataSections[0];
                        var beaconDataBytes = new byte[beaconData.Data.Length];
                        beaconDataString = BitConverter.ToString(beaconDataBytes);
                        latitude = beaconDataString;

                    }

               }, true );
            }
            catch (Exception ex)
            {
                 log = ex.Message;
            }
    }
#endif
    

    // Update is called once per frame
    void Update()
    {
        textMeshPro.text = latitude;
    }

     void Awake()
    {
        textMeshPro = GetComponent<TextMeshPro>();
        textMeshPro.text = "Example";

    }
}

