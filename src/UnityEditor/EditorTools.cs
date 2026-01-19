using UnityEditor;
using WrathTools.Unity;
using System;


namespace WrathTools.UnityEditor
{
  public static class EditorTools
  {

    public static void EnsurePathExists(string path)
    {
      if(AssetDatabase.IsValidFolder(path)) { return; }
      string[] assetParts = path.Split('.');
      if(assetParts.Length > 1)
      {
        UnityDiagnostics.LogError(new Exception("Path contains a period, cannot assure valid folder pathing."));
        return;
      }
      string[] parts = path.Split('/');
      string currPath = "Assets";
      for(int i = 0; i < parts.Length; i++)
      {
        if(parts[i] == "Assets") { continue; }
        if(!AssetDatabase.IsValidFolder($"{currPath}/{parts[i]}"))
        {
          AssetDatabase.CreateFolder(currPath, parts[i]);
        }
        currPath += $"/{parts[i]}";
      }
    }

  }
}