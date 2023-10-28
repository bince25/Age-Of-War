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

    public Dictionary<string, Castle> castlesDictionary = new Dictionary<string, Castle>();

    public Dictionary<string, Turret> turretsDictionary = new Dictionary<string, Turret>();
    public Dictionary<string, Building> buildingsDictionary = new Dictionary<string, Building>();

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
        castlesDictionary.Add(SpawnSide.LeftCastle.ToString(), leftCastle.GetComponent<Castle>());
        castlesDictionary.Add(SpawnSide.RightCastle.ToString(), rightCastle.GetComponent<Castle>());
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

