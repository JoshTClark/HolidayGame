using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingEffect : MonoBehaviour
{
    public GameObject following;

    void Update()
    {
        if (following)
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
                if (s)
                {
                    done = false;
                    s.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    if (s.particleCount == 0)
                    {
                        Destroy(s);
                    }
                }
            }
            if (done)
            {
                Destroy(this);
            }
        }
    }
}
