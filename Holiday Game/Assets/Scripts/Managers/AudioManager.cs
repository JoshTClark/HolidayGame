using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Pool;

public class AudioManager : MonoBehaviour
{
    // Singlet
    public static AudioManager instance;

    // Main pool for audio sources
    private static ObjectPool<AudioSource> pool = new ObjectPool<AudioSource>(createFunc: () => CreateSource(), actionOnGet: (obj) => obj.gameObject.SetActive(true), actionOnRelease: (obj) => obj.gameObject.SetActive(false), actionOnDestroy: (obj) => Destroy(obj.gameObject), collectionCheck: true, defaultCapacity: 10);
    private static List<AudioSource> allSources = new List<AudioSource>();

    [SerializeField] private AudioClip menuSound;
    [SerializeField] private AudioClip titleMusic;
    [SerializeField] private AudioClip mapMusic;

    public static float globalVolume = 0f;

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
        obj.AddComponent<AudioSource>();
        obj.transform.parent = instance.transform;
        allSources.Add(obj.GetComponent<AudioSource>());
        return obj.GetComponent<AudioSource>();
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
        source.pitch = pitch;
        source.clip = audio;
        source.Play();
    }

    public void ButtonPress()
    {
        PlaySound(menuSound);
    }
}
