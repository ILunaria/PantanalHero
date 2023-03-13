using System.Collections.Generic;
using UnityEngine;
using static SoundSO;

public static class SoundManager
{
    public enum SoundType
    {
        PlayerMove,
        Dash,
        Jump,
        Attack,
        Defence,
        WallSlide,
        ButtonClick,
    }
    private static Dictionary<SoundType, float> soundTimerDictionary;
    private static GameObject oneShotGameObject;
    private static AudioSource oneShotAudioSource;

    public static float soundVolume = 0.3f;
    public static float musicVolume = 1f;

    public static void Initialize()
    {

        if (PlayerPrefs.HasKey("EffectVol"))
        {
            soundVolume = PlayerPrefs.GetFloat("EffectVol");
            Debug.Log(PlayerPrefs.GetFloat("EffectVol"));
        }
        if (PlayerPrefs.HasKey("MusicVol"))
        {
            musicVolume = PlayerPrefs.GetFloat("MusicVol");
            Debug.Log(PlayerPrefs.GetFloat("MusicVol"));
        }
        soundTimerDictionary = new Dictionary<SoundType, float>();
        soundTimerDictionary[SoundType.PlayerMove] = 0.5f;
        soundTimerDictionary[SoundType.Dash] = 0f;
        soundTimerDictionary[SoundType.Jump] = 0f;
        soundTimerDictionary[SoundType.Attack] = 0f;
        soundTimerDictionary[SoundType.Defence] = 0f;
        soundTimerDictionary[SoundType.WallSlide] = 0f;
        soundTimerDictionary[SoundType.ButtonClick] = 0f;
    }
    public static void PlaySound(SoundType sound)
    {
        if (CanPlaySound(sound))
        {
            if (oneShotGameObject == null)
            {
                oneShotGameObject = new GameObject("One Shot Sound");
                oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
                oneShotAudioSource.outputAudioMixerGroup = SoundSO.Mixer;
                oneShotAudioSource.volume = soundVolume;
            }
            oneShotAudioSource.PlayOneShot(GetAudioClip(sound));
        }
    }
    public static void PlaySound(SoundType sound, Vector3 position)
    {
        if (CanPlaySound(sound))
        {
            GameObject soundGO = new GameObject("Sound");
            soundGO.transform.position = position;
            AudioSource audio = soundGO.AddComponent<AudioSource>();
            audio.clip = GetAudioClip(sound);
            audio.volume = soundVolume;
            audio.maxDistance = 30f;
            audio.spatialBlend = 1f;
            audio.rolloffMode = AudioRolloffMode.Linear;
            audio.dopplerLevel = 0f;
            audio.outputAudioMixerGroup = SoundSO.Mixer;
            audio.Play();
            Object.Destroy(soundGO, audio.clip.length);
        }
    }
    private static bool CanPlaySound(SoundType sound) // Verifica se j? pode repetir o som
    {
        foreach (SoundAudioClip soundAudioClip in Sounds)
        {
            if (soundAudioClip.SoundType == sound)
            {
                float lastTimePlayed = soundTimerDictionary[sound];
                float maxTimer = soundAudioClip.TimeToRepeat;//tempo at? repetir
                if (lastTimePlayed + maxTimer < Time.unscaledTime)
                {
                    soundTimerDictionary[sound] = Time.unscaledTime;
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }
        return false;
    }
    private static AudioClip GetAudioClip(SoundType sound)
    {
        //SoundSO.SoundAudioClip soundAudioClip in SoundSO.soundArray
        foreach (SoundSO.SoundAudioClip soundAudioClip in SoundSO.Sounds)
        {
            if (soundAudioClip.SoundType == sound)
            {
                return soundAudioClip.Clip;
            }
        }
        Debug.LogError("Sound " + sound + " not found");
        return null;
    }
}
