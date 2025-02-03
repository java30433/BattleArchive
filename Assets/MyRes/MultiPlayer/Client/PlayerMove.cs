

using UnityEngine;

static class PackageClientPlayerMove
{
    public const byte Id = 0x03;
    public static byte[] Encode(
        byte selfId,
        Vector3 position,
        float rotationY,
        float speed,
        bool isAiming,
        bool isReloading
    )
    {
        using var writer = new Writer();
        writer.WriteByte(selfId);
        writer.WriteVector3(position);
        writer.WriteFloat(rotationY);
        writer.WriteFloat(speed);
        writer.WriteBool(isAiming);
        writer.WriteBool(isReloading);
        return writer.EncodePackage(Id);
    }
}