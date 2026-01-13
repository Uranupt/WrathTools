using System.IO;
using UnityEngine;
using System;


namespace WrathTools.Unity
{
  public static class UnitySerialization
  {

    public static uint PackQuaternion(Quaternion value)
    {
      value = value.normalized;
      if(value.w < 0)
      {
        value = new Quaternion(-value.x, -value.y, -value.z, -value.w);
      }
      int maxIndex = 0;
      float maxValue = Mathf.Abs(value[0]);
      //Find highest value so it can be discarded
      for(int i = 1; i < 4; i++)
      {
        float component = Mathf.Abs(value[i]);
        if(component > maxValue)
        {
          maxValue = component;
          maxIndex = i;
        }
      }
      uint resl = (uint)maxIndex & 3; //Store index of dropped value

      //Store the remaining 3 raw to 10 bit (511) precision
      for(int i = 0, j = 2; i < 4; i++)
      {
        if(i == maxIndex) { continue; }
        float component = value[i] * Mathf.Sign(value[maxIndex]); //Preserves sign of droppped component
        resl |= ((uint)Mathf.RoundToInt((component + 1f) * 511f) << j);
        j += 10;
      }
      return resl;
    }

    public static Quaternion UnpackQuaternion(uint value)
    {
      int maxIndex = (int)(value & 3); //Retrieve index of dropped component
      int[] raw = new int[3];
      for(int i = 0, j = 2; i < 3; i++, j += 10)
      {
        raw[i] = (int)((value >> j) & 0x3ff); //Shift and trim by 0ing out bit outside component range, 0x3FF = 10 bits
      }
      float[] q = new float[4];
      q[maxIndex] = 0f;
      //Restore packed q by performing the inverse of Packing math
      for(int i = 0, j = 0; i < 4; i++)
      {
        if(i == maxIndex) { continue; }
        q[i] = (raw[j] / 511f) - 1f;
        q[maxIndex] += q[i] * q[i]; //Setting up dropped component reconstruction
        j++;
      }
      q[maxIndex] = Mathf.Sqrt(1f - q[maxIndex]);
      return new Quaternion(q[0], q[1], q[2], q[3]);
    }

    public static void Write(this BinaryWriter writer, Vector3 value)
    {
      writer.Write(value.x);
      writer.Write(value.y);
      writer.Write(value.z);
    }

    public static Vector3 ReadVector3(this BinaryReader reader)
    {
      return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
    }

    public static void Write(this BinaryWriter writer, Quaternion value)
    {
      writer.Write(value.x);
      writer.Write(value.y);
      writer.Write(value.z);
      writer.Write(value.w);
    }

    public static void PackAndWrite(this BinaryWriter writer, Quaternion value)
    {
      writer.Write(PackQuaternion(value));
    }

    public static Quaternion ReadQuaternion(this BinaryReader reader)
    {
      return new Quaternion(
        reader.ReadSingle(),
        reader.ReadSingle(),
        reader.ReadSingle(),
        reader.ReadSingle()
      );
    }

    public static Quaternion ReadPackedQuaternion(this BinaryReader reader)
    {
      return UnpackQuaternion(reader.ReadUInt32());
    }

  }
}