using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public CharacterController controller;

    public float speed = 12f;
    public float gravity = -19.62f;
    public float jumpHeight = 2f;

    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;

    [SerializeField] private Animator animator;

    public GameObject mainCamera;
    public GameObject weaponCamera;
    public GameObject realGun;
    public SkinnedMeshRenderer mainBody;
    public SkinnedMeshRenderer fakeGun;

    bool live = true;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner)
        {
            gameObject.GetComponent<PlayerMovement>().enabled = false;
            gameObject.GetComponent<Gun>().enabled = false;
            gameObject.GetComponent<WeaponSway>().enabled = false;
            gameObject.GetComponent<MouseLook>().enabled = false;
            mainCamera.GetComponent<Camera>().enabled = false;
            mainCamera.GetComponent<AudioListener>().enabled = false;
            weaponCamera.SetActive(false);
            realGun.SetActive(false);
            return;
        }

        gameObject.layer = LayerMask.NameToLayer("Default");
        mainBody.enabled = false;
        fakeGun.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!live)
        {
            return;
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        animator.SetBool("isRunning", x != 0 || z != 0);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        if (gameObject.transform.position.y < -10f)
        {
            gameObject.GetComponent<HealthManager>().TakeDamage(100f);
        }
    }

    public void Despawn()
    {
        live = false;
        velocity = Vector3.zero;
        realGun.SetActive(false);
    }

    public void Respawn()
    {
        live = true;
        gameObject.transform.position = new Vector3(0f, 0f, 0f);
        gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        realGun.SetActive(true);
    }
}
