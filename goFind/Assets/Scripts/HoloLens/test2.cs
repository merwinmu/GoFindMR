using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

#if !UNITY_EDITOR
using HoloBeaconScanner;
#endif


public class test2 : MonoBehaviour
{
    // Start is called before the first frame update
    TextMeshPro textMeshPro;

    void Start()
    {
#if !UNITY_EDITOR
        Scanner beaconScanner = new Scanner();
        Scanner.ScannedBeaconEventHandler += BeaconFound;
        beaconScanner.StartScanning();
#endif
    }
    public void BeaconFound(string str)
    {
        try
        {
            char[] delimiterChars = { '*' };
            string[] components = str.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
            if (components[1] != "0")
            {
                Debug.Log("[BEACON DETECTOR] : FOUND : " + str.ToString());
                textMeshPro.text = str.ToString();
            }
        }
        catch(Exception ex)
        {
            //Log error or something else.
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Awake()
    {
        textMeshPro = GetComponent<TextMeshPro>();
        textMeshPro.text = "Example";

    }
}
