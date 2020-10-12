using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultViewSpawner : MonoBehaviour
{
    public Transform result1;
    public int resultNr = 10;

    public string url =
        "https://upload.wikimedia.org/wikipedia/commons/thumb/b/bf/Universit%C3%A4t_Basel_2018_logo.svg/800px-Universit%C3%A4t_Basel_2018_logo.svg.png";

    public Renderer thisRenderer;
    
    Vector3 startVector = new Vector3(-0.5f,0.0f,2);
    
    List<Transform> TransformPositions = new List<Transform>();
    List<Vector3> ResultPositions = new List<Vector3>();

    public GameObject spawnee;
    // Start is called before the first frame update
    void Start()
    {
       // StartCoroutine(LoadFromLikeCouroutine());
        thisRenderer.material.color = Color.red;
    }

    // private IEnumerator LoadFromLikeCouroutine()
    // {
    //     WWW wwloader = WWW(url);
    //     yield return wwloader;
    //     this.thisRenderer.material.color = Color.white;
    //     this.thisRenderer.material.mainTexture = wwloader.texture;
    // }

    public void fieldOfView()
    {
        
    }

    public void createStartVectors()
    {   ResultPositions.Add(startVector);
        
        float offset = 0.5f;
        
        Vector3 currentPos = startVector;
      
        for(int i = 0; i < 3; i++)
        {
            Instantiate(spawnee, currentPos, result1.rotation);
            currentPos.x = offset + currentPos.x;
            Debug.Log(currentPos + " created");
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnClickListView()
    {
        createStartVectors();
    }

    public void calculatePosition()
    {
        
    }
}
 