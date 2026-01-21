using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.IO;


namespace WrathTools
{
  internal class BinarySerializationSchema
  {

    private readonly Type _type;
    private readonly Func<object> _new;
    private readonly Dictionary<string, FieldInfo> _fields = new();

    public BinarySerializationSchema(Type type, bool incPublic, bool canNew, HashSet<Type> allowedTypes)
    {
      _type = type;
      //TODO: New vs Create
      HashSet<FieldInfo> autoFields = new(type.GetFields(BindingFlags.Instance)
        .Where(f => f.GetCustomAttribute<SerializeBinaryAttribute>() != null));
      if(incPublic)
      {
        autoFields.UnionWith(type.GetFields(BindingFlags.Instance | BindingFlags.Public));
      }
      foreach(FieldInfo field in autoFields)
      {
        if(!BinaryEnumerableSerializer.IsBaseTypeSerializable(field.FieldType, allowedTypes)) { continue; }
        _fields[field.Name] = field;
      }
    }

    public void Write(BinaryWriter writer, object instance)
    {
      writer.Write(_fields.Count);
      foreach(FieldInfo field in _fields.Values)
      {
        writer.Write(field.Name);
        writer.WriteAs(field.FieldType, field.GetValue(instance), true);
      }
    }

    public object Read(BinaryReader reader)
    {
      object instance = _type.Create();
      int count = reader.ReadInt32();
      for(int i = 0; i  < count; i++)
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
