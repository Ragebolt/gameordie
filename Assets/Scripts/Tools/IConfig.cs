using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public interface IConfig
{
    string GetConfig();
    void SetConfig(string value);
    System.Type GetJsonType();
#if UNITY_EDITOR
    SerializedProperty GetProperty();
    void ApplyProperty();
#endif
}