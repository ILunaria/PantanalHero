using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu_Scripts : MonoBehaviour
{
    public void Game_Start()
    {
        SceneManager.LoadScene("Fase_1");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
