using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


[CreateAssetMenu(menuName = "ScriptableObjects/Sound", fileName = "Sound-")]
public class SoundSO : ScriptableObject
{
    [SerializeField] private AudioMixerGroup mixer;
    public static AudioMixerGroup Mixer;
    [SerializeField] private List<SoundAudioClip> sounds = new List<SoundAudioClip>();
    public static List<SoundAudioClip> Sounds = new List<SoundAudioClip>();

    private void Awake()
    {
        SetSounds();
    }
    private void OnValidate()
    {
        Debug.Log("Validate");
        SetSounds();
    }
    private void OnEnable()
    {
        Debug.Log("Enable");
        SetSounds();
    }
    private void OnDisable()
    {
        Debug.Log("Disable");
    }
    private void SetSounds()
    {
        Sounds = sounds;
        Mixer = mixer;
    }

    [Serializable]
    public class SoundAudioClip
    {
        [SerializeField] private SoundManager.SoundType soundType;
        [SerializeField] private AudioClip clip;
        [SerializeField] private float timeToRepeat = 0.1f;

        public SoundManager.SoundType SoundType => soundType;
        public AudioClip Clip => clip;
        public float TimeToRepeat => timeToRepeat;
    }
}
