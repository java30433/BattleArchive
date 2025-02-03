using System;
using System.IO;
using System.Text;
using UnityEngine;

class Reader : IDisposable
{
    private BinaryReader _reader;

    public Reader(byte[] data)
    {
        _reader = new BinaryReader(new MemoryStream(data));
        _reader.ReadByte();
    }

    public void Dispose()
    {
        _reader.Dispose();
    }

    public byte ReadByte()
    {
        return _reader.ReadByte();
    }
    public string ReadString()
    {
        // 读取 uint16 长度
        ushort length = _reader.ReadUInt16();

        // 从流中读取指定长度的字节
        byte[] stringData = _reader.ReadBytes(length);

        // 将字节数组转换为字符串并返回
        return Encoding.UTF8.GetString(stringData);
    }
    public float ReadFloat()
    {
        return _reader.ReadSingle();
    }
    public bool ReadBool()
    {
        return _reader.ReadBoolean();
    }
    public short ReadShort()
    {
        return _reader.ReadInt16();
    }
    public int ReadInt()
    {
        return _reader.ReadInt32();
    }
    public Vector3 ReadVector3()
    {
        float x = ReadFloat();
        float y = ReadFloat();
        float z = ReadFloat();
        return new Vector3(x, y, z);
    }
}