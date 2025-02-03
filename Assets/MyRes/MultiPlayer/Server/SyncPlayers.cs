

using UnityEngine;

static class PackageServerSyncPlayers
{
    public const byte Id = 0x02;
    public static void SyncPlayers(byte[] data)
    {
        using var reader = new Reader(data);
        var count = reader.ReadByte();
        for (int i = 0; i < count; i++)
        {
            var id = reader.ReadByte();
            var player = MultiPlayerManager.Instance.GetPlayer(id);
            player.Position = reader.ReadVector3();
            player.RotationY = reader.ReadFloat();
            player.Speed = reader.ReadFloat();
            player.IsAiming = reader.ReadBool();
            player.IsReloading = reader.ReadBool();
            player.Health = reader.ReadShort();
        }
    }
}