using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRateLimit : MonoBehaviour
{
    void Awake()
    {
        QualitySettings.vSyncCount = 1;
    }
}
