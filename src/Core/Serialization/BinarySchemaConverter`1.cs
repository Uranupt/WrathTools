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
            BinarySerialization.TryGetConverterNoInitialize(Field.FieldType, out _converter, true, _serializerName);
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

      public void Read(BinaryReadContext context, T instance)
      {
        Field.SetValue(instance, Converter.Read(context));
      }

      public void Write(BinaryWriteContext context, T instance)
      {
        Converter.Write(context, Field.GetValue(instance));
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
        if(field.CustomAttributes.Any(a => a.AttributeType == typeof(DoNotSerializeAttribute))) { continue; }
        SerializeBinaryAttribute attr = field.GetCustomAttribute<SerializeBinaryAttribute>();
        string serializerName = attr?.SerializerName;
        if(BinarySerialization.TryGetConverterNoInitialize(field.FieldType, out BinaryConverter fieldSerializer, true, serializerName)
          || autoTypes.Contains(field.FieldType))
        {
          _fields[field.Name] = new FieldConverter(field, fieldSerializer, serializerName);
          _sortedFields.Add(_fields[field.Name]);
        }
      }
      _sortedFields.Sort((x, y) => x.Field.Name.CompareTo(y.Field.Name));
      SetMethods(ReadLoop, WriteLoop);
    }

    private void WriteLoop(BinaryWriteContext context, T instance)
    {
      context.Writer.Write(_sortedFields.Count);
      foreach(FieldConverter field in _sortedFields)
      {
        context.Writer.Write(field.Field.Name);
        field.Write(context, instance); ;
      }
    }

    private T ReadLoop(BinaryReadContext context)
    {
      T instance = _creator.Create();
      if(IsReferenceType)
      {
        context.AddToGraph(instance);
      }
      int count = context.Reader.ReadInt32();
      if(count != _sortedFields.Count)
      {
        Diagnostics.LogError(
          new InvalidDataException($"Schema deserialization for Type '{this.Type.Name}' cannot continue, the amount of fields in the" +
          $" provided stream do not match the schema. Ensure the serialized data represents the same version of the Type."),
          id: $"{Serialization.DiagnosticID}.schema_misaligned.binary",
          stackTrace: new(true)
        );
        return default;
      }
      for(int i = 0; i < count; i++)
      {
        string fieldName = context.Reader.ReadString();
        if(!_fields.TryGetValue(fieldName, out FieldConverter converter))
        {
          Diagnostics.LogError(
            new InvalidDataException($"Schema deserialization for Type '{this.Type.Name}' cannot continue, missing expected field with name: " +
            $"'{fieldName}. Ensure the serialized data represents the same version of the Type."),
            id: $"{Serialization.DiagnosticID}.missing_schema_field.binary",
            stackTrace: new(true)
          );
          return default;
        }
        converter.Read(context, instance);
      }
      return instance;
    }

  }
}
