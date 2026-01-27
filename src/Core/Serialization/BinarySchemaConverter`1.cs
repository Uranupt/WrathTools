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

    private class FieldConverter
    {
      private BinaryConverter _converter;
      private readonly string _serializerName;

      public readonly FieldInfo Field;
      public BinaryConverter Converter
      {
        get
        {
          if(_converter == null)
          {
            BinarySerialization.TryGetConverter(Field.FieldType, out _converter, true, _serializerName);
          }
          return _converter;
        }
      }

      public FieldConverter(FieldInfo field, BinaryConverter converter, string serializerName)
      {
        Field = field;
        _converter = converter;
        _serializerName = serializerName;
      }

      public void Read(BinaryReader reader, T instance)
      {
        Field.SetValue(instance, Converter.Read.Invoke(reader));
      }

      public void Write(BinaryWriter writer, T instance)
      {
        Converter.Write.Invoke(writer, Field.GetValue(instance));
      }

    }

    private readonly Dictionary<string, FieldConverter> _fields = new();
    private readonly List<FieldConverter> _sortedFields = new();
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
        SerializeBinaryAttribute attr = field.GetCustomAttribute<SerializeBinaryAttribute>();
        string serializerName = attr?.SerializerName;
        if(BinarySerialization.TryGetConverter(field.FieldType, out BinaryConverter fieldSerializer, true, serializerName)
          || autoTypes.Contains(field.FieldType))
        {
          _fields[field.Name] = new FieldConverter(field, fieldSerializer, serializerName);
          _sortedFields.Add(_fields[field.Name]);
        }
      }
      _sortedFields.Sort((x, y) => x.Field.Name.CompareTo(y.Field.Name));
      SetMethods(ReadLoop, WriteLoop);
    }

    private void WriteLoop(BinaryWriter writer, T instance)
    {
      writer.Write(_sortedFields.Count);
      foreach(FieldConverter field in _sortedFields)
      {
        writer.Write(field.Field.Name);
        field.Write(writer, instance); ;
      }
    }

    private T ReadLoop(BinaryReader reader)
    {
      T instance = _creator.Create();
      int count = reader.ReadInt32();
      for(int i = 0; i < count; i++)
      {
        if(_fields.TryGetValue(reader.ReadString(), out FieldConverter converter))
        {
          converter.Read(reader, instance);
        }
      }
      return instance;
    }

  }
}
