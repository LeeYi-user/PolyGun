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

    public Camera mainCamera;
    public GameObject weaponCamera;
    public GameObject realGun;
    public SkinnedMeshRenderer mainBody;
    public SkinnedMeshRenderer fakeGun;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner)
        {
            gameObject.GetComponent<PlayerMovement>().enabled = false;
            gameObject.GetComponent<Gun>().enabled = false;
            mainCamera.enabled = false;
            weaponCamera.SetActive(false);
            realGun.SetActive(false);
            return;
        }

        mainBody.enabled = false;
        fakeGun.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
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

        if (Input.GetButton("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}
