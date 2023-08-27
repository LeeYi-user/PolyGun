using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 2f;

    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    public int maxAmmo = 7;
    private int currentAmmo;
    public float reloadTime = 1.35f;
    private bool isReloading = false;
    private float nextTimeToFire = 0f;

    [SerializeField] private Animator animator;

    [SerializeField] private Transform BulletSpawnPoint;
    [SerializeField] private TrailRenderer BulletTrail;
    [SerializeField] private float BulletSpeed = 100;

    public Animator fakeAnimator;
    public ParticleSystem fakeMuzzleFlash;
    public Transform fakeBulletSpawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        currentAmmo = maxAmmo;
    }

    // Update is called once per frame
    void Update()
    {
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
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;

        animator.SetBool("isReloading", true);
        fakeAnimator.SetBool("isReloading", true);

        yield return new WaitForSeconds(reloadTime);

        animator.SetBool("isReloading", false);
        fakeAnimator.SetBool("isReloading", false);

        currentAmmo = maxAmmo;
        isReloading = false;
    }

    void Shoot()
    {
        muzzleFlash.Play();
        fakeMuzzleFlash.Play();
        animator.SetTrigger("isFiring");
        fakeAnimator.SetTrigger("isFiring");

        currentAmmo--;

        RaycastHit hit;

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            TrailRenderer trail = Instantiate(BulletTrail, BulletSpawnPoint.position, Quaternion.identity);
            TrailRenderer fakeTrail = Instantiate(BulletTrail, fakeBulletSpawnPoint.position, Quaternion.identity);

            StartCoroutine(SpawnTrail(trail, hit.point, hit.normal, true));
            StartCoroutine(SpawnTrail(fakeTrail, hit.point, hit.normal, true));
        }
        else
        {
            TrailRenderer trail = Instantiate(BulletTrail, BulletSpawnPoint.position, Quaternion.identity);
            TrailRenderer fakeTrail = Instantiate(BulletTrail, fakeBulletSpawnPoint.position, Quaternion.identity);

            StartCoroutine(SpawnTrail(trail, BulletSpawnPoint.position + fpsCam.transform.forward * range, Vector3.zero, false));
            StartCoroutine(SpawnTrail(fakeTrail, fakeBulletSpawnPoint.position + fpsCam.transform.forward * range, Vector3.zero, false));
        }
    }

    private IEnumerator SpawnTrail(TrailRenderer Trail, Vector3 HitPoint, Vector3 HitNormal, bool MadeImpact)
    {
        Vector3 startPosition = Trail.transform.position;
        float distance = Vector3.Distance(Trail.transform.position, HitPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            Trail.transform.position = Vector3.Lerp(startPosition, HitPoint, 1 - (remainingDistance / distance));

            remainingDistance -= BulletSpeed * Time.deltaTime;

            yield return null;
        }

        Trail.transform.position = HitPoint;

        if (MadeImpact)
        {
            GameObject impactGO = Instantiate(impactEffect, HitPoint, Quaternion.LookRotation(HitNormal));
            Destroy(impactGO, 2f);
        }

        Destroy(Trail.gameObject, Trail.time);
    }
}
