using UnityEngine;

class EventPlayerFire : BasePlayerEvent
{
    public const byte Id = 0x04;
    public Vector3 StartPoint;
    public Vector3 Forward;

    public byte[] Encode()
    {
        using var writer = new Writer();
        writer.WriteByte(SenderId);
        writer.WriteVector3(StartPoint);
        writer.WriteVector3(Forward);
        return writer.EncodePackage(Id);
    }
    public EventPlayerFire Read(byte[] data)
    {
        using var reader = new Reader(data);
        SenderId = reader.ReadByte();
        StartPoint = reader.ReadVector3();
        Forward = reader.ReadVector3();
        return this;
    }
    public override void Dispath(PlayerStat playerStat)
    {
        playerStat.FireEffect(StartPoint, Forward);
    }
}