using UnityEngine;

class EventPlayerDamage
{
    public const byte Id = 0x05;
    public byte SenderId;
    public byte TargetId;
    public Vector3 Position;
    public byte Number;
    public byte[] Encode()
    {
        using var writer = new Writer();
        writer.WriteByte(SenderId);
        writer.WriteByte(TargetId);
        writer.WriteVector3(Position);
        writer.WriteByte(Number);
        return writer.EncodePackage(Id);
    }
    public EventPlayerDamage Read(byte[] data)
    {
        using var reader = new Reader(data);
        SenderId = reader.ReadByte();
        TargetId = reader.ReadByte();
        Position = reader.ReadVector3();
        Number = reader.ReadByte();
        return this;
    }
    public void Apply()
    {
        WUDamageNumber.Instance.AddEvent(this);
    }
}