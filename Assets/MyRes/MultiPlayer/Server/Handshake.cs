class PackageServerHandShake
{
    public const byte Id = 0x00;
    public byte SelfId;
    
    public void Read(byte[] data)
    {
        using var reader = new Reader(data);
        SelfId = reader.ReadByte();
    }
}