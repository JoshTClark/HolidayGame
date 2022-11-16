using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField]
    AudioSource levelUpEffect;

    [SerializeField]
    AudioSource pumpkinExplosionEffect;
    
    //[SerializeField]
    //AudioSource rocketExplosionEffect;
    //
    //[SerializeField]
    //AudioSource snowballEffect;
    //
    [SerializeField]
    AudioSource arrowEffect;

    [SerializeField]
    AudioSource buttonPress;



    // Start is called before the first frame update
    void Start()
    {
        // Nothing Special
        instance = this; 
    }

    // Update is called once per frame
    void Update()
    {
        // Nothing Special
    }

    public void PumpkinExplosion()
    {
        if (!pumpkinExplosionEffect.isPlaying)
        {
            pumpkinExplosionEffect.Play();
        }
    }
    public void RocketExplosion()
    {
        //rocketExplosionEffect.Play();
    }
    public void SnowballHit()
    {
        //snowballEffect.Play();
    }
    public void ArrowHit()
    {
        if (!arrowEffect.isPlaying)
        {
            arrowEffect.Play();
        }
    }
    public void LevelUp()
    {
        if (!levelUpEffect.isPlaying)
        {
            levelUpEffect.Play();
        }
    }
    public void ButtonPress()
    {
        if (!buttonPress.isPlaying) { buttonPress.Play(); }
    }
}
