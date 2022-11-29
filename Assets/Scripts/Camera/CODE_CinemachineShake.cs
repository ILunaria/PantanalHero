using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CODE_CinemachineShake : MonoBehaviour
{
    public float shakeLength, shakePower;

    public float shakeTimeRemaining, power, shakeFadeTime;

    private CinemachineVirtualCamera cinemachineVirtualCamera;
    private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;

    public bool shakeStarted;

    private void Start()
    {
        CODE_EventManager.current.OnScreenShakeCallback += ShakeCamera;
    }

    // Start is called before the first frame update
    private void Awake()
    { 
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Update()
    {
        if (shakeStarted)
        {
            if (shakeTimeRemaining > 0)
            {
                shakeTimeRemaining -= Time.deltaTime;

                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = power;
                power = Mathf.MoveTowards(power, 0f, shakeFadeTime * Time.deltaTime);
            }
            else
            {
                shakeStarted = false;
            }
        }

        if(shakeTimeRemaining < 0)
        {
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
        }
    }

   
    private void ShakeCamera()
    {
        shakeStarted = true;
        shakeTimeRemaining = shakeLength;
        power = shakePower;

        shakeFadeTime = shakePower / shakeLength;
    }
}
