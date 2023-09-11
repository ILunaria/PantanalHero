using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Vol_checker : MonoBehaviour
{
    public AudioMixer _mixer;

    void Start(){
        _mixer.SetFloat("Music", 1f);
    }
    public void setLevel(float _slideVal)
    {
        _mixer.SetFloat("Music", Mathf.Log10(_slideVal) * 20);
    }
}
