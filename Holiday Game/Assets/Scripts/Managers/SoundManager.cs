using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    private void Start()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        instance = null;
    }

    void Update()
    {
    }
}
