using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CODE_Jacare : MonoBehaviour
{
    public void ScreenShake()
    {
        CODE_EventManager.current.ScreenShakeCallback();
    }
}
