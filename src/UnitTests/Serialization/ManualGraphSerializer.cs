

namespace WrathTools.UnitTests
{
  [BinarySerializable(SerializationBehavior.Manual)]
  public sealed class ManualGraphSerializer
  {

    [BinarySerializable(SerializationBehavior.Manual)]
    public sealed class Cyclical
    {

      public ManualGraphSerializer Parent;

      public Cyclical(ManualGraphSerializer parent)
      {
        Parent = parent;
      }

      public static void Write(BinaryWriteContext context, Cyclical instance)
      {
        context.WriteAs(instance.Parent);
      }

      public static Cyclical Read(BinaryReadContext context)
      {
        return new Cyclical(context.ReadAs<ManualGraphSerializer>());
      }

    }

    [BinarySerializable(SerializationBehavior.Manual)]
    public sealed class Value
    {

      public int Content;

      public Value(int content)
      {
        Content = content;
      }

      public static void Write(BinaryWriteContext context, Value instance)
      {
        context.WriteAs<int>(instance.Content);
      }

      public static Value Read(BinaryReadContext context)
      {
        return new Value(context.ReadAs<int>());
      }

    }

    [BinarySerializable(SerializationBehavior.Manual)]
    public sealed class Twin
    {

      public Twin? Other;
      public Value ValueRef;

      public Twin(Value valueRef)
      {
        ValueRef = valueRef;
      }

      public static void Write(BinaryWriteContext context, Twin instance)
      {
        context.WriteAs(instance.ValueRef);
        context.WriteAs(instance.Other);
      }

      public static Twin Read(BinaryReadContext context)
      {
        Twin instance = new(context.ReadAs<Value>());
        context.AddToGraph(instance);
        instance.Other = context.ReadAs<Twin>();
        return instance;
      }

    }

    private Cyclical? _cyclical;
    private Value? _value;
    private Twin? _leftTwin;
    private Twin? _rightTwin;

    public ManualGraphSerializer(bool reading = false)
    {
      if(reading) { return; }
      _cyclical = new Cyclical(this);
      _value = new Value(13);
      _leftTwin = new(_value);
      _rightTwin = new(_value);
      _leftTwin.Other = _rightTwin;
      _rightTwin.Other = _leftTwin;
    }

    public static void Write(BinaryWriteContext context, ManualGraphSerializer instance)
    {
      context.WriteAs(instance._cyclical);
      context.WriteAs(instance._value);
      context.WriteAs(instance._leftTwin);
      context.WriteAs(instance._rightTwin);
    }

    public static ManualGraphSerializer Read(BinaryReadContext context)
    {
      ManualGraphSerializer instance = new(true);
      context.AddToGraph(instance);
      instance._cyclical = context.ReadAs<Cyclical>();
      instance._value = context.ReadAs<Value>();
      instance._leftTwin = context.ReadAs<Twin>();
      instance._rightTwin = context.ReadAs<Twin>();
      return instance;
    }

    public bool TestReferences()
    {
      return _cyclical?.Parent == this
        && _value?.Content == 13
        && _leftTwin?.Other == _rightTwin
        && _rightTwin?.Other == _leftTwin
        && _leftTwin?.ValueRef == _value
        && _rightTwin?.ValueRef == _value;
    }

  }
}
