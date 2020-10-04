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

    public static Stack<int> sceneStack = new Stack<int>();

    private void Awake()
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
    }

    private void LoadNewScene(int sceneToLoad)
    {
        currentScene = SceneManager.GetActiveScene().buildIndex;
        sceneStack.Push(currentScene);
        SceneManager.LoadScene(sceneToLoad);
    }

    private void LoadLastScene()
    {
        lastScene = sceneStack.Pop();
        SceneManager.LoadScene(lastScene);
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
        LoadNewScene(4);
    }
}
