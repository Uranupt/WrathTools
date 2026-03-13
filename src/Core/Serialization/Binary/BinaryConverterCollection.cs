using System.Collections.Generic;
using System;
using System.Linq;


namespace WrathTools
{
  internal class BinaryConverterCollection
  {

    private Dictionary<string, BinaryConverter> _converters = new();

    public readonly Type Type;

    public BinaryConverterCollection(Type type)
    {
      this.Type = type;
    }

    public bool TryGetConverter(string name, out BinaryConverter converter) => _converters.TryGetValue(name, out converter);

    public bool TryGetConverter(out BinaryConverter converter)
    {
      if(!_converters.TryGetValue(BinarySerialization.DefaultConverterName, out converter))
      {
        if(_converters.Count == 1)
        {
          converter = _converters.Values.FirstOrDefault();
        }
      }
      return converter != null;
    }

    public BinaryConverter GetConverter(string name)
    {
      if(!TryGetConverter(name, out BinaryConverter resl))
      {
        Diagnostics.LogError(
          new KeyNotFoundException($"No BinaryConverter found for Type '{this.Type.Name}' with name '{name}'"),
          id: $"{Serialization.DiagnosticID}.missing_converter.binary",
          stackTrace: new(true)
        );
      }
      return resl;
    }

    public BinaryConverter GetConverter()
    {
      if(!TryGetConverter(out BinaryConverter resl))
      {
        Diagnostics.LogError(
          new Exception($"No default BinaryConverter for Type '{this.Type.Name}' and multiple named converters, cannot resolve ambiguity. Add a default or specify a name."),
          id: $"{Serialization.DiagnosticID}.ambiguous_default_converter.binary",
          stackTrace: new(true)
        );
      }
      return resl;
    }

    public bool AddConverter(BinaryConverter converter)
    {
      if(converter.Type != this.Type
        || _converters.ContainsKey(converter.Name)) { return false; }
      _converters[converter.Name] = converter;
      return true;
    }

  }
}