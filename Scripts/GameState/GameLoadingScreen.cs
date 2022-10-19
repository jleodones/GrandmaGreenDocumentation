using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Core.SceneManagement;
using UnityEngine.UI;

public class GameLoadingScreen : MonoBehaviour
{
    public Canvas canvas;

    void Awake()
    {
        //Sce.sceneLoaded += DisableOnFirstSceneLoad;
    }

    void OnDestroy()
    {

    }

    void OnEnable()
    {
        SceneHandler.onSceneLoadStart += LoadingStart;
        SceneHandler.onSceneLoaded += LoadingEnd;
    }

    void OnDisable()
    {
        SceneHandler.onSceneLoadStart -= LoadingStart;
        SceneHandler.onSceneLoaded -= LoadingEnd;
    }

    protected virtual void LoadingStart(SCENES scene)
    {
        canvas.enabled = true;
    }

    protected virtual void LoadingEnd(SCENES scene)
    {
        canvas.enabled = false;
    }
}
