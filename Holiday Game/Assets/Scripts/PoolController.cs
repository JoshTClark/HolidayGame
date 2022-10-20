using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolController : MonoBehaviour
{
    ObjectPool<GameObject> snowballPool = new ObjectPool<GameObject>(createFunc: () => new GameObject("PooledObject"), actionOnGet: (obj) => obj.SetActive(true), actionOnRelease: (obj) => obj.SetActive(false), actionOnDestroy: (obj) => Destroy(obj), collectionCheck: false, defaultCapacity: 10);
}
