using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conig_Script : MonoBehaviour
{   
    public bool fullScreen = true;
    
    public void res1(){
        Screen.SetResolution(640, 360, true);
        if(fullScreen == true)
            Screen.fullScreen = false;
    }

    public void res2(){
        Screen.SetResolution(1280, 720, true);
        if(fullScreen == true)
            Screen.fullScreen = false;
    }

    public void res3(){
        Screen.SetResolution(1920,1080,true);
        if(fullScreen == false)
            Screen.fullScreen = true;
    }
}
