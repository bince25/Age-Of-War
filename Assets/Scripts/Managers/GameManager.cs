using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject leftCastle, rightCastle;
    public static GameManager Instance { get; private set; }

    public Dictionary<string, UnitController> unitsDictionary = new Dictionary<string, UnitController>();

    public Dictionary<string, Castle> castlesDictionary = new Dictionary<string, Castle>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log(SpawnSide.LeftCastle.ToString());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        castlesDictionary.Add(SpawnSide.LeftCastle.ToString(), leftCastle.GetComponent<Castle>());
        castlesDictionary.Add(SpawnSide.RightCastle.ToString(), rightCastle.GetComponent<Castle>());
    }
}

