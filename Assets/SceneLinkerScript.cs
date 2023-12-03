using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLinkerScript : MonoBehaviour
{
    public string GoToScene;
    public void LoadScene()
    {
        SceneManager.LoadScene(GoToScene);
        Debug.Log("LoadedScene");
    }
}
