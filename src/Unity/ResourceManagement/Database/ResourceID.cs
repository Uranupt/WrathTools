using System;

namespace WrathTools.Unity.ResourceManagement
{
  public static class ResourceID
  {

    private const int ResourceSpan = 4; //Number of base 10 digits members of a Collection occupy in the ID
    private const int CollectionSpan = 2; //Number of base 10 digits the Collections of a Library occupy in the ID
    private const int LibrarySpan = 3; //Number of base 10 digits the Libraries in a Database occupy in the ID

    private static readonly int LibraryShift = (int)Math.Pow(10, ResourceSpan + CollectionSpan);

    public static readonly int MaxResources = (int)Math.Pow(10, ResourceSpan);
    public static readonly int MaxCollections = (int)Math.Pow(10, CollectionSpan);
    public static readonly int MaxLibraries = (int)Math.Pow(10, LibrarySpan);

    /// <summary> Returns an integer representing the index of an ID's Type Library in the Database. </summary>
    public static int LibraryIndex(this int id) => (id / LibraryShift) % MaxLibraries;
    /// <summary> Returns an integer representing the index of an ID's Collection within its Type Library. </summary>
    public static int CollectionIndex(this int id) => (id / MaxResources) % MaxCollections;
    /// <summary> Returns an integer representing the index of an ID within its Collection. </summary>
    public static int ResourceIndex(this int id) => id % MaxResources;

    public static bool TryGetResourcePath(int id, out string path) => ResourceDatabase.Instance.TryGetResourcePath(id, out path);
    public static bool TryGetResourceType(int id, out Type type) => ResourceDatabase.Instance.TryGetResourceType(id, out type);
    public static bool TryGetBuildType(int id, out Type type) => ResourceDatabase.Instance.TryGetBuildType(id, out type);

    public static bool IsValid(int id) => ResourceDatabase.Instance.IsValidID(id);

    public static bool IsBuildType<T>(int id, bool exactType = true) => IsBuildType(id, typeof(T), exactType);
    public static bool IsBuildType(int id, Type buildType, bool exactType = true)
    {
      return TryGetBuildType(id, out Type idType) && buildType.TypeMatch(idType, exactType);
    }

    public static bool IsResourceType<T>(int id, bool exactType = true) where T : ResourceObject => IsResourceType(id, typeof(T), exactType);
    public static bool IsResourceType(int id, Type resourceType, bool exactType = true)
    {
      return TryGetResourceType(id, out Type idType) && resourceType.TypeMatch(idType, exactType);
    }

    public static Type GetResourceType(int id) => TryGetResourceType(id, out Type resl) ? resl : null;
    public static Type GetBuildType(int id) => TryGetBuildType(id, out Type resl) ? resl : null;

    public static int Build(int library, int collection, int resource)
    {
      return (library * LibraryShift) 
        + (collection * MaxResources) 
        + resource;
    }

    public static bool TryBuildFromPath(string libName, string collName, string resName, out int id)
    {
      if(ResourceDatabase.Instance.TryGetResourcePeek(libName, collName, resName, out ResourcePeek peek))
      {
        id = peek.ID;
        return true;
      }
      id = -1;
      return false;
    }

    public static bool TryBuildFromPath(Type type, string collName, string resName, out int id)
      => TryBuildFromPath(type.Name, collName, resName, out id);

    public static bool TryBuildFromPath<T>(string collName, string resName, out int id)
      => TryBuildFromPath(typeof(T), collName, resName, out id);

    public static bool TryBuildFromPath(string fullPath, out int id)
    {
      string[] parts = fullPath.Replace(".meta", "").Replace(".asset", "").Split('/');
      return TryBuildFromPath(parts[^3], parts[^2], parts[^1], out id);
    }

    public static int BuildFromPath(string libName, string collName, string resName)
    {
      return ResourceDatabase.Instance[libName, collName, resName].ID;
    }

    public static int BuildFromPath(Type type, string collName, string resName) => BuildFromPath(type.Name, collName, resName);
    public static int BuildFromPath<T>(string collName, string resName) => BuildFromPath(typeof(T), collName, resName);

    public static int BuildFromPath(string fullPath)
    {
      string[] parts = fullPath.Replace(".meta", "").Replace(".asset", "").Split('/');
      return BuildFromPath(parts[^3], parts[^2], parts[^1]);
    }

    public static string ToIDString(this int id, bool dashes = false)
    {
      return dashes 
        ? $"{id.LibraryIndex()}-{id.CollectionIndex()}-{id.ResourceIndex()}"
        : id.ToString("D9");
    }


  }
}