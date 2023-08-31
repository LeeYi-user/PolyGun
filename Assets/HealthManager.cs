using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthManager : NetworkBehaviour
{
    public Image healthBar;
    public float maxHealth = 100f;
    public float currentHealth;
    public float respawnTime = 2f;

    private SkinnedMeshRenderer mainBody;
    private Color originalColor;
    private float flashTime = 0.15f;

    private bool live = true;

    // Start is called before the first frame update
    void Start()
    {
        if (IsOwner)
        {
            healthBar = GameObject.Find("Health").GetComponent<Image>();
        }

        currentHealth = maxHealth;
        mainBody = gameObject.GetComponent<PlayerMovement>().mainBody;
        originalColor = mainBody.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (live && currentHealth <= 0)
        {
            live = false;

            GameObject.Find("Crosshair").GetComponent<Image>().enabled = false;
            GameObject.Find("Death Screen").GetComponent<Image>().enabled = true;
            GameObject.Find("Death Message").GetComponent<TMP_Text>().enabled = true;
            gameObject.GetComponent<PlayerMovement>().Despawn();
            gameObject.GetComponent<WeaponSway>().Despawn();
            gameObject.GetComponent<MouseLook>().Despawn();
            gameObject.GetComponent<Gun>().Despawn();

            PlayerDespawn_ServerRpc(NetworkObjectId, NetworkManager.Singleton.LocalClientId);
            StartCoroutine(Respawn());
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (healthBar)
        {
            healthBar.fillAmount = currentHealth / maxHealth;
        }

        StartCoroutine(DamageFlash());
    }

    IEnumerator DamageFlash()
    {
        mainBody.material.color = Color.red;
        yield return new WaitForSeconds(flashTime);
        mainBody.material.color = originalColor;
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);

        live = true;

        GameObject.Find("Crosshair").GetComponent<Image>().enabled = true;
        GameObject.Find("Death Screen").GetComponent<Image>().enabled = false;
        GameObject.Find("Death Message").GetComponent<TMP_Text>().enabled = false;
        gameObject.GetComponent<PlayerMovement>().Respawn();
        gameObject.GetComponent<WeaponSway>().Respawn();
        gameObject.GetComponent<MouseLook>().Respawn();
        gameObject.GetComponent<Gun>().Respawn();

        currentHealth = maxHealth;

        if (healthBar)
        {
            healthBar.fillAmount = currentHealth / maxHealth;
        }

        PlayerRespawn_ServerRpc(NetworkObjectId, NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    void PlayerDespawn_ServerRpc(ulong objectId, ulong playerId)
    {
        PlayerDespawn_ClientRpc(objectId, playerId);
    }

    [ClientRpc]
    void PlayerDespawn_ClientRpc(ulong objectId, ulong playerId)
    {
        GameObject playerGO = NetworkManager.SpawnManager.SpawnedObjects[objectId].gameObject;

        if (playerId != NetworkManager.Singleton.LocalClientId)
        {
            playerGO.GetComponent<PlayerMovement>().mainBody.enabled = false;
            playerGO.GetComponent<PlayerMovement>().fakeGun.enabled = false;
            playerGO.layer = LayerMask.NameToLayer("Default");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void PlayerRespawn_ServerRpc(ulong objectId, ulong playerId)
    {
        PlayerRespawn_ClientRpc(objectId, playerId);
    }

    [ClientRpc]
    void PlayerRespawn_ClientRpc(ulong objectId, ulong playerId)
    {
        GameObject playerGO = NetworkManager.SpawnManager.SpawnedObjects[objectId].gameObject;

        if (playerId != NetworkManager.Singleton.LocalClientId)
        {
            playerGO.GetComponent<PlayerMovement>().mainBody.enabled = true;
            playerGO.GetComponent<PlayerMovement>().fakeGun.enabled = true;
            playerGO.layer = LayerMask.NameToLayer("Hittable");
        }
    }
}
