using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

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
    //[SerializeField]
    //AudioSource arrowEffect;



    // Start is called before the first frame update
    void Start()
    {
        // Nothing Special
    }

    // Update is called once per frame
    void Update()
    {
        // Nothing Special
    }

    public void PumpkinExplosion()
    {
        pumpkinExplosionEffect.Play();
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
        //arrowEffect.Play();
    }
    public void LevelUp()
    {
        levelUpEffect.Play();
    }
}
