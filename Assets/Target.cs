using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public Camera fpsCam;

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = Quaternion.Euler(fpsCam.transform.localEulerAngles.x, 0f, 0f);
    }
}
