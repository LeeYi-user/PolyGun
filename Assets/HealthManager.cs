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

    // Start is called before the first frame update
    void Start()
    {
        if (IsOwner)
        {
            healthBar = GameObject.Find("Health").GetComponent<Image>();
        }

        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (healthBar)
        {
            healthBar.fillAmount = currentHealth / maxHealth;
        }
    }
}
