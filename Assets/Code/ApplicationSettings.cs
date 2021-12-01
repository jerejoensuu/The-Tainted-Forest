using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class ApplicationSettings {
    
    public static float defaultMasterVolume = 1f;
    public static float defaultSoundVolume = 1f;
    public static float defaultMusicVolume = 1f;

    public static void ChangeVolumeSettings(float master, float sound, float music) {
        PlayerPrefs.SetFloat("MasterVolume", Mathf.Clamp(master, 0f, 1f));
        PlayerPrefs.SetFloat("SoundVolume", Mathf.Clamp(sound, 0f, 1f));
        PlayerPrefs.SetFloat("MusicVolume", Mathf.Clamp(music, 0f, 1f));
    }

    public static float GetMasterVolume() {
        return PlayerPrefs.GetFloat("MasterVolume", defaultMasterVolume);
    }

    public static float GetSoundVolume() {
        return PlayerPrefs.GetFloat("SoundVolume", defaultSoundVolume);
    }

    public static float GetMusicVolume() {
        return PlayerPrefs.GetFloat("MusicVolume", defaultMusicVolume);
    }

    public static float SoundVolume() { // Calculate volume to be used for sound effects
        float vol = GetMasterVolume() * GetSoundVolume();
        return vol;
    }

    public static float MusicVolume() { // Calculate volume to be used for music
        float vol = GetMasterVolume() * GetMusicVolume();
        return vol;
    }
}
