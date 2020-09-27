using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageLoader : MonoBehaviour
{
    public string URL =
        "https://www.wallpaperup.com/uploads/wallpapers/2014/05/02/348237/418634e46415dc2424a305da3a77cd1b.jpg";

    public Renderer thisRenderer;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadFromLikeCoroutine());
        
        thisRenderer.material.color = Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator LoadFromLikeCoroutine()
    {
     Debug.Log("Loading...");   
     WWW wwwLoader = new WWW(URL);
     yield return wwwLoader;
     
     Debug.Log("Loaded");
     thisRenderer.material.color = Color.white;
     thisRenderer.material.mainTexture = wwwLoader.texture;
    }
}
