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
    private readonly Creator<T> _creator;

    internal BinarySchemaConverter(string name, SerializationBehavior behavior, HashSet<Type> autoTypes) : base(name)
    {
      _creator = (Creator<T>)typeof(T).GetCreator();
      IEnumerable<FieldInfo> autoFields = this.Type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        .Where(f => behavior == SerializationBehavior.AllFields
          || (f.IsPublic && behavior == SerializationBehavior.PublicFields)
          || f.CustomAttributes.Any(a => a.AttributeType == typeof(SerializeBinaryAttribute))
        );
      foreach(FieldInfo field in autoFields)
      {
        if(!BinarySerialization.IsBaseTypeSerializable(field.FieldType, autoTypes)) { continue; }
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
      T instance = _creator.Create();
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
