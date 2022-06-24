using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    public GameObject playerPrefab;
    public GameObject projectilePrefab;
    
    // timer
    [HideInInspector]public float _timeTick;
    public float _maxTime = 60.0f;

    [HideInInspector] public List<PlayerColors> player_colors;
    [HideInInspector] public List<Player> playerList;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        Server.Start(50, 26950);

        ResetPlayerColorList();

        playerList = new List<Player>();

        _timeTick = _maxTime;
    }

    private void Update()
    {
        if (_timeTick > 0.0f && playerList.Count > 0)
        {
            // decrement timer
            ServerSend.UpdateGameManager(_timeTick, _maxTime);
            _timeTick -= Time.deltaTime;
        }
        else if (playerList.Count <= 0)
        {
            // reset timer
            _timeTick = _maxTime;
            ServerSend.UpdateGameManager(_timeTick, _maxTime);
        }
    }

    public PlayerColors PlayerSelectColor()
    {
        if (player_colors.Count == 0)
        {
            ResetPlayerColorList();
        }

        int rand = Random.Range(0, player_colors.Count);
        var tempColor = player_colors[rand];
        player_colors.RemoveAt(rand);
        return tempColor;
    }

    public void ResetPlayerColorList()
    {
        player_colors = new List<PlayerColors>();
        for (int i = 0; i < Enum.GetValues(typeof(PlayerColors)).Length; i++)
        {
            player_colors.Add((PlayerColors)i);
        }
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    public Player InstantiatePlayer()
    {
        int rand = Random.Range(0, GameObjectHandler.instance.playerSpawns.Count);
        var temp = Instantiate(playerPrefab, GameObjectHandler.instance.playerSpawns[rand].transform.position,
            Quaternion.identity).GetComponent<Player>();
        return temp;
    }

    public Projectile InstantiateProjectile(Transform _shootOrigin)
    {
        return Instantiate(projectilePrefab, _shootOrigin.position + _shootOrigin.forward * 0.7f, Quaternion.identity).GetComponent<Projectile>();
    }
}