using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


namespace WrathTools
{
  internal sealed class CreatorFactory
  {

    private class Template
    {

      public readonly string Name;
      public readonly Type DeclaringType;
      public readonly RuntimeMethodHandle MethodHandle;

      public Template(string name, Type declaringType, MethodInfo method)
      {
        Name = name;
        DeclaringType = declaringType;
        MethodHandle = method.MethodHandle;
      }

    }

    private readonly List<Template> _templates = new();
    private readonly HashSet<Type> _builtTypes = new();
    private readonly object _buildLock = new();

    public readonly Type OpenType;
    public readonly bool IncludeConstructors;

    public CreatorFactory(Type openType)
    {
      OpenType = openType;
      IncludeConstructors = openType.CustomAttributes.Any(a => a.AttributeType == typeof(ConstructorsAsCreatorsAttribute));
    }

    public void AddTemplate(string name, Type declaringType, MethodInfo method)
    {
      _templates.Add(new Template(name, declaringType, method));
    }

    public void Build(Type closedType)
    {
      if(closedType.GetGenericTypeDefinition() != OpenType || _builtTypes.Contains(closedType)) { return; }
      lock(_buildLock )
      {
        if( _builtTypes.Contains(closedType)) { return; }
        if(IncludeConstructors)
        {
          Creators.TryDiscoverConstructors(closedType, out _);
        }
        foreach(Template template in _templates)
        {
          Type closedDeclaringType = template.DeclaringType == OpenType
            ? closedType : template.DeclaringType.MakeGenericType(closedType.GenericTypeArguments);
          MethodInfo createMethod = (MethodInfo)MethodBase.GetMethodFromHandle(template.MethodHandle, closedDeclaringType.TypeHandle);
          Creators.BuildCreators(closedType, createMethod.GetParameters(), (ex) => Expression.Call(createMethod, ex), template.Name);
        }
        _builtTypes.Add(closedType);
      }

    }

  }
}
