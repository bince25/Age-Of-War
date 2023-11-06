using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameMode gameMode;
    public IGameStrategy CurrentStrategy { get; private set; }

    [SerializeField]
    private GameObject leftCastle, rightCastle;
    public static GameManager Instance { get; private set; }

    public Dictionary<string, UnitController> unitsDictionary = new Dictionary<string, UnitController>();

    public Dictionary<string, CastleController> castlesDictionary = new Dictionary<string, CastleController>();

    public Dictionary<string, Turret> turretsDictionary = new Dictionary<string, Turret>();
    public Dictionary<string, BuildingController> buildingsDictionary = new Dictionary<string, BuildingController>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SetGameStrategy(new LocalGameStrategy());
        castlesDictionary.Add(SpawnSide.LeftCastle.ToString(), leftCastle.GetComponent<CastleController>());
        castlesDictionary.Add(SpawnSide.RightCastle.ToString(), rightCastle.GetComponent<CastleController>());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            // Switch to online strategy
            SetGameStrategy(new OnlineGameStrategy());
            AuthManager.Instance.DefaultLogin(() =>
            {
                WebSocketClient.Instance.ConnectToWebSocket();
            });

        }
    }

    public void SetGameStrategy(IGameStrategy strategy)
    {
        CurrentStrategy = strategy;
    }
}

