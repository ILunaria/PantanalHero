using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interface_Ingame : MonoBehaviour
{
    public GameObject canvas;

    public void abrirmenu()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            canvas.SetActive(true);
        }
    }
}
