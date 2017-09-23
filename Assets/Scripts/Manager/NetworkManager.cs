using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using SocketIO;

public class NetworkManager : MonoBehaviour {

    public static NetworkManager Instance;
    public Canvas canvas;
    public SocketIOComponent socket;

    public InputField playerNameInput;
    public GameObject player;


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start () {
        socket.On("enemies", OnEnemies);
        socket.On("other player connected", OnOtherPlayerConnected);
        socket.On("play", OnPlay);
        socket.On("player move", OnPlayerMove);
        socket.On("player turn", OnPlayerTurn);
        socket.On("player shoot", OnPlayerShoot);
        socket.On("health", OnHealth);
        socket.On("other player disconnected", OnOtherPlayerDisConnect);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // 게임 참가 처리
    public void JoinGame()
    {
        StartCoroutine(ConnectToServer());
    }

    #region Commands

    

    IEnumerator ConnectToServer()
    {

        yield return new WaitForSeconds(0.5f);

        socket.Emit("player connect");

        yield return new WaitForSeconds(1.0f);

        string playerName = playerNameInput.text;
        Debug.Log(playerName);

        List<SpawnPoint> playerSpawnPoints = GetComponent<PlayerSpawner>().playerSpawnPoints;
        List<SpawnPoint> EnemySpawnPoints = GetComponent<EnemySpawner>().enemySpawnPoints;

        PlayerJSON playerJSON = new PlayerJSON(playerName, playerSpawnPoints, EnemySpawnPoints);

        string data = JsonUtility.ToJson(playerJSON);

        socket.Emit("play", new JSONObject(data));

        canvas.gameObject.SetActive(false);
    }

    #endregion

    #region Listening

    void OnEnemies(SocketIOEvent socketIOEvent)
    {
        EnemyJSON enemiesJSON = EnemyJSON.CreateFromJSON(socketIOEvent.data.ToString());
        EnemySpawner es = GetComponent<EnemySpawner>();
        es.SpawnEnemies(enemiesJSON);
    }

    // 플레이어가 접속했을 때 이벤트 콜백
    void OnOtherPlayerConnected(SocketIOEvent socketIOEvent)
    {
        print("Someone else joined");

        string data = socketIOEvent.data.ToString();
        UserJSON userJSON = UserJSON.CreateFromJSON(data);

        Vector3 position = new Vector3(userJSON.position[0], userJSON.position[1], userJSON.position[2]);
        Quaternion rotation = Quaternion.Euler(userJSON.rotation[0], userJSON.rotation[1], userJSON.rotation[2]);
        GameObject o = GameObject.Find(userJSON.name) as GameObject;

        if(o != null)
        {
            return;
        }

        GameObject p = Instantiate(player, position, rotation) as GameObject;

        // 세팅이 전부 끝나면 이름을 올려준다
        PlayerController pc = p.GetComponent<PlayerController>();
        Transform t = p.transform.Find("Healthbar Canvas");
        Transform t1 = t.transform.Find("PlayerName");
        Text playerName = t1.GetComponent<Text>();
        playerName.text = userJSON.name;
        pc.isLocalPlayer = false;
        p.name = userJSON.name;

        // 체력 설정 역시 해준다.
        Health h = p.GetComponent<Health>();
        h.currentHealth = userJSON.health;
        h.OnChangeHealth();
    }

    // 플레이어가 접속했을때 콜백 이벤트
    void OnPlay(SocketIOEvent socketIOEvent)
    {
        print("you Joined");

        string data = socketIOEvent.data.ToString();
        UserJSON currentUserJSON = UserJSON.CreateFromJSON(data);

        Vector3 position = new Vector3(currentUserJSON.position[0], currentUserJSON.position[1], currentUserJSON.position[2]);
        Quaternion rotation = Quaternion.Euler(currentUserJSON.rotation[0], currentUserJSON.rotation[1], currentUserJSON.rotation[2]);
        GameObject p = Instantiate(player, position, rotation) as GameObject;//GameObject.Find(currentUserJSON.name) as GameObject;



        // 세팅이 전부 끝나면 이름을 올려준다
        PlayerController pc = p.GetComponent<PlayerController>();
        Transform t = p.transform.Find("Healthbar Canvas");
        Transform t1 = t.transform.Find("PlayerName");
        Text playerName = t1.GetComponent<Text>();
        playerName.text = currentUserJSON.name;
        pc.isLocalPlayer = true;
        p.name = currentUserJSON.name;
    }

    void OnPlayerMove(SocketIOEvent socketIOEvent)
    {

    }

    void OnPlayerTurn(SocketIOEvent socketIOEvent)
    {

    }

    void OnPlayerShoot(SocketIOEvent socketIOEvent)
    {

    }

    void OnHealth(SocketIOEvent socketIOEvent)
    {

    }

    void OnOtherPlayerDisConnect(SocketIOEvent socketIOEvent)
    {
        print("user disconnect");
        
        string data = socketIOEvent.data.ToString();
        UserJSON userJSON = UserJSON.CreateFromJSON(data);
        Destroy(GameObject.Find(userJSON.name));
    }

    #endregion

    #region JSONMessageClasses

    [Serializable]
    public class PlayerJSON
    {
        public string name;
        public List<PointJSON> playerSpawnPoints;
        public List<PointJSON> enemySpawnPoints;

        public PlayerJSON(string _name, List<SpawnPoint> _playerSpawnPoints, List<SpawnPoint> _enemySpawnPoints)
        {
            playerSpawnPoints = new List<PointJSON>();
            enemySpawnPoints = new List<PointJSON>();
            name = _name;

            foreach(SpawnPoint playerSpawnPoint in _playerSpawnPoints)
            {
                PointJSON pointJSON = new PointJSON(playerSpawnPoint);
                playerSpawnPoints.Add(pointJSON);
            }

            foreach (SpawnPoint enemySpawnPoint in _enemySpawnPoints)
            {
                PointJSON pointJSON = new PointJSON(enemySpawnPoint);
                enemySpawnPoints.Add(pointJSON);
            }
        }
    }

    [Serializable]
    public class PointJSON
    {
        public float[] position;
        public float[] rotation;

        public PointJSON(SpawnPoint spawnPoint)
        {
            position = new float[]
            {
                spawnPoint.transform.position.x,
                spawnPoint.transform.position.y,
                spawnPoint.transform.position.z
            };

            rotation = new float[]
            {
                spawnPoint.transform.eulerAngles.x,
                spawnPoint.transform.eulerAngles.y,
                spawnPoint.transform.eulerAngles.z
            };

        }
    }

    [Serializable]
    public class PositionJSON
    {
        public float[] position;

        public PositionJSON(Vector3 _position)
        {
            position = new float[]
            {
                _position.x,
                _position.y,
                _position.z
            };
        }
    }

    [Serializable]
    public class RotationJSON
    {
        public float[] rotation;

        public RotationJSON(Quaternion _rotation)
        {
            rotation = new float[]
            {
                _rotation.eulerAngles.x,
                _rotation.eulerAngles.y,
                _rotation.eulerAngles.z
            };
        }
    }

    [Serializable]
    public class UserJSON
    {
        public string name;
        public float[] position;
        public float[] rotation;
        public int health;

        public static UserJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<UserJSON>(data);
        }
    }

    [Serializable]
    public class HealthChangeJSON
    {
        public string name;
        public int healthChange;
        public string from;

        public bool isEnemy;
        
        public HealthChangeJSON(string _name, int _heathChange, string _from, bool _isEnemy)
        {
            name = _name;
            healthChange = _heathChange;
            from = _from;
            isEnemy = _isEnemy;
        }
    }

    [Serializable]
    public class EnemyJSON
    {
        public List<UserJSON> enemies;

        public static EnemyJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<EnemyJSON>(data);
        }
    }

    [Serializable]
    public class ShootJSON
    {
        public string name;
        public static ShootJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<ShootJSON>(data);
        }
    }

    [Serializable]
    public class UserHealthJSON
    {
        public string name;
        public int health;

        public static UserHealthJSON CreateFromJSON(string data)
        {
            return JsonUtility.FromJson<UserHealthJSON>(data);
        }
    }
    #endregion
}
