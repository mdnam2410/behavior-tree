using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    [SerializeField]
    private List<GameObject> _players;
    
    private int _alivePlayersCount;
    public int PlayersCount
    {
        get => _players.Count;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _alivePlayersCount = CountAlivePlayers();
    }

    private void LateUpdate()
    {
        _alivePlayersCount = CountAlivePlayers();
        Debug.Log($"PlayerManager: {_alivePlayersCount} players are alive");
    }

    private int CountAlivePlayers()
    {
        int alivePlayersCount = 0;
        for (int i = 0; i < PlayersCount; ++i)
        {
            if (_players[i].activeInHierarchy)
            {
                ++alivePlayersCount;
            }
        }
        return alivePlayersCount;
    }

    public bool AllPlayerAreDead => _alivePlayersCount == 0;
}
