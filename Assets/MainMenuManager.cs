using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartPressed()
    {
        SceneManager.LoadScene(1);
    }

    public void ExitPressed()
    {
        Application.Quit();
    }
}