using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CODE_EventManager : MonoBehaviour
{
    public static CODE_EventManager current;
    // Start is called before the first frame update
    void Awake()
    {
        current = this;
    }

    public event Action OnScreenShakeCallback;

    public void ScreenShakeCallback()
    {
        if(OnScreenShakeCallback != null)
        {
            OnScreenShakeCallback();
        }
    }
}
