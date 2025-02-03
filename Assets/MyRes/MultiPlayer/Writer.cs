using System;
using System.IO;
using System.Text;
using UnityEngine;

public class Writer : IDisposable
{
    private readonly MemoryStream _data;
    private readonly BinaryWriter _writer;

    public Writer()
    {
        _data = new MemoryStream();
        _writer = new BinaryWriter(_data);
    }

    public void WriteByte(byte value)
    {
        _writer.Write(value);
    }

    public void WriteString(string value)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        ushort length = (ushort)bytes.Length;
        _writer.Write(length);
        _writer.Write(bytes);
    }

    public void WriteFloat(float value)
    {
        _writer.Write(value);
    }

    public void WriteBool(bool value)
    {
        _writer.Write(value);
    }

    public void WriteVector3(Vector3 value)
    {
        WriteFloat(value.x);
        WriteFloat(value.y);
        WriteFloat(value.z);
    }

    public byte[] EncodePackage(byte id)
    {
        _writer.Flush();
        byte[] dataBytes = _data.ToArray();
        ushort length = (ushort)(dataBytes.Length + 1);
        byte byte1 = (byte)(length & 0x00FF);
        byte byte2 = (byte)((length >> 8) & 0x00FF);
        byte[] result = new byte[3 + dataBytes.Length];
        result[0] = byte1;  // 低字节
        result[1] = byte2;  // 高字节
        result[2] = id;     // ID
        Array.Copy(dataBytes, 0, result, 3, dataBytes.Length);
        return result;
    }

    public void Flush()
    {
        _writer.Flush();
    }

    public void Dispose()
    {
        _writer.Dispose();
        _data.Dispose();
    }
}