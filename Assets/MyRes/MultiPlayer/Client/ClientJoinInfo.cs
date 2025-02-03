static class PackageClientJoinInfo
{
    public const byte Id = 0x01;
    public static byte[] Encode(
        byte selfId,
        string name
    )
    {
        using var writer = new Writer();
        writer.WriteByte(selfId);
        writer.WriteString(name);  
        return writer.EncodePackage(Id);
    }
}