using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

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
    public void OnDefeat()
    {
        StartCoroutine("GameWin");
    }
    IEnumerator GameWin()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(0);
    }
}
