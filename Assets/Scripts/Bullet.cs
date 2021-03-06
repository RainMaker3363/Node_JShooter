﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    [HideInInspector]
    public GameObject PlayerFrom;

    private void OnCollisionEnter(Collision collision)
    {
        var hit = collision.gameObject;
        var health = hit.GetComponent<Health>();
        
        if(health != null)
        {
            health.TakeDamege(PlayerFrom, 10);
        }

        Destroy(gameObject);   
    }
}
