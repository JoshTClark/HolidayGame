using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static ResourceManager;

public class ProjectileManager : MonoBehaviour
{
    public static Dictionary<ProjectileIndex, ObjectPool<ProjectileBase>> pools = new Dictionary<ProjectileIndex, ObjectPool<ProjectileBase>>();

    private static void CreateNewPool(ProjectileIndex index)
    {
        ObjectPool<ProjectileBase> pool = new ObjectPool<ProjectileBase>(createFunc: () => ResourceManager.GetProjectile(index), actionOnGet: (obj) => obj.Clean(GetPool(index)), actionOnRelease: (obj) => obj.gameObject.SetActive(false), actionOnDestroy: (obj) => Destroy(obj.gameObject), collectionCheck: false, defaultCapacity: 50);
        pools.Add(index, pool);
    }

    public static ObjectPool<ProjectileBase> GetPool(ProjectileIndex index)
    {
        ObjectPool<ProjectileBase> pool;
        pools.TryGetValue(index, out pool);
        return pool;
    }

    public static ProjectileBase GetProjectile(ProjectileIndex index)
    {
        ObjectPool<ProjectileBase> pool;
        if (pools.TryGetValue(index, out pool))
        {
            return pool.Get();
        }
        else
        {
            CreateNewPool(index);
            pools.TryGetValue(index, out pool);
            return pool.Get();
        }
    }
}
