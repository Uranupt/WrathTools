using UnityEditor;
using System;
using UnityEngine;
using WrathTools.Unity;
using UnityEngine.UIElements;


namespace WrathTools.UnityEditor
{
  public sealed class TextInputPopup : EditorWindow
  {

    private static readonly GUIStyle _labelStyle = new(EditorStyles.label)
    {
      alignment = TextAnchor.MiddleCenter
    };

    private string _message;
    private string _input;
    private Action<string> _onSubmit;
    private Action _onCancel;

    public static void Create(string message, Action<string> onSubmit, string defaultValue = "", Action onCancel = null, float width = 250, float height = 150)
    {
      TextInputPopup window = ScriptableObject.CreateInstance<TextInputPopup>();
      window.position = UnityTools.ScreenRectFromCenter(width, height);
      window._message = message;
      window._input = defaultValue;
      window._onSubmit = onSubmit;
      window._onCancel = onCancel;
      window.ShowUtility();
    }
    
    private void OnGui()
    {
      EditorGUILayout.LabelField(_message, _labelStyle);
      _input = EditorGUILayout.TextField(_input);
      EditorGUILayout.Space();
      GUILayout.BeginHorizontal();
      if(GUILayout.Button("Submit"))
      {
        _onSubmit?.Invoke(_input);
        _onCancel = null;
        Close();
      }
      GUILayout.FlexibleSpace();
      if(GUILayout.Button("Cancel"))
      {
        Close();
      }
      GUILayout.EndHorizontal();
    }

    private void OnDisable()
    {
      _onCancel?.Invoke();
    }

  }
}
