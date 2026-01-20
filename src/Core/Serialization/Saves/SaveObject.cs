using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;


namespace WrathTools
{
  public abstract class SaveObject
  {
    //TODO: Versioning and conversion framework

    private static readonly Dictionary<Type, object> _defaultValueCache = new();
    private static readonly Dictionary<Type, FieldInfo[]> _mustBeSetCache = new();

    public BuildState State { get; private set; }
    public bool Sealed => State.Has(BuildState.Validated);
    public bool Consumed => State.Has(BuildState.Consumed);
    public bool Valid => State == BuildState.Validated;
    public abstract Type LoadType { get; }

    private static bool IsDefault(Type type, object obj)
    {
      if(obj == null)
      {
        return true;
      }
      if(type == typeof(string))
      {
        return string.IsNullOrEmpty((string)obj);
      }
      if(type.IsValueType)
      {
        if(!_defaultValueCache.TryGetValue(type, out object value))
        {
          value = Activator.CreateInstance(type);
          _defaultValueCache[type] = value;
        }
        return obj.Equals(value);
      }
      return false;
    }

    public void Build(BinaryReader reader)
    {
      if(State != BuildState.Incomplete)
      {
        Diagnostics.LogError(
          new InvalidOperationException("A SaveObject cannot be built after it has been sealed or consumed."),
          stackTrace: new(true)
        );
        return; 
      }
      Read(reader);
      MarkSealed();
    }

    public async Task BuildAsync(BinaryReader reader, Action onDone = null)
    {
      if(State != BuildState.Incomplete)
      {
        Diagnostics.LogError(
          new InvalidOperationException("A SaveObject cannot be built after it has been sealed or consumed."),
          stackTrace: new(true)
        );
        onDone?.Invoke();
        return;
      }
      await ReadAsync(reader);
      MarkSealed();
      onDone?.Invoke();
    }

    public void Save(BinaryWriter writer)
    {
      if(!Valid)
      {
        Diagnostics.LogError(
          new InvalidOperationException($"SaveObjects must be Valid to be used. Build State Flags: {State}"),
          stackTrace: new(true)
        );
        return;
      }
      Write(writer);
      MarkConsumed();
    }

    public async Task SaveAsync(BinaryWriter writer, Action onDone = null)
    {
      if(!Valid)
      {
        Diagnostics.LogError(
          new InvalidOperationException($"SaveObjects must be Valid to be used. Build State Flags: {State}"),
          stackTrace: new(true)
        );
        onDone?.Invoke();
        return;
      }
      await WriteAsync(writer);
      MarkConsumed();
      onDone?.Invoke();
    }

    public T Load<T>() where T : class
    {
      if(!Valid)
      {
        Diagnostics.LogError(
          new InvalidOperationException($"SaveObjects must be Valid to be used. Build State Flags: {State}"),
          stackTrace: new(true)
        );
        return null;
      }
      if(typeof(T).IsAssignableFrom(this.LoadType))
      {
        Diagnostics.LogError(
          new InvalidOperationException($"The Type '{this.LoadType.Name}' cannot be assigned to a field of Type '{typeof(T).Name}'"),
          stackTrace: new(true)
        );
        return null;
      }
      T resl = LoadInternal<T>();
      MarkConsumed();
      return resl;
    }

    public async Task<T> LoadAsync<T>(Action<T> onDone = null) where T : class
    {
      if(!Valid)
      {
        Diagnostics.LogError(
          new InvalidOperationException($"SaveObjects must be Valid to be used. Build State Flags: {State}"),
          stackTrace: new(true)
        );
        onDone?.Invoke(null);
        return null;
      }
      if(typeof(T).IsAssignableFrom(this.LoadType))
      {
        Diagnostics.LogError(
          new InvalidOperationException($"The Type '{this.LoadType.Name}' cannot be assigned to a field of Type '{typeof(T).Name}'"),
          stackTrace: new(true)
        );
        onDone?.Invoke(null);
        return null;
      }
      T resl = await LoadAsyncInternal<T>();
      MarkConsumed();
      onDone?.Invoke(resl);
      return resl;
    }

    public void MarkSealed()
    {
      if(Sealed) { return; }
      State |= BuildState.Validated;
      if(!ValidateFields())
      {
        State |= BuildState.MissingFields;
      }
      State |= (CustomValidation() & ~BuildState.Consumed);
    }

    public void MarkConsumed()
    {
      if(!State.IsValid() || Consumed) { return; }
      State |= BuildState.Consumed;
      OnConsumed();
    }

    protected virtual BuildState CustomValidation()
    {
      return BuildState.Incomplete;
    }

    protected virtual void OnConsumed()
    {

    }

    protected void SetValue<T>(ref T field, T value)
    {
      if(State != BuildState.Incomplete) { return; }
      field = value;
    }

    protected void SetValue<T>(ref T field, T value, ref bool setCheck)
    {
      if(State != BuildState.Incomplete) { return; }
      field = value;
      setCheck = true;
    }

    protected abstract void Read(BinaryReader reader);
    protected abstract Task ReadAsync(BinaryReader reader);
    protected abstract void Write(BinaryWriter writer);
    protected abstract Task WriteAsync(BinaryWriter writer);

    internal abstract T LoadInternal<T>() where T : class;
    internal abstract Task<T> LoadAsyncInternal<T>() where T : class;

    private bool ValidateFields()
    {
      Type type = GetType();
      if(!_mustBeSetCache.TryGetValue(type, out FieldInfo[] mustBeSetFields))
      {
        mustBeSetFields = type
          .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
          .Select(f => (field: f, attr: f.GetCustomAttribute<MustBeSetAttribute>()))
          .Where(p => p.field != null && p.attr != null)
          .Select(p => p.field)
          .ToArray();
        _mustBeSetCache[type] = mustBeSetFields;
      }
      foreach(FieldInfo field in mustBeSetFields)
      {
        if(IsDefault(field.FieldType, field.GetValue(this)))
        {
          return false;
        }
      }
      return true;
    }

  }
}