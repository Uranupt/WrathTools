using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;


namespace WrathTools
{ 
  internal class BinarySchemaConverter<T> : BinaryConverter<T>
  {

    private readonly Dictionary<string, FieldInfo> _fields = new();
    private readonly ICreator<T> _create;

    internal BinarySchemaConverter(bool incPublic, HashSet<Type> allowedTypes)
    {
      _create = Creators.GetCreator<T>();
      HashSet<FieldInfo> autoFields = new(this.Type.GetFields(BindingFlags.Instance)
        .Where(f => f.GetCustomAttribute<SerializeBinaryAttribute>() != null));
      if(incPublic)
      {
        autoFields.UnionWith(this.Type.GetFields(BindingFlags.Instance | BindingFlags.Public));
      }
      foreach(FieldInfo field in autoFields)
      {
        if(!BinaryEnumerableSerializer.IsBaseTypeSerializable(field.FieldType, allowedTypes)) { continue; }
        _fields[field.Name] = field;
      }
      SetMethods(ReadLoop, WriteLoop);
    }

    private void WriteLoop(BinaryWriter writer, T instance)
    {
      writer.Write(_fields.Count);
      foreach(FieldInfo field in _fields.Values)
      {
        writer.Write(field.Name);
        writer.WriteAs(field.FieldType, field.GetValue(instance));
      }
    }

    private T ReadLoop(BinaryReader reader)
    {
      T instance = _create.Create();
      int count = reader.ReadInt32();
      for(int i = 0; i < count; i++)
      {
        if(_fields.TryGetValue(reader.ReadString(), out FieldInfo info))
        {
          info.SetValue(instance, reader.ReadAs(info.FieldType));
        }
      }
      return instance;
    }

  }
}
