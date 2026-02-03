using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;


namespace WrathTools
{ 

  internal sealed class BinaryConverterFactory
  {

    private class Template
    {
      public readonly string Name;
      public readonly Type DeclaringType;

      public Template(string name, Type declaringType)
      {
        Name = name;
        DeclaringType = declaringType;
      }
    }

    private readonly Dictionary<string, Template> _templates = new();
    public readonly Type OpenType;


    public BinaryConverterFactory(Type openType)
    {
      OpenType = openType;
    }

    public bool AddTemplate(string name, Type declaringType)
    {
      if(_templates.ContainsKey(name)) { return false; }
      _templates[name] = new Template(name, declaringType);
      return true;
    }

    //TODO: Why isn't this just building everything at once? Let's try and change it to do that
    public bool TryBuild(Type closedType, out BinaryConverter converter, string name = null, HashSet<Type> autoTypes = null)
    {
      Template template;
      if(name == null && _templates.Count == 1)
      {
        template = _templates.Values.First();
      }
      else
      {
        _templates.TryGetValue(name ?? BinarySerialization.DefaultConverterName, out template);
      }
      if(template == null)
      {
        Diagnostics.LogWarning(
          $"No BinarySerializer with name '{name ?? "(none)"}' or ambiguous default " +
          $"for open generic Type '{OpenType.Name}'",
          id: $"{Serialization.DiagnosticID}.factory_missing_name.binary"
        );
        converter = null;
        return false;
      }

      Type closedDeclaringType;
      if(template.DeclaringType != closedType)
      {
        closedDeclaringType = template.DeclaringType.MakeGenericType(closedType.GenericTypeArguments);
      }
      else
      {
        closedDeclaringType = closedType;
        BinarySerializableAttribute attr = closedDeclaringType.GetCustomAttribute<BinarySerializableAttribute>();
        if(attr.Behavior != SerializationBehavior.Manual)
        {
          if(!closedType.HasCreator(true))
          {
            Diagnostics.LogWarning(
              $"The Type '{closedType}' marked with the BinarySerializable Attribute does not have any available parameterless" +
              $" constructors or parameterless Creators. Unable to build a serialization schema.",
              id: $"{Serialization.DiagnosticID}.factory_missing_creator.binary"
            );
            converter = null;
            return false;
          }
          converter = (BinaryConverter)BinarySerialization.SchemaConverterBuilder
            .MakeGenericMethod(closedType).Invoke(null, new object[] { template.Name, attr.Behavior, autoTypes ?? new HashSet<Type>() });
          return true;
        }
      }

      if(!BinarySerialization.TrySelectSerializerMethods(closedDeclaringType, closedType, out MethodInfo read, out MethodInfo write))
      {
        Diagnostics.LogWarning(
          $"The Type '{closedDeclaringType.Name}' is missing the required Read and Write methods to satisfy BinarySerialization for" +
          $" the Type '{closedType.Name}'. Read: {read != null}, Write: {write != null}",
          id: $"{Serialization.DiagnosticID}.factory_missing_methods.binary"
        );
        converter = null;
        return false;
      }
      converter = (BinaryConverter)BinarySerialization.ManualConverterBuilder
        .MakeGenericMethod(closedType).Invoke(null, new object[] { template.Name, read, write });
      return true;
    }

  }

}

