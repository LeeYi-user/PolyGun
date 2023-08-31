using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRateLimit : MonoBehaviour
{
    public int fps = 60;

    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = fps;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.targetFrameRate != fps)
        {
            Application.targetFrameRate = fps;
        }
    }
}
