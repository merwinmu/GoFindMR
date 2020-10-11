using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneController : MonoBehaviour
{
    private float secondsToLoadNextScene = 0.5f;
    private static int lastScene;
    private int mainScene = 1;
    private int currentScene;
    public Canvas GameObjectcanvas;

    public static Stack<int> sceneStack = new Stack<int>();

    private void Awake()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
    }

    private void Start()
    {
        GameObjectcanvas.enabled = false;
    }

    private void LoadNewScene(int sceneToLoad)
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
        sceneStack.Push(currentScene);
        SceneManager.LoadScene(sceneToLoad);
        Debug.Log("New Scene Loaded");
    }

    private void LoadLastScene()
    {
        lastScene = sceneStack.Pop();
        SceneManager.LoadScene(lastScene);
        Debug.Log("Last Scene Loaded");
    }

    public void backScene()
    {
        LoadLastScene();
    }

    public void imageQuery()
    {
        LoadNewScene(1);
    }

    public void spatialQuery()
    {
        LoadNewScene(2);
    }

    public void positionQuery()
    {
        LoadNewScene(3);
    }

    public void temporalQuery()
    {
        GameObjectcanvas.enabled = !GameObjectcanvas.enabled;
    }
}
