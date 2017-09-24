using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    public GameObject enemy;
    public GameObject spawnPoint;
    public int numberofEnemies;

    [HideInInspector]
    public List<SpawnPoint> enemySpawnPoints;

	// Use this for initialization
	void Start () {
		// 랜덤 리스폰 포인트를 지정
        for(int i =0; i<numberofEnemies; i++)
        {
            var spawnPosition = new Vector3(Random.Range(-8f, 8f), 0f, Random.Range(-8f, 8f));
            var spawnRotation = Quaternion.Euler(0.0f, Random.Range(0, 180f), 0f);

            SpawnPoint enemySpawnPoint = (Instantiate(spawnPoint, spawnPosition, spawnRotation) as GameObject).GetComponent<SpawnPoint>();

            enemySpawnPoints.Add(enemySpawnPoint);
        }

        //SpawnEnemies();

    }

    // 네트워크 처리 필요
    public void SpawnEnemies(NetworkManager.EnemyJSON enemiesJSON)
    {
        //int i = 0;

        //foreach(SpawnPoint sp in enemySpawnPoints)
        foreach (NetworkManager.UserJSON enemyJSON in enemiesJSON.enemies)
        {
            if(enemyJSON.health <= 0)
            {
                continue;
            }

            Vector3 position = new Vector3(enemyJSON.position[0], enemyJSON.position[1], enemyJSON.position[2]);//sp.transform.position;
            Quaternion rotation = Quaternion.Euler(enemyJSON.rotation[0], enemyJSON.rotation[1], enemyJSON.rotation[2]);//sp.transform.rotation;

            GameObject newEnemy = Instantiate(enemy, position, rotation) as GameObject;

            // 네트워크에서 플레이어의 이름을 받아서 넣어준다.
            newEnemy.name = enemyJSON.name;//i + " Enemy";

            PlayerController pc = newEnemy.GetComponent<PlayerController>();
            pc.isLocalPlayer = false;

            Health h = newEnemy.GetComponent<Health>();
            h.currentHealth = enemyJSON.health;//100;
            h.OnChangeHealth();
            h.destroyOnDeath = true;
            h.IsEnemy = true;

            //i++;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
