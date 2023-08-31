using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MouseLook : NetworkBehaviour
{
    public float mouseSensitivity = 200f;

    public Transform mainCamera;

    float xRotation = 0f;

    bool live = true;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner)
        {
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (Cursor.lockState == CursorLockMode.None || !live)
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        mainCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    public void Despawn()
    {
        live = false;
    }

    public void Respawn()
    {
        live = true;
    }
}
