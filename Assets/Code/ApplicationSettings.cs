using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ApplicationSettings {
    
    public static float masterVolume = 1f;
    public static float soundVolume = 1f;
    public static float musicVolume = 1f;

    public static void ChangeVolumeSettings(float master, float sound, float music) {
        masterVolume = Mathf.Clamp(master, 0f, 1f);
        soundVolume = Mathf.Clamp(sound, 0f, 1f);
        musicVolume = Mathf.Clamp(music, 0f, 1f);
    }

    public static float SoundVolume() {
        float vol = masterVolume * soundVolume;
        return vol;
    }

    public static float MusicVolume() {
        float vol = masterVolume * musicVolume;
        return vol;
    }
}
