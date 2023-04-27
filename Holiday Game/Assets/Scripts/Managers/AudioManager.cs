using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

public class AudioManager : MonoBehaviour
{
    // Singlet
    public static AudioManager instance;

    // Dictionary of audio clips
    private static Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();

    // Mixers
    [SerializeField]
    private AudioMixerGroup master, xpMixer, enemyHurtMixer;

    // Main pool for audio sources
    private static ObjectPool<AudioSource> pool = new ObjectPool<AudioSource>(createFunc: () => CreateSource(), actionOnGet: (obj) => obj.gameObject.SetActive(true), actionOnRelease: (obj) => obj.gameObject.SetActive(false), actionOnDestroy: (obj) => Destroy(obj.gameObject), collectionCheck: true, defaultCapacity: 10);
    private static List<AudioSource> allSources = new List<AudioSource>();

    public static float globalVolume = 0.3f;

    [SerializeField] private float HelperVolume = globalVolume;

    /// <summary>
    /// Called when the scene starts
    /// </summary>
    private void Awake()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        allSources.Clear();
        pool.Clear();
        instance = null;
    }

    void Update()
    {
        HelperVolume= globalVolume;
        // Checking which sources are finished playing and releasing them to the pool
        foreach (AudioSource i in allSources)
        {
            if (i.gameObject.activeSelf && !i.isPlaying)
            {
                pool.Release(i);
            }
        }
    }

    /// <summary>
    /// Called when the pool needs a new audio source
    /// Creates a gameobject, attaches it to the instance, and adds an AudioSource to it.
    /// </summary>
    /// <returns></returns>
    private static AudioSource CreateSource()
    {
        GameObject obj = new GameObject("PooledSource");
        AudioSource source = obj.AddComponent<AudioSource>();
        obj.transform.parent = instance.transform;
        allSources.Add(source);
        return source;
    }

    public static void SetAudioClips(AudioClip[] audio)
    {
        foreach (AudioClip c in audio)
        {
            clips.Add(c.name, c);
        }
    }

    /// <summary>
    /// Plays the given audio source with the given volume and pitch
    /// </summary>
    /// <param name="audio"></param>
    /// <param name="volume">Optional volume - Default is 1f</param>
    /// <param name="pitch">Optional pitch - Default is 1f</param>
    public void PlaySound(AudioClip audio, float volume = 1f, float pitch = 1f)
    {
        AudioSource source = pool.Get();
        source.volume = volume * globalVolume;
        source.outputAudioMixerGroup = master;
        source.pitch = pitch;
        source.clip = audio;
        source.Play();
    }

    /// <summary>
    /// Finds the audio source based on its name and plays it with the given volume and pitch
    /// </summary>
    /// <param name="audio"></param>
    /// <param name="volume">Optional volume - Default is 1f</param>
    /// <param name="pitch">Optional pitch - Default is 1f</param>
    public void PlaySound(string soundName, float volume = 1f, float pitch = 1f)
    {
        AudioClip clip;
        clips.TryGetValue(soundName, out clip);
        if (clip == null)
        {
            Debug.LogError("Cannot find audio clip with name " + soundName);
            return;
        }
        AudioSource source = pool.Get();
        source.volume = volume * globalVolume;
        source.pitch = pitch;
        source.clip = clip;

        if (soundName == "XP")
        {
            source.outputAudioMixerGroup = xpMixer;
        }
        else if (soundName == "EnemyHurt")
        {
            source.outputAudioMixerGroup = enemyHurtMixer;
        }
        else
        {
            source.outputAudioMixerGroup = master;
        }
        source.Play();
    }

    public void PlayMenuButton() 
    {
        PlaySound("MenuButton");
    }
}
