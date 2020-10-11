using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerialUISummoner : MonoBehaviour
{
    public float minDistance = 2;
    public bool showing = false;
    public float delay = 0.1f;
    
    protected Animator[] children;
    // Start is called before the first frame update
    void Start()
    {
        children = GetComponentsInChildren<Animator>();
        for (int a = 0; a < children.Length; a++)
        {
            children[a].SetBool("Shown",showing);
        }

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 delta = Camera.main.transform.position = transform.position;
        if (delta.magnitude < minDistance)
        {
            if(showing) return;
            StartCoroutine("ActivateInTurn");
        }
        else
        {
            if(! showing) return;
            StartCoroutine("DeactiveInTurn");
        }
    }

    public IEnumerator ActivateInTurn()
    {
        showing = true;
        
        yield return new WaitForSeconds(delay);
        for (int a = 0; a < children.Length; a++)
        {
            children[a].SetBool("Shown",true);
            yield return new WaitForSeconds(delay);
        }
    }

    public IEnumerator DeactiveInTurn()
    {
        showing = false;
        yield return new WaitForSeconds(delay);
        for (int a = 0; a < children.Length; a++)
        {
            children[a].SetBool("Shown",false);
            yield return new WaitForSeconds(delay);

        }
    }
    
}
