using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Interface_Config_Res : MonoBehaviour
{
    public Button res1;
    public Button res2;
    public Button res3;
    private bool full = true;

    void Awake()
    {
        var rot = GetComponent<UIDocument>();

        res1 = rot.rootVisualElement.Q<Button>("Resol1");
        res2 = rot.rootVisualElement.Q<Button>("Resol2");
        res3 = rot.rootVisualElement.Q<Button>("Resol3");

        res1.clicked += res1Pressed;
        res2.clicked += res2Pressed;
        res3.clicked += res3Pressed;
    }

    void res1Pressed()
    {
        Screen.SetResolution(640, 360, true);
        if(full == true)
        {
           Screen.fullScreen = false;
        }
    }

    void res2Pressed()
    {
        Screen.SetResolution(1280, 720, true);
        if(full == true)
        {
           Screen.fullScreen = false;
        }
    }

    void res3Pressed()
    {
        Screen.SetResolution(1920, 1080, true);
        if(full == false )
        {
            Screen.fullScreen = true;
        }
    }
}
