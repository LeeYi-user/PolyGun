using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeGun : MonoBehaviour
{
    public Gun realGun;

    public ParticleSystem muzzleFlash;

    private int currentAmmo;
    private float reloadTime;
    private bool isReloading;
    private float nextTimeToFire;

    [SerializeField] private Animator animator;

    // Update is called once per frame
    void Update()
    {
        currentAmmo = realGun.currentAmmo;
        reloadTime = realGun.reloadTime;
        isReloading = realGun.isReloading;
        nextTimeToFire = realGun.nextTimeToFire;

        if (isReloading)
        {
            return;
        }

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
        {
            Shoot();
        }
    }

    IEnumerator Reload()
    {
        animator.SetBool("isReloading", true);

        yield return new WaitForSeconds(reloadTime);

        animator.SetBool("isReloading", false);
    }

    void Shoot()
    {
        muzzleFlash.Play();
        animator.SetTrigger("isFiring");
    }
}
