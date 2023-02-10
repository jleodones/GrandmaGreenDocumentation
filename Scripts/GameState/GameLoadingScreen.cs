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
        SceneManager.sceneLoaded += DisableOnGameStartup;
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

    void DisableOnGameStartup(Scene scene, LoadSceneMode loadSceneMode)
    {
        SceneManager.sceneLoaded -= DisableOnGameStartup;
        LoadingEnd();
    }

    public void LoadingEnd(SCENES scene) => LoadingEnd();

    protected virtual void LoadingEnd()
    {
        canvas.enabled = false;
    }
}
