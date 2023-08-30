using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : NetworkBehaviour
{
    public Image healthBar;
    public float maxHealth = 100f;
    public float currentHealth;

    private SkinnedMeshRenderer mainBody;
    private Color originalColor;
    private float flashTime = 0.15f;

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
}
