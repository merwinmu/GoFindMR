using System;
using TMPro;
using UnityEngine;
#if NETFX_CORE
using Windows.Devices.Bluetooth.Advertisement;
using System.Runtime.InteropServices.WindowsRuntime;
#endif

public class GPSReceiver : MonoBehaviour
{
    TextMeshPro textMeshPro;
    public static double latitude;


#if NETFX_CORE
BluetoothLEAdvertisementWatcher watcher;
public static ushort BEACON_ID = 24;
#endif

    //private EventProcessor eventProcessor;

    private void Start()
    {
        latitude = 0.2;
    }

    void Awake()
    {
        textMeshPro = GetComponent<TextMeshPro>();


        //eventProcessor = (EventProcessor)FindObjectOfType(typeof(EventProcessor));

#if NETFX_CORE
watcher = new BluetoothLEAdvertisementWatcher();
var manufacturerData = new BluetoothLEManufacturerData {CompanyId = BEACON_ID};
watcher.AdvertisementFilter.Advertisement.ManufacturerData.Add(manufacturerData);
watcher.Received += Watcher_Received;
watcher.Start();
#endif

#if NETFX_CORE
        private async void Watcher_Received(BluetoothLEAdvertisementWatcher sender,
            BluetoothLEAdvertisementReceivedEventArgs args)
        {
            ushort identifier = args.Advertisement.ManufacturerData[0].CompanyId;
            byte[] data = args.Advertisement.ManufacturerData[0].Data.ToArray();
            eventProcessor.QueueEvent(() => { });
            latitude = BitConverter.ToDouble(data, 0);
           
            
        }
#endif
    }

    void Update()
    {
        textMeshPro.text = latitude.ToString();

    }
}