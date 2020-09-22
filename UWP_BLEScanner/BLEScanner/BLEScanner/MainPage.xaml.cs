using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BLEScanner
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Windows.Devices.Bluetooth.Advertisement.BluetoothLEAdvertisementWatcher watcher;
        public static ushort CUSTOM_ID = 24;
        double latitude = 0;
        double longitude = 0;
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void Watcher_Received(Windows.Devices.Bluetooth.Advertisement.BluetoothLEAdvertisementWatcher sender,
        Windows.Devices.Bluetooth.Advertisement.BluetoothLEAdvertisementReceivedEventArgs args)
        {
            ushort identifier = args.Advertisement.ManufacturerData.First().CompanyId;
            byte[] data = args.Advertisement.ManufacturerData.First().Data.ToArray();
            var ignore = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {

                latitude = BitConverter.ToDouble(data, 0);
                longitude = BitConverter.ToDouble(data, 8);

                TextBlock.Text = latitude.ToString();
                /* GPS Data Parsing / UI integration goes here */
            });
        }


        private void Scan_Click(object sender, RoutedEventArgs e)
        {
            if (watcher != null) watcher.Stop();
            watcher = new BluetoothLEAdvertisementWatcher();
            var manufacturerData = new BluetoothLEManufacturerData { CompanyId = CUSTOM_ID };
            watcher.AdvertisementFilter.Advertisement.ManufacturerData.Add(manufacturerData);
            watcher.Received += Watcher_Received;
            watcher.Start();
        }

        private void deserialize()
        {
            string lat, lng;
            if (latitude > 0)
            {
                lat = string.Format("{0:0.00} ???????????????????????????????????'??N", latitude);
            }
            else
            {
                lat = string.Format("{0:0.00} ???????????????????????????????????'??S", -latitude);
            }

            if (longitude > 0)
            {
                lng = string.Format("{0:0.00} ???????????????????????????????????'??E", longitude);
            }
            else
            {
                lng = string.Format("{0:0.00} ???????????????????????????????????'??W", -longitude);
            }
            TextBlock.Text = lat;
        }

    }
}
