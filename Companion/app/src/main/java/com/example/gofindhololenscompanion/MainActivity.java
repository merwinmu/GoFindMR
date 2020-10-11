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
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.os.Bundle;
import android.os.Debug;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

import java.nio.ByteBuffer;
import java.util.Arrays;
import java.util.Timer;
import java.util.TimerTask;

public class MainActivity extends AppCompatActivity implements SensorEventListener {

    private static final int REQUEST_CODE_LOCATION_PERMISSION = 1;
    BluetoothAdapter mBAdapter;
    BluetoothManager mBManager;
    BluetoothLeAdvertiser mBLEAdvertiser;
    Button advertiseButton;
    int count = 0;
    EditText latitude_text;
    EditText longitude_text;
    public Timer myTimer;
    boolean isAdvertismentRunning = false;
    public static double LATITUDE = 0;
    public static double LONGITUDE = 0;
    private static final int CUSTOM_ID = 24;

    // device sensor manager
    private SensorManager mSensorManager;
    private static float HEADING = 0f;


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

        latitude_text = (EditText)findViewById(R.id.latitude);
        longitude_text = (EditText)findViewById(R.id.longitude);


        findViewById(R.id.set_payload).setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                LATITUDE = Double.parseDouble(latitude_text.getText().toString());
                LONGITUDE = Double.parseDouble(longitude_text.getText().toString());
                Toast.makeText(MainActivity.this, "PAYLOAD SET", Toast.LENGTH_LONG).show();

            }
        });

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

        // initialize android device sensor capabilities
        mSensorManager = (SensorManager) getSystemService(SENSOR_SERVICE);
    }

    @Override
    protected void onResume() {
        super.onResume();
        // for the system's orientation sensor registered listeners
        mSensorManager.registerListener(this, mSensorManager.getDefaultSensor(Sensor.TYPE_ORIENTATION),
                SensorManager.SENSOR_DELAY_GAME);
    }

    @Override
    protected void onPause() {
        super.onPause();
        mSensorManager.unregisterListener(this);
    }

    @Override
    public void onSensorChanged(SensorEvent sensorEvent) {
        HEADING = Math.round(sensorEvent.values[0]);
    }

    @Override
    public void onAccuracyChanged(Sensor sensor, int i) {
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
                .setAdvertiseMode( AdvertiseSettings.ADVERTISE_MODE_BALANCED )
                .setTxPowerLevel( AdvertiseSettings.ADVERTISE_TX_POWER_MEDIUM )
                .setConnectable( false )
                .setTimeout(0)
                .build();

        byte[] payload = new byte[20];

        byte[] lat = ByteBuffer.allocate(8).putDouble(LATITUDE).array();
        for(int i = 0, j = 7; i < 8; i++, j--) payload[i] = lat[j];

        byte[] lon = ByteBuffer.allocate(8).putDouble(LONGITUDE).array();
        for(int i = 8, j = 7; i < 16; i++, j--) payload[i] = lon[j];

        byte[] hed = ByteBuffer.allocate(4).putFloat(HEADING).array();
        for(int i = 16, j = 3; i < 20; i++, j--) payload[i] = hed[j];


        AdvertiseData data = new AdvertiseData.Builder()
                .addManufacturerData(CUSTOM_ID,payload)
                .build();

//        Log.d("Location: Latitude:",LATITUDE + ", "+ " Longitude "+ LONGITUDE + " HEADING "+ HEADING);
        Log.d("PAYLOAD IN BINARY", Arrays.toString(payload));
//        Log.d("PAYLOAD IN HEX", byteArrayToHex(payload));
//        Log.d("ORGINAL PAYLOAD: ",data.toString());

        mBLEAdvertiser.startAdvertising( settings, data, advertisingCallback );
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
