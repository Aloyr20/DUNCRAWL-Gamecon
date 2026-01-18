using System.Collections.Generic;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public List<string> _sceneNames;
    public AudioSource [] music;

    public void Start()
    {
        music[0].Play();
    }

    public void LoadScene(string sceneName)
    {
        music[0].Stop();

        //Load the scene
        SceneManager.LoadScene(sceneName);
    }

    public void QuitButton()
    {
#if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false;

#else

        Application.Quit();

#endif
    }
}
