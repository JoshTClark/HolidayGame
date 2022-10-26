using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ResourceManager;

public class FollowingEffect : MonoBehaviour
{
    public GameObject following;
    public bool on = true;
    public FollowingEffectIndex index;

    void Update()
    {
        if (following && following.activeSelf && on)
        {
            this.gameObject.transform.position = following.transform.position;
            this.gameObject.transform.localScale = following.transform.localScale;
        }
        else
        {
            ParticleSystem[] systems = this.GetComponents<ParticleSystem>();
            bool done = true;
            foreach (ParticleSystem s in systems)
            {
                if (s.particleCount > 0)
                {
                    done = false;
                    s.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
            }
            if (done)
            {
                on = false;
                Destroy(this.gameObject);
            }
        }
    }

    public void Activate(bool isActive)
    {
        //Debug.Log("Activating effect");
        on = isActive;
        ParticleSystem[] systems = this.GetComponents<ParticleSystem>();
        foreach (ParticleSystem s in systems)
        {
            s.Play();
        }
    }
}
