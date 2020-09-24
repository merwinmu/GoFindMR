package com.example.gofindhololenscompanion;
import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;

import android.Manifest;
import android.app.ActivityManager;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothManager;
import android.bluetooth.le.AdvertiseCallback;
import android.bluetooth.le.AdvertiseData;
import android.bluetooth.le.AdvertiseSettings;
import android.bluetooth.le.BluetoothLeAdvertiser;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.Toast;

import java.nio.ByteBuffer;
import java.util.Arrays;
import java.util.Timer;
import java.util.TimerTask;

public class MainActivity extends AppCompatActivity{

    private static final int REQUEST_CODE_LOCATION_PERMISSION = 1;
    BluetoothAdapter mBAdapter;
    BluetoothManager mBManager;
    BluetoothLeAdvertiser mBLEAdvertiser;
    Button advertiseButton;
    int count = 0;
    public Timer myTimer;
    boolean isAdvertismentRunning = false;
    public static double LATITUDE = 0;
    public static double LONGITUDE = 0;
    private static final int CUSTOM_ID = 24;


    AdvertiseCallback advertisingCallback = new AdvertiseCallback() {
        @Override
        public void onStartSuccess(AdvertiseSettings settingsInEffect) {
            super.onStartSuccess(settingsInEffect);
        }

        @Override
        public void onStartFailure(int errorCode) {
            if (errorCode != ADVERTISE_FAILED_ALREADY_STARTED) {
                String msg = "Service failed to start: " + errorCode;
            } else {
                restartAdvertising();
            }
        }
    };


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);


        findViewById(R.id.request_location_updates_button).setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if(ContextCompat.checkSelfPermission(
                        getApplicationContext(), Manifest.permission.ACCESS_FINE_LOCATION) != PackageManager.PERMISSION_GRANTED){
                    ActivityCompat.requestPermissions(MainActivity.this, new String[]{
                            Manifest.permission.ACCESS_FINE_LOCATION},REQUEST_CODE_LOCATION_PERMISSION);
                    } else{
                    startLocationService();
                }
            }
        });

        findViewById(R.id.remove_updates_button).setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                stopLocationService();
            }
        });


        BluetoothAdapter mBluetoothAdapter = BluetoothAdapter.getDefaultAdapter(); // getting permission to use Bluetooth
        if (mBluetoothAdapter == null) {
            // Device does not support Bluetooth
        } else if (!mBluetoothAdapter.isEnabled()) {
            // Bluetooth is not enabled :)
            Toast.makeText(MainActivity.this, "PLEASE ENABLE BLUETOOTH", Toast.LENGTH_LONG).show();
            startActivityForResult(new Intent(android.provider.Settings.ACTION_SETTINGS), 0);
        }

        if( !BluetoothAdapter.getDefaultAdapter().isMultipleAdvertisementSupported() ) {
            Toast.makeText( this, "Multiple advertisement not supported", Toast.LENGTH_SHORT ).show();
            advertiseButton.setEnabled( false );
        }


        mBManager = (BluetoothManager) getSystemService(BLUETOOTH_SERVICE);
        mBAdapter = mBManager.getAdapter();

        myTimer = new Timer();
        myTimer.schedule(new TimerTask() {
            @Override
            public void run() {
                if(isAdvertismentRunning){
                    stopAdvertising();
                    advertise();
                }
                else {
                    advertise();
                    isAdvertismentRunning=true;

                }
            }
        },0,1000);
    }


    @Override
    public void onRequestPermissionsResult(int requestCode, @NonNull String[] permissions, @NonNull int[] grantResults) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults);
        if(requestCode == REQUEST_CODE_LOCATION_PERMISSION && grantResults.length > 0){
            startLocationService();
        }else{
            Toast.makeText(this,"Permission denied", Toast.LENGTH_SHORT).show();
        }
    }


    private boolean isLocationServiceRunning(){
        ActivityManager activityManager =
                (ActivityManager) getSystemService(Context.ACTIVITY_SERVICE);
        if(activityManager != null){
            for(ActivityManager.RunningServiceInfo service: activityManager.getRunningServices(Integer.MAX_VALUE)){
                if(LocationService.class.getName().equals(service.service.getClassName())){
                    return true;
                }

            }
        }
        return false;
    }


    private void startLocationService(){
        if(!isLocationServiceRunning()){
            Intent intent = new Intent(getApplicationContext(), LocationService.class);
            intent.setAction(Constants.ACTION_START_LOCATION_SERVICE);
            startService(intent);
            Toast.makeText(this, "Location service started", Toast.LENGTH_SHORT).show();
        }
    }

    private void stopLocationService(){
        if (isLocationServiceRunning()){
            Intent intent = new Intent(getApplicationContext(),LocationService.class);
            intent.setAction(Constants.Action_STOP_LOCATION_SERVICE);
            startService(intent);
            Toast.makeText(this,"Location service stopped", Toast.LENGTH_SHORT).show();
        }
    }

    private void advertise(){
        mBLEAdvertiser = mBAdapter.getBluetoothLeAdvertiser();

        AdvertiseSettings settings = new AdvertiseSettings.Builder()
                .setAdvertiseMode( AdvertiseSettings.ADVERTISE_MODE_LOW_LATENCY )
                .setTxPowerLevel( AdvertiseSettings.ADVERTISE_TX_POWER_HIGH )
                .setConnectable( false )
                .build();


        byte[] lat = ByteBuffer.allocate(8).putDouble(LATITUDE).array();
        byte[] lon = ByteBuffer.allocate(8).putDouble(LONGITUDE).array();

        byte[] payload = new byte[lat.length + lon.length];
        System.arraycopy(lat, 0, payload, 0, lat.length);
        System.arraycopy(lon, 0, payload, lat.length, lon.length);


        AdvertiseData data = new AdvertiseData.Builder()
                .setIncludeDeviceName( true )
                .addManufacturerData(CUSTOM_ID,payload)
           //     .addServiceData( pUuid, "Data".getBytes( Charset.forName( "UTF-8" ) ) )
                .build();

        Log.d("Location: Latitude:",LATITUDE + ", "+ " Longitude "+ LONGITUDE);
        Log.d("PAYLOAD IN BINARY", Arrays.toString(payload));
        Log.d("PAYLOAD IN HEX", byteArrayToHex(payload));


        mBLEAdvertiser.startAdvertising( settings, data, advertisingCallback );
        count++;
        Log.e("COUNT","count: " + count);
    }

    private void stopAdvertising() {
        if (mBLEAdvertiser == null) return;
        mBLEAdvertiser.stopAdvertising(advertisingCallback);
    }

    private void restartAdvertising() {
        stopAdvertising();
        advertise();
    }

    public static String byteArrayToHex(byte[] a) {
        StringBuilder sb = new StringBuilder(a.length * 2);
        for(byte b: a)
            sb.append(String.format("%02x", b));
        return sb.toString();
    }

}
