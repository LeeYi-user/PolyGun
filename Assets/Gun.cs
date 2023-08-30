using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Gun : NetworkBehaviour
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

        fakeMuzzleFlash.gameObject.SetActive(false);
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
        PlayFakeMuzzleFlash_ServerRpc(NetworkObjectId);
        animator.SetTrigger("isFiring");
        fakeAnimator.SetTrigger("isFiring");

        currentAmmo--;

        RaycastHit hit;

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range, LayerMask.GetMask("Hittable")))
        {
            CreateBulletTrail_ServerRpc(NetworkManager.Singleton.LocalClientId, BulletSpawnPoint.position, true, hit.point, hit.normal, true, fpsCam.transform.forward);
            CreateBulletTrail_ServerRpc(NetworkManager.Singleton.LocalClientId, fakeBulletSpawnPoint.position, false, hit.point, hit.normal, true, fpsCam.transform.forward);
        }
        else
        {
            CreateBulletTrail_ServerRpc(NetworkManager.Singleton.LocalClientId, BulletSpawnPoint.position, true, hit.point, hit.normal, false, fpsCam.transform.forward);
            CreateBulletTrail_ServerRpc(NetworkManager.Singleton.LocalClientId, fakeBulletSpawnPoint.position, false, hit.point, hit.normal, false, fpsCam.transform.forward);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayFakeMuzzleFlash_ServerRpc(ulong objectId)
    {
        PlayFakeMuzzleFlash_ClientRpc(objectId);
    }

    [ClientRpc]
    private void PlayFakeMuzzleFlash_ClientRpc(ulong objectId)
    {
        NetworkManager.SpawnManager.SpawnedObjects[objectId].gameObject.GetComponent<Gun>().fakeMuzzleFlash.Play();
    }

    [ServerRpc(RequireOwnership = false)]
    private void CreateBulletTrail_ServerRpc(ulong playerId, Vector3 position, bool real, Vector3 HitPoint, Vector3 HitNormal, bool MadeImpact, Vector3 forward)
    {
        TrailRenderer trail = Instantiate(BulletTrail, position, Quaternion.identity);

        trail.GetComponent<NetworkObject>().Spawn(true);

        if (MadeImpact)
        {
            StartCoroutine(SpawnTrail(trail, HitPoint, HitNormal, MadeImpact, real));
        }
        else
        {
            StartCoroutine(SpawnTrail(trail, position + forward * range, Vector3.zero, false, real));
        }

        CreateBulletTrail_ClientRpc(trail.GetComponent<NetworkObject>().NetworkObjectId, playerId, real);
    }

    [ClientRpc]
    private void CreateBulletTrail_ClientRpc(ulong objectId, ulong ownerId, bool real)
    {
        GameObject trailGO = NetworkManager.SpawnManager.SpawnedObjects[objectId].gameObject;

        if ((ownerId == NetworkManager.Singleton.LocalClientId && !real) || (ownerId != NetworkManager.Singleton.LocalClientId && real))
        {
            trailGO.SetActive(false);
        }
    }

    private IEnumerator SpawnTrail(TrailRenderer Trail, Vector3 HitPoint, Vector3 HitNormal, bool MadeImpact, bool real)
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

        if (MadeImpact && real)
        {
            GameObject impactGO = Instantiate(impactEffect, HitPoint, Quaternion.LookRotation(HitNormal));
            impactGO.GetComponent<NetworkObject>().Spawn(true);
            Destroy(impactGO, 2f);
        }

        Destroy(Trail.gameObject, Trail.time);
    }
}
