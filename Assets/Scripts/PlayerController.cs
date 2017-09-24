using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public bool isLocalPlayer = false;

    // 네트워크에서 보간시킬 변수들
    Vector3 oldPosition;
    Vector3 currentPosition;
    Quaternion oldRotation;
    Quaternion currentRotation;


	// Use this for initialization
	void Start () {
        oldPosition = transform.position;
        currentPosition = oldPosition;

        oldRotation = transform.rotation;
        currentRotation = oldRotation;
	}

	
	// Update is called once per frame
	void Update () {

		if(!isLocalPlayer)
        {
            return;
        }

        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);

        currentPosition = transform.position;
        currentRotation = transform.rotation;

        if(currentPosition != oldPosition)
        {
            // 네트워크 처리
            NetworkManager.Instance.GetComponent<NetworkManager>().CommandMove(transform.position);

            oldPosition = currentPosition;
        }

        if(currentRotation != oldRotation)
        {
            // 네트워크 처리
            NetworkManager.Instance.GetComponent<NetworkManager>().CommandRotate(transform.rotation);

            oldRotation = currentRotation;
        }

        // 발사 버튼 처리
        if(Input.GetKeyDown(KeyCode.Space))
        {
            // 네트워크 처리
            NetworkManager n = NetworkManager.Instance.GetComponent<NetworkManager>();
            n.CommandShoot();

            //CommandFire();
        }
	}

    public void CommandFire()
    {
        var bullet = Instantiate(bulletPrefab, 
            bulletSpawn.position, 
            bulletSpawn.rotation) as GameObject;

        Bullet b = bullet.GetComponent<Bullet>();
        b.PlayerFrom = this.gameObject;
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.up * 6.0f;

        Destroy(bullet, 3.0f);
    }
}
