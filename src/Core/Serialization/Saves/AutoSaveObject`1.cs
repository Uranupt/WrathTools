using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using System.IO;


namespace WrathTools
{
  public sealed class AutoSaveObject<TProvider> : SaveObject<TProvider> where TProvider : class, IAutoSaveProvider
  {

    private class FieldAndOrder
    {
      public FieldInfo Field;
      public int Order;
    }

    private readonly struct InstanceField
    {
      public readonly FieldInfo Field;
      public Type Type => Field.FieldType;
      public readonly object Value;

      public InstanceField(FieldInfo field, object value)
      {
        Field = field;
        Value = value;
      }
    }

    private static bool _initialized;
    private static Func<TProvider> _creator;
    private static List<FieldAndOrder> _saveFields;

    private readonly List<InstanceField> _instanceFields = new();

    private static void Initialize()
    {
      if(typeof(TProvider).IsAbstract)
      {
        Diagnostics.LogError(
          new InvalidOperationException("AutoSaveObjects can only be used with non abstract Types"),
          stackTrace: new(true)
        );
        return;
      }
      MethodInfo createInfo = typeof(TProvider).GetMethods(BindingFlags.Static)
        .Where(m => m.Name == "Create" && m.ReturnType == typeof(TProvider) && m.GetParameters().Length == 0)
        .FirstOrDefault();
      if(createInfo == null)
      {
        Diagnostics.LogError(
          new MissingMethodException($"The IAutoSaveProvider Type '{typeof(TProvider).Name}' is missing the required static Create method.")
        );
        return;
      }
      _creator = (Func<TProvider>)Delegate.CreateDelegate(typeof(TProvider), createInfo);
      _saveFields = new List<FieldAndOrder>(
        typeof(TProvider).GetFields()
        .Select(f => (field: f, attr: f.GetCustomAttribute<AutoSaveFieldAttribute>()))
        .Where(p => p.field != null && p.attr != null)
        .Select(p => new FieldAndOrder(){ Field = p.field, Order = p.attr.Order})
        .ToArray()
      );
      _saveFields.Sort((x, y) => x.Order - y.Order);
      for(int i = 0; i < _saveFields.Count; i++)
      {
        FieldAndOrder curr = _saveFields[i];
        if(!curr.Field.FieldType.IsBinarySerializable())
        {
          Diagnostics.LogWarning($"A field is marked with the AutoSaveField Attribute but is not of a Binary Convertible Type." +
            $" \n Class: '{typeof(TProvider).Name}', Field: '{curr.Field.Name}', FieldType: '{curr.Field.FieldType.Name}'");
          _saveFields.RemoveAt(i);
          i--;
          continue;
        }
        if(i < 1) { continue; }
        if(curr.Order == _saveFields[i - 1].Order)
        {
          Diagnostics.LogWarning($"The IAutoSaveProvider`1 Type '{typeof(TProvider).Name}' contains multiple AutoSaveField Attributes with the same order. " +
            $"This will result in inconsistent serialization and deserialization. " +
            $"Fields: '{curr.Field.Name}' and '{_saveFields[i - 1].Field.Name}'");
        }
      }
      _initialized = true;
    }

    private static TProvider Create()
    {
      if(!_initialized)
      {
        Initialize();
      }
      return _creator?.Invoke();
    }

    public AutoSaveObject(TProvider provider)
    {
      Build(provider);
    }

    public AutoSaveObject(BinaryReader reader)
    {
      Build(reader);
    }

    public void Build(TProvider provider)
    {
      if(State != BuildState.Incomplete)
      {
        Diagnostics.LogError(
          new InvalidOperationException("A SaveObject cannot be built after it has been sealed or consumed."),
          stackTrace: new(true)
        );
        return;
      }
      if(!_initialized)
      {
        Initialize();
      }
      for(int i = 0; i < _saveFields.Count; i++)
      {
        FieldInfo field = _saveFields[i].Field;
        _instanceFields.Add(new InstanceField(field, field.GetValue(provider)));
      }
      MarkSealed();
    }

    protected override TProvider LoadProtected()
    {
      TProvider resl = Create();
      foreach(InstanceField field in _instanceFields)
      {
        field.Field.SetValue(resl, field.Value);
      }
      return resl;
    }

    protected override async Task<TProvider> LoadAsyncProtected() => LoadProtected();

    protected override void Read(BinaryReader reader)
    {
      if(!_initialized)
      {
        Initialize();
      }
      for(int i = 0; i < _saveFields.Count; i++)
      {
        FieldInfo field = _saveFields[i].Field;
        _instanceFields.Add(new InstanceField(field, reader.ReadAs(field.FieldType)));
      }
    }

    protected override async Task ReadAsync(BinaryReader reader) => Read(reader);

    protected override void Write(BinaryWriter writer)
    {
      for(int i = 0; i < _instanceFields.Count; i++)
      {
        writer.WriteAs(_instanceFields[i].Type, _instanceFields[i].Value);
      }
    }

    protected override async Task WriteAsync(BinaryWriter writer) => Write(writer);

  }
}
