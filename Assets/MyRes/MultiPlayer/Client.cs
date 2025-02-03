using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

class Client : MonoBehaviour
{
    public string ServerAddress;
    public int ServerPort;
    public byte SelfId;
    private NetworkStream _stream;
    public bool IsConnected => _stream != null;
    public static Client Instance;
    public void Start()
    {
        Instance = this;
        Connect();
    }
    public void SendMove(PlayerStat player)
    {
        if (!IsConnected) return;
        _stream.WriteAsync(PackageClientPlayerMove.Encode(
            SelfId,
            player.transform.position,
            player.transform.rotation.eulerAngles.y,
            player.Speed,
            player.IsAiming,
            player.IsReloading
        ));
    }
    public void SendFire(Vector3 startPoint, Vector3 forward)
    {
        if (!IsConnected) return;
        _stream.WriteAsync(new EventPlayerFire(){
            SenderId = SelfId,
            StartPoint = startPoint,
            Forward = forward
        }.Encode());
    }

    public void SendDamage(byte targetId, Vector3 position, byte number)
    {
        if (!IsConnected) return;
        _stream.WriteAsync(new EventPlayerDamage(){
            SenderId = SelfId,
            TargetId = targetId,
            Position = position,
            Number = number
        }.Encode());
    }

    public void Connect()
    {
        Task.Run(() =>
        {
            var tcp = new TcpClient(ServerAddress, ServerPort);
            _stream = tcp.GetStream();
            var lengthBuffer = new byte[2];
            while (true)
            {
                try
                {
                    _stream.Read(lengthBuffer, 0, 2);
                    var length = (ushort)(lengthBuffer[0] | (lengthBuffer[1] << 8));
                    byte[] data = new byte[length];
                    _stream.Read(data, 0, length);
                    switch (data[0])
                    {
                        case PackageServerHandShake.Id:
                            var handshake = new PackageServerHandShake();
                            handshake.Read(data);
                            SelfId = handshake.SelfId;
                            _stream.Write(PackageClientJoinInfo.Encode(SelfId, "Player" + SelfId));
                            break;
                        case PackageServerSyncPlayers.Id:
                            PackageServerSyncPlayers.SyncPlayers(data);
                            break;
                        case EventPlayerFire.Id:
                            new EventPlayerFire().Read(data).Apply();
                            break;
                        case EventPlayerDamage.Id:
                            new EventPlayerDamage().Read(data).Apply();
                            break;
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    _stream.Close();
                    _stream.Dispose();
                    _stream = null;
                    break;
                }
            }
        });
    }

    private void OnDestroy()
    {
        if (_stream != null)
        {
            _stream.Close();
            _stream.Dispose();
            _stream = null;
        }
    }
}