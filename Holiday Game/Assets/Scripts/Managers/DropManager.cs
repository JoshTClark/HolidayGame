using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static ResourceManager;

public class DropManager : MonoBehaviour
{
    public static Dictionary<PickupIndex, ObjectPool<DropBase>> pools = new Dictionary<PickupIndex, ObjectPool<DropBase>>();
    public static List<DropBase> allDrops = new List<DropBase>();

    private static void CreateNewPool(PickupIndex index)
    {
        ObjectPool<DropBase> pool = new ObjectPool<DropBase>(createFunc: () => ResourceManager.GetPickup(index), actionOnGet: (obj) => obj.Clean(GetPool(index)), actionOnRelease: (obj) => obj.gameObject.SetActive(false), actionOnDestroy: (obj) => Destroy(obj.gameObject), collectionCheck: false, defaultCapacity: 50);
        pools.Add(index, pool);
    }

    public static ObjectPool<DropBase> GetPool(PickupIndex index)
    {
        ObjectPool<DropBase> pool;
        pools.TryGetValue(index, out pool);
        return pool;
    }

    public static DropBase GetPickup(PickupIndex index)
    {
        ObjectPool<DropBase> pool;
        if (pools.TryGetValue(index, out pool))
        {
            DropBase p = pool.Get();
            if (!allDrops.Contains(p))
            {
                allDrops.Add(p);
            }
            return p;
        }
        else
        {
            CreateNewPool(index);
            pools.TryGetValue(index, out pool);
            DropBase p = pool.Get();
            if (!allDrops.Contains(p))
            {
                allDrops.Add(p);
            }
            return p;
        }
    }

    public static void Clean()
    {
        foreach (DropBase p in allDrops)
        {
            p.pool.Release(p);
        }
        pools.Clear();
        allDrops.Clear();
    }
}
