using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;


namespace WrathTools
{
  /// <summary>
  /// Abstract base class for the SaveBridge system. Provides automatic validation and Save / Load bare-bones framework for
  /// derived types to lean on, as well as a non-generic Type to be boxed as for easier collection storage.
  /// </summary>
  public abstract class SaveObject
  {
    //TODO: Versioning and conversion framework
    private struct FieldPair
    {
      public FieldInfo Field { get; private set; }
      public FieldInfo Condition { get; private set; }

      public FieldPair(FieldInfo field, FieldInfo conditional)
      {
        Field = field;
        Condition = conditional;
      }
    }

    private static readonly Dictionary<Type, object> _defaultValueCache = new();
    private static readonly Dictionary<Type, FieldPair[]> _mustBeSetCache = new();

    private BuildState _state;

    /// <summary> The current validation state of the <see cref="SaveObject"/>. </summary>
    public BuildState State => _state;
    /// <summary> Whether this <see cref="SaveObject"/> has been validated and sealed yet. </summary>
    public bool Sealed { get; private set; }
    /// <summary> Whether this <see cref="SaveObject"/> has already been used. </summary>
    public bool Consumed { get; private set; }
    /// <summary> Whether this <see cref="SaveObject"/> is ready to be used to Save or Load. </summary>
    public bool ReadyToUse => Sealed && !Consumed && State.IsValid();
    /// <summary> The <see cref="System.Type"/> that should be expected from Load methods. </summary>
    public abstract Type Type { get; }

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

    /// <summary> 
    /// Populates the <see cref="SaveObject"/> from a <see cref="System.IO.BinaryReader"/> and automatically calls <see cref="MarkSealed"/>.
    /// Will not work if the <see cref="SaveObject"/> has already been sealed.
    /// </summary>
    public void BuildFrom(BinaryReader reader)
    {
      if(Sealed || Consumed) { return; }
      Read(reader);
      MarkSealed();
    }

    /// <summary> 
    /// Returns a common usage <see cref="System.InvalidOperationException"/> remarking the <see cref="SaveObject"/> was
    /// in an invalid state. Contains no logic, does not automatically throw, just returns the <see cref="System.InvalidOperationException"/>.
    /// </summary>
    public InvalidOperationException GetUnreadyException()
    {
      return new InvalidOperationException("Cannot use SaveObject, it is either Unsealed, Invalid, or Consumed.");
    }

    /// <summary> 
    /// Attempts to serialize the <see cref="SaveObject"/> data to the <see cref="System.IO.BinaryWriter"/>.
    /// Automatically calls <see cref="MarkConsumed"/>. Returns whether the attempt was successful. 
    /// </summary>
    public bool TrySave(BinaryWriter writer)
    {
      if(!ReadyToUse)
      {
        return false;
      }
      Write(writer);
      MarkConsumed();
      return true;
    }

    /// <summary>
    /// Attempts to construct a new instance of Type <typeparamref name="T"/> using the <see cref="SaveObject"/>'s data.
    /// Automatically calls <see cref="MarkConsumed"/>. Provides null and returns false if unsuccessful.
    /// </summary>
    public bool TryLoad<T>(out T instance) where T : class
    {
      if(!ReadyToUse || typeof(T) != Type)
      {
        instance = null;
        return false;
      }
      LoadInternal(out instance);
      MarkConsumed();
      return true;
    }

    /// <summary>
    /// Attempts to asynchronously construct a new instance of Type <typeparamref name="T"/> using the <see cref="SaveObject"/>'s data,
    /// and passes it, along with the success status, to the provided callback. Automatically calls <see cref="MarkConsumed"/>.
    /// </summary>
    public void LoadAsync<T>(Action<T, bool> onDone) where T : class
    {
      if(!ReadyToUse || typeof(T) != Type)
      {
        onDone?.Invoke(null, false);
        return;
      }
      LoadAsyncInternal(onDone);
      MarkConsumed();
    }

    /// <summary> Marks the <see cref="SaveObject"/> as sealed and then runs validation on its fields. </summary>
    public void MarkSealed()
    {
      if(Sealed || Consumed) { return; }
      Sealed = true;
      _state |= BuildState.Validated;
      if(!ValidateFields())
      {
        _state |= BuildState.MissingFields;
      }
      FinishValidation(ref _state);
    }

    /// <summary> Marks the <see cref="SaveObject"/> as consumed, preventing duplicate usage. </summary>
    public void MarkConsumed()
    {
      if(!Sealed || Consumed) { return; }
      Consumed = true;
      OnConsumed();
    }

    /// <summary> Overridable optional method called at the end of <see cref="MarkSealed"/>, used for custom validation. </summary>
    protected virtual void FinishValidation(ref BuildState state)
    {

    }

    /// <summary> Overridable optional method called at the end of <see cref="MarkConsumed"/>, used for custom consumption catching. </summary>
    protected virtual void OnConsumed()
    {

    }

    /// <summary>
    /// Sets the provided field of Type <typeparamref name="T"/> to the given value. 
    /// Will refuse if <see cref="Sealed"/> or <see cref="Consumed"/> are true.
    /// This should be used in derived types to easily ensure immutability once sealed.
    /// </summary>
    protected void SetValue<T>(ref T field, T value)
    {
      if(Sealed || Consumed) { return; }
      field = value;
    }

    /// <summary>
    /// Sets the provided field of Type <typeparamref name="T"/> to the given value and set the given boolean to true.
    /// Will refuse if <see cref="Sealed"/> or <see cref="Consumed"/> are true.
    /// This should be used in derived types to easily ensure immutability once sealed, and is useful for types where defaults are allowed, the boolean
    /// acting as a set check.
    /// </summary>
    protected void SetValue<T>(ref T field, T value, ref bool setCheck)
    {
      if(Sealed || Consumed) { return; }
      field = value;
      setCheck = true;
    }

    /// <summary> Constructs the data from the given <see cref="System.IO.BinaryReader"/>. </summary>
    protected abstract void Read(BinaryReader reader);
    /// <summary> Writes the data to the given <see cref="System.IO.BinaryWriter"/>. </summary>
    protected abstract void Write(BinaryWriter writer);

    protected internal abstract void LoadInternal<T>(out T instance) where T : class;
    protected internal abstract void LoadAsyncInternal<T>(Action<T, bool> onDone) where T : class;

    private bool ValidateFields()
    {
      Type type = GetType();
      if(!_mustBeSetCache.TryGetValue(type, out FieldPair[] mustBeSetFields))
      {
        mustBeSetFields = type
          .GetMembers(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
          .Select(m => (field: m as FieldInfo, attr: m.GetCustomAttribute<MustBeSetAttribute>()))
          .Where(p => p.field != null && p.attr != null)
          .Select(p =>
          {
            FieldInfo condition = null;
            if(!string.IsNullOrEmpty(p.attr.Condition))
            {
              condition = type.GetField(p.attr.Condition,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            }
            return new FieldPair(p.field, condition);
          })
          .ToArray();
        _mustBeSetCache[type] = mustBeSetFields;
      }
      foreach(FieldPair pair in mustBeSetFields)
      {
        if(pair.Condition != null)
        {
          if(!(bool)pair.Condition.GetValue(this)) { continue; }
        }
        if(IsDefault(pair.Field.FieldType, pair.Field.GetValue(this)))
        {
          return false;
        }
      }
      return true;
    }

  }
}