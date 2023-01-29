using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class TrailEffect : MonoBehaviour
{
    private List<DashParticle> dashParticles = new List<DashParticle>();
    private ObjectPool<DashParticle> dashParticlePool = new ObjectPool<DashParticle>(createFunc: () => Instantiate<DashParticle>(ResourceManager.playerDashEffect), actionOnGet: (obj) => obj.gameObject.SetActive(true), actionOnRelease: (obj) => obj.gameObject.SetActive(false), actionOnDestroy: (obj) => Destroy(obj.gameObject), collectionCheck: false, defaultCapacity: 10);
    private float particleTimer = 0.0f;
    public float particleDelay = 0.02f;
    public bool createParticles = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (createParticles)
        {
            particleTimer += Time.deltaTime;
            if (particleTimer >= particleDelay)
            {
                particleTimer = 0.0f;
                DashParticle p = dashParticlePool.Get();
                p.SetSprite(this.gameObject.GetComponent<SpriteRenderer>().sprite, GetComponent<SpriteRenderer>().flipX);
                p.transform.position = transform.position;
                p.transform.rotation = transform.rotation;
                p.transform.localScale = transform.lossyScale;
                dashParticles.Add(p);
            }
        }

        for (int i = dashParticles.Count - 1; i >= 0; i--)
        {
            DashParticle p = dashParticles[i];
            if (p.finished)
            {
                dashParticlePool.Release(p);
                dashParticles.RemoveAt(i);
            }
        }
    }
}
