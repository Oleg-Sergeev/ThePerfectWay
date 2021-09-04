using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public AudioSource[] Audio;
    private static AudioManager Instance;
    private static Dictionary<string, AudioSource> DictAudio;


    private void Awake()
    {
        #region SingleTon
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        #endregion

        DictAudio = new Dictionary<string, AudioSource>();

        for (int i = 0; i < Audio.Length; i++) DictAudio.Add(Audio[i].name, Audio[i]);
    }

    public static void PlayAudio(string name)
    {
        if (DictAudio.ContainsKey(name)) DictAudio[name].Play();
        else throw new KeyNotFoundException("Audio not found. Audio name: " + name);
    }

    public static void ChangeAudioVolume(string name, float value)
    {
        if (DictAudio.ContainsKey(name)) DictAudio[name].volume = value;
        else throw new KeyNotFoundException("Audio not found. Audio name: " + name);
    }

    public static void ChangeVolumeAll(string audio, float value)
    {
        for (int i = 0; i < DictAudio.Count; i++)
        {
            if(DictAudio[Instance.Audio[i].name].tag == audio) DictAudio[Instance.Audio[i].name].volume = value;
        }
    }

    public static AudioSource GetAudio(string name)
    {
        if (DictAudio.ContainsKey(name)) return DictAudio[name];
        else throw new KeyNotFoundException("Audio not found. Audio name: " + name);
    }

    public static void PauseAudio(string name)
    {
        if (DictAudio.ContainsKey(name)) DictAudio[name].Pause();
        else throw new KeyNotFoundException("Audio not found. Audio name: " + name);
    }

    public static void StopAudio(string name)
    {
        if (DictAudio.ContainsKey(name)) DictAudio[name].Stop();
        else throw new KeyNotFoundException("Audio not found. Audio name: " + name);
    }

    public static void StopAllAudio()
    {
        for (int i = 0; i < DictAudio.Count; i++) DictAudio[Instance.Audio[i].name].Stop();
    }
}
