using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public Camera fpsCam;

    float xRotation = 0f;

    // Update is called once per frame
    void Update()
    {
        xRotation = fpsCam.transform.localEulerAngles.x;

        if (xRotation >= 70 && xRotation <= 90)
        {
            xRotation = 70;
        }

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
