using System;
using System.Collections.Generic;
using UnityEngine;

class MultiPlayerManager : MonoBehaviour
{
    public GameObject PlayerObj;
    private readonly Dictionary<byte, ModelPlayer> _players = new();
    private readonly Dictionary<byte, PlayerStat> _objPlayers = new();
    public static MultiPlayerManager Instance { get; private set; }
    private PlayerStat _selfStat;
    private void Start()
    {
        Instance = this;
        _selfStat = PlayerObj.GetComponent<PlayerStat>();
    }
    private List<byte> _destroies = new();
    private void Update()
    {
        foreach (var pair in _players)
        {
            var model = pair.Value;
            if (pair.Key == Client.Instance.SelfId)
            {
                _selfStat.Target.Health = model.Health;
            }
            else
            {
                if (!_objPlayers.TryGetValue(pair.Key, out var playerStat))
                {
                    var clone = Instantiate(PlayerObj, Vector3.zero, Quaternion.identity);
                    Debug.Log("create player " + pair.Key);
                    Destroy(clone.GetComponent<PlayerController>());
                    Destroy(clone.GetComponent<PlayerShooter>());
                    playerStat = clone.GetComponent<PlayerStat>();
                    _objPlayers.Add(pair.Key, playerStat);
                }
                if (model.NeedDestroy)
                {
                    Destroy(playerStat.gameObject);
                    _destroies.Add(pair.Key);
                }
                else
                {
                    playerStat.transform.SetPositionAndRotation(
                        model.Position,
                        Quaternion.Euler(0, model.RotationY, 0)
                    );
                    playerStat.Speed = model.Speed;
                    playerStat.IsAiming = model.IsAiming;
                    playerStat.IsReloading = model.IsReloading;
                    playerStat.Target.Health = model.Health;
                    foreach (var e in model.EventList)
                    {
                        e.Dispath(playerStat);
                    }
                    model.EventList.Clear();
                }
            }
        }
        foreach (var destroy in _destroies)
        {
            _players.Remove(destroy);
            _objPlayers.Remove(destroy);
        }
        _destroies.Clear();
    }
    public ModelPlayer GetPlayer(byte playerId)
    {
        if (!_players.TryGetValue(playerId, out ModelPlayer result))
        {
            result = new ModelPlayer();
            _players.Add(playerId, result);
        }
        return result;
    }

    public byte FindPlayerId(GameObject obj)
    {
        foreach (var pair in _objPlayers)
        {
            if (pair.Value.gameObject == obj)
            {
                return pair.Key;
            }
        }
        throw new Exception("not found");
    }
}