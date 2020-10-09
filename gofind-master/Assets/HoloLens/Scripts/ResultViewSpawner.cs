using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{
    public Transform result1;
    public int resultNr = 10;
    
    Vector3 startVector = new Vector3(-0.5f,0.0f,2);
    
    List<Transform> TransformPositions = new List<Transform>();
    List<Vector3> ResultPositions = new List<Vector3>();

    public GameObject spawnee;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    public void fieldOfView()
    {
        
    }

    public void createStartVectors()
    {   ResultPositions.Add(startVector);
        
        float offset = 0.3f;
        
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
        if (Input.GetKey("up"))
        {
            createStartVectors();
        }
    }

    public void OnClickListView()
    {
        createStartVectors();
    }

    public void calculatePosition()
    {
        
    }
}
 