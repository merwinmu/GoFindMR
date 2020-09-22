using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GPSUpdate : MonoBehaviour

{
    TextMeshPro textMeshPro;

    int score;
    // Start is called before the first frame update
    void Start()
    {
        score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        textMeshPro.text = score.ToString();
        score++;
    }

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshPro>();
        textMeshPro.text = "Example";
    }
}
