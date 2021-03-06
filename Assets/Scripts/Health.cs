﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

    public const int maxHealth = 100;
    public bool destroyOnDeath;
    public int currentHealth = maxHealth;
    public bool IsEnemy = false;

    public RectTransform healthBar;

    private bool isLocalPlayer;

	// Use this for initialization
	void Start () {
        PlayerController pc = GetComponent<PlayerController>();
        isLocalPlayer = pc.isLocalPlayer;
	}
	
    public void TakeDamege(GameObject playerFrom, int amount)
    {
        currentHealth -= amount;

        // 네트워크 처리
        NetworkManager n = NetworkManager.Instance.GetComponent<NetworkManager>();
        n.CommandHealthChange(playerFrom, this.gameObject, amount, IsEnemy);
    }

    public void OnChangeHealth()
    {
        healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);

        if(currentHealth <= 0)
        {
            if(destroyOnDeath)
            {
                Destroy(gameObject);
            }
            else
            {
                currentHealth = maxHealth;
                healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
                Respawn();
            }
        }
    }

    void Respawn()
    {
        if(isLocalPlayer)
        {
            Vector3 spawnPoint = Vector3.zero;
            Quaternion spawnRotation = Quaternion.Euler(0, 100, 0);
            transform.position = spawnPoint;
            transform.rotation = spawnRotation;
        }
    }

	// Update is called once per frame
	void Update () {
        OnChangeHealth();

    }
}
