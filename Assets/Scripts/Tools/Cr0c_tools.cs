using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UObject = UnityEngine.Object;
using SObject = System.Object;

// проверка наличия слоя в маске
// ReSharper disable once CheckNamespace
public static class LayerExtension {
    public static bool Contains(this LayerMask layerMask, int layer) {
        return layerMask == (layerMask | (1 << layer));
    }
}

public static class Vector3Extend {
    public static float GetXZDistance(this Vector3 from, Vector3 to) {
        to.y = from.y;
        return Vector3.Distance(from, to);
    }

    public static Vector3 GetXZVector(this Vector3 from, Vector3 to) {
        to.y = from.y;
        return to - from;
    }

    public static float GetXZAngle(this Vector3 from, Vector3 to) {
        to.y = from.y;
        return Vector3.Angle(from, to);
    }

    public static float GetXZAngleSigned(this Vector3 from, Vector3 to) {
        to.y = from.y;
        return Vector3.SignedAngle(from, to, Vector3.up);
    }

    public static Vector3 GetAngleVector3(this Vector3 from, float angle) {
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad), 0f);
    }

    public static Vector3 GetVector3(this Vector2Int from) {
        return new Vector3(from.x, from.y, 0f);
    }

    public static Vector2Int GetVector2Int(this Vector3 from) {
        return new Vector2Int((int)from.x, (int)from.y);
    }
}

public static class RectIntExtend {
    public static Vector2 VectorCenter(this RectInt from) {
        return new Vector2(from.x + from.width * 0.5f, from.y + from.height * 0.5f);
    }

    public static Vector3 VectorSize(this RectInt from) {
        return new Vector3(from.width, from.height, 0f);
    }

    public static Vector3 VectorPos(this RectInt from) {
        return new Vector3(from.x, from.y, 0f);
    }
}

public static class MonoBehaviourExtension {
    /// <summary>
    ///     Call delegate after pause (time scaled)
    /// </summary>
    /// <param name="mn"></param>
    /// <param name="func">Delegate</param>
    /// <param name="time">Pause time</param>
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static Coroutine InvokeDelegate(this MonoBehaviour mn, Action func, float time) {
        return mn.StartCoroutine(InvokeDelegateCor(func, time));
    }

    private static IEnumerator InvokeDelegateCor(Action func, float time) {
        yield return new WaitForSeconds(time);
        func();
    }

    /// <summary>
    ///     Call delegate after pause (time unscaled)
    /// </summary>
    /// <param name="mn"></param>
    /// <param name="func">Delegate</param>
    /// <param name="time">Pause time</param>
    // ReSharper disable once UnusedMember.Global
    public static Coroutine InvokeDelegateUnscaled(this MonoBehaviour mn, Action func, float time) {
        return mn.StartCoroutine(InvokeDelegateUnscaledCor(func, time));
    }

    private static IEnumerator InvokeDelegateUnscaledCor(Action func, float time) {
        yield return new WaitForSecondsRealtime(time);
        func();
    }

    /// <summary>
    ///     Call delegate by time with normalized time in parameter (time scaled)
    /// </summary>
    /// <param name="mn"></param>
    /// <param name="func">Delegate with parameter float [0..1]</param>
    /// <param name="time">Time to work</param>
    /// <param name="endFunc">Delegate after end</param>
    // ReSharper disable once UnusedMember.Global
    public static Coroutine InvokeDelegate(this MonoBehaviour mn, Action<float> func, float time, Action endFunc = null) {
        return mn.StartCoroutine(InvokeDelegateCor(func, time, endFunc));
    }

    private static IEnumerator InvokeDelegateCor(Action<float> func, float time, Action endFunc) {
        var timer = 0f;
        while (timer <= time) {
            func(timer / time);
            yield return null;
            timer += Time.deltaTime;
        }

        if (endFunc != null)
            endFunc();
    }

    /// <summary>
    ///     Call delegate by time with normalized time in parameter (time unscaled)
    /// </summary>
    /// <param name="mn"></param>
    /// <param name="func">Delegate with parameter float [0..1]</param>
    /// <param name="time">Time to work</param>
    /// <param name="endFunc">Delegate after end</param>
    // ReSharper disable once UnusedMember.Global
    public static Coroutine InvokeDelegateUnscaled(this MonoBehaviour mn, Action<float> func, float time, Action endFunc = null) {
        return mn.StartCoroutine(InvokeDelegateUnscaledCor(func, time, endFunc));
    }

    private static IEnumerator InvokeDelegateUnscaledCor(Action<float> func, float time, Action endFunc) {
        var timer = 0f;
        while (timer <= time) {
            func(timer / time);
            yield return null;
            timer += Time.unscaledDeltaTime;
        }

        if (endFunc != null)
            endFunc();
    }

    /// <summary>
    ///     TRUE if cursor (touch) is over UI element
    /// </summary>
    /// <returns>TRUE if cursor (touch) is over UI element</returns>
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once UnusedParameter.Global
    public static bool CursorOverUI(this MonoBehaviour mn) {
#if (UNITY_ANDROID || UNITY_IOS) && (!UNITY_EDITOR)
        int cursorID = Input.GetTouch(0).fingerId;
        return EventSystem.current.IsPointerOverGameObject(cursorID);
#else
        return EventSystem.current.IsPointerOverGameObject();
#endif
    }
}

// ReSharper disable once InconsistentNaming
public static class UIDebug {
    public static readonly Color Warning = Color.magenta;
    public static readonly Color Info    = Color.black;
    public static readonly Color Assert  = Color.red;

    // ReSharper disable once UnusedMember.Global
    public static readonly Color Network = Color.blue;

    // ReSharper disable once UnusedMember.Global
    public static readonly Color State = Color.green;

    // ReSharper disable once UnusedMember.Global
    public static readonly Color Other = Color.white;

    // ReSharper disable once UnusedMember.Global
    public static readonly Color System = Color.cyan;

    /// <summary>
    ///     Стандартный: первый string раскрашивается и делается жирным (поддерживается Format)
    /// </summary>
    /// <param name="color">Цвет UnityEngine.Color</param>
    /// <param name="headerText">Заголовок</param>
    /// <param name="text">Остальной текст</param>
    /// <param name="param">прочие параметры</param>
    public static void Log(Color color, string headerText, string text = "", params object[] param) {
        var colorString = ColorUtility.ToHtmlStringRGBA(color);
        Debug.LogFormat("<color=#" + colorString + "><b>" + headerText + "</b></color>" + text, param);
    }

    /// <summary>
    ///     Стандартный: первый string раскрашивается и делается жирным (поддерживается Format)
    /// </summary>
    /// <param name="color">Цвет в формате RRGGBBAA</param>
    /// <param name="headerText">Заголовок</param>
    /// <param name="text">Остальной текст</param>
    /// <param name="param">прочие параметры</param>
    // ReSharper disable once UnusedMember.Global
    public static void Log(string color, string headerText, string text = "", params object[] param) {
        var colorString = color;
        var s           = "";
        for (var i = 0; i < param.Length; i++)
            if (param[i] is string)
                s += (string) param[i];
        Debug.LogFormat("<color=#" + colorString + "><b>" + headerText + "</b></color>" + text + s, param);
    }

    /// <summary>
    ///     Warning: первый string раскрашивается и делается жирным (поддерживается Format)
    /// </summary>
    /// <param name="color">Цвет UnityEngine.Color</param>
    /// <param name="headerText">Заголовок</param>
    /// <param name="text">Остальной текст</param>
    /// <param name="param">прочие параметры</param>
    public static void LogWarning(Color color, string headerText, string text = "", params object[] param) {
        var colorString = ColorUtility.ToHtmlStringRGBA(color);
        var s           = "";
        for (var i = 0; i < param.Length; i++)
            if (param[i] is string)
                s += (string) param[i];
        Debug.LogWarningFormat("<color=#" + colorString + "><b>" + headerText + "</b></color>" + text + s, param);
    }

    /// <summary>
    ///     Warning: первый string раскрашивается и делается жирным (поддерживается Format)
    /// </summary>
    /// <param name="color">Цвет в формате RRGGBBAA</param>
    /// <param name="headerText">Заголовок</param>
    /// <param name="text">Остальной текст</param>
    /// <param name="param">прочие параметры</param>
    // ReSharper disable once UnusedMember.Global
    public static void LogWarning(string color, string headerText, string text = "", params object[] param) {
        var colorString = color;
        var s           = "";
        for (var i = 0; i < param.Length; i++)
            if (param[i] is string)
                s += (string) param[i];
        Debug.LogWarningFormat("<color=#" + colorString + "><b>" + headerText + "</b></color>" + text + s, param);
    }

    /// <summary>
    ///     Assertion: первый string раскрашивается и делается жирным (поддерживается Format)
    /// </summary>
    /// <param name="color">Цвет UnityEngine.Color</param>
    /// <param name="headerText">Заголовок</param>
    /// <param name="text">Остальной текст</param>
    /// <param name="param">прочие параметры</param>
    // ReSharper disable once UnusedMember.Global
    public static void LogAssertion(Color color, string headerText, string text = "", params object[] param) {
        var colorString = ColorUtility.ToHtmlStringRGBA(color);
        var s           = "";
        for (var i = 0; i < param.Length; i++)
            if (param[i] is string)
                s += (string) param[i];
        Debug.LogAssertionFormat("<color=#" + colorString + "><b>" + headerText + "</b></color>" + text + s, param);
    }

    /// <summary>
    ///     Assertion: первый string раскрашивается и делается жирным (поддерживается Format)
    /// </summary>
    /// <param name="color">Цвет в формате RRGGBBAA</param>
    /// <param name="headerText">Заголовок</param>
    /// <param name="text">Остальной текст</param>
    /// <param name="param">прочие параметры</param>
    // ReSharper disable once UnusedMember.Global
    public static void LogAssertion(string color, string headerText, string text = "", params object[] param) {
        var colorString = color;
        var s           = "";
        for (var i = 0; i < param.Length; i++)
            if (param[i] is string)
                s += (string) param[i];
        Debug.LogAssertionFormat("<color=#" + colorString + "><b>" + headerText + "</b></color>" + text + s, param);
    }

    private static float lastTimeStamp = 0f;

    public static void LogStamp(string text) {
        if (lastTimeStamp <= 0f) lastTimeStamp = Time.time;
        Debug.LogFormat("[{0}] {1}", (Time.time - lastTimeStamp).ToString("F3"), text);
        lastTimeStamp = Time.time;
    }

    /// <summary>
    ///     Получение полного пути к объекту
    /// </summary>
    /// <param name="object">Объект</param>
    /// <param name="divider">Разделитель пути</param>
    /// <returns></returns>
    // ReSharper disable once UnusedMember.Global
    public static string GetFullPath(GameObject @object, string divider = " → ") {
        var s  = @object.name;
        var tr = @object.transform.parent;
        while (tr != null) {
            s  = tr.name + divider + s;
            tr = tr.parent;
        }
        return s;
    }

    /// <summary>
    ///     Получение полного пути к объекту
    /// </summary>
    /// <param name="object">Объект</param>
    /// <param name="divider">Разделитель пути</param>
    /// <returns></returns>
    // ReSharper disable once UnusedMember.Global
    public static string GetFullPath(Transform @object, string divider = " → ") {
        var s  = @object.name;
        var tr = @object.parent;
        while (tr != null) {
            s  = tr.name + divider + s;
            tr = tr.parent;
        }
        return s;
    }

    /// <summary>
    ///     Получение полного пути к объекту
    /// </summary>
    /// <param name="object">Объект</param>
    /// <param name="divider">Разделитель пути</param>
    /// <returns></returns>
    // ReSharper disable once UnusedMember.Global
    public static string GetFullPath(MonoBehaviour @object, string divider = " → ") {
        var s  = @object.name;
        var tr = @object.transform.parent;
        while (tr != null) {
            s  = tr.name + divider + s;
            tr = tr.parent;
        }
        return s;
    }
}

#if UNITY_EDITOR
namespace EasyEditorGUI {
// ReSharper disable once InconsistentNaming
    public static class eGUI {
        #region Размер подписей + сдвиги от ident

        private const  float fixedIdentDefault = 60f;
        private static float _fixedIdent       = 60f;

        /// <summary>
        ///     Size of indent
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public static float indentSize {
            get { return EditorGUI.indentLevel * 15f; }
        }

        /// <summary>
        ///     Size of work area without indent
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public static float indentWidth {
            get { return EditorGUIUtility.currentViewWidth - indentSize - 5f; }
        }

        /// <summary>
        ///     Set label width to fixedIdent
        /// </summary>
        /// <param name="value">FALSE - set to default</param>
        public static void SetLabelWidth(bool value) {
            EditorGUIUtility.labelWidth = value ? indentSize + _fixedIdent : 0f;
        }

        /// <summary>
        ///     Set label width to fixedIdent with defined size
        /// </summary>
        /// <param name="size">defined size (less zero to set default)</param>
        /// <param name="value">FALSE - set to default</param>
        // ReSharper disable once UnusedMember.Global
        public static void SetLabelWidth(float size, bool value) {
            if (size < 0f)
                _fixedIdent = fixedIdentDefault;
            _fixedIdent = size;
            SetLabelWidth(value);
        }

        #endregion

        #region Преобразование типов

        /// <summary>
        ///     Convert Quaternion to Vector4
        /// </summary>
        /// <param name="rot"></param>
        /// <returns></returns>
        public static Vector4 QuaternionToVector4(Quaternion rot) {
            return new Vector4(rot.x, rot.y, rot.z, rot.w);
        }

        /// <summary>
        ///     Convert Vector4 to Quaternion
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Global
        public static Quaternion Vector4ToQuaternion(Vector4 vec) {
            return new Quaternion(vec.x, vec.y, vec.z, vec.w);
        }

        #endregion

        #region Создание вертикального скролла

        private static Dictionary<int, Vector2> _scrollPoss;

        public static void StartScrollVertical(EditorWindow value) {
            var hash = value.GetHashCode();
            if (_scrollPoss == null)
                _scrollPoss = new Dictionary<int, Vector2>();
            if (!_scrollPoss.ContainsKey(hash))
                _scrollPoss.Add(hash, Vector2.zero);
            _scrollPoss[hash] = GUILayout.BeginScrollView(_scrollPoss[hash], false, true, GUIStyle.none, GUI.skin.verticalScrollbar);
            GUILayout.BeginVertical();
        }

        public static void StartScrollVertHoriz(EditorWindow value) {
            var hash = value.GetHashCode();
            if (_scrollPoss == null)
                _scrollPoss = new Dictionary<int, Vector2>();
            if (!_scrollPoss.ContainsKey(hash))
                _scrollPoss.Add(hash, Vector2.zero);
            _scrollPoss[hash] = GUILayout.BeginScrollView(_scrollPoss[hash], true, true, GUI.skin.horizontalScrollbar, GUI.skin.verticalScrollbar);
            GUILayout.BeginVertical();
        }

        public static void EndScroll() {
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }

        #endregion

        #region Универсальный фолд

        private static readonly Dictionary<UObject, bool> foldouts = new Dictionary<UObject, bool>();

        public static bool ShowFold(string text, UObject @object) {
            var result = false;
            if (!foldouts.ContainsKey(@object))
                foldouts.Add(@object, false);
            else
                result = foldouts[@object];
            result            = EditorGUILayout.Foldout(result, text, true);
            foldouts[@object] = result;
            return result;
        }

        private static readonly Dictionary<object, bool> foldouts2 = new Dictionary<object, bool>();

        public static bool ShowFold(string text, object @object) {
            var result = false;
            if (!foldouts2.ContainsKey(@object))
                foldouts2.Add(@object, false);
            else
                result = foldouts2[@object];
            result             = EditorGUILayout.Foldout(result, text, true);
            foldouts2[@object] = result;
            return result;
        }

        #endregion

        #region Отображение полей

        /// <summary>
        ///     Compact view Vector3 in EditorWindow
        /// </summary>
        /// <param name="prop">Property Vector3</param>
        /// <param name="width">Total width</param>
        /// <param name="gui">Custom label & hint</param>
        public static void ShowVector3(SerializedProperty prop, GUIContent gui, float width = 0f) {
            var lw = EditorGUIUtility.labelWidth;
            var v  = prop.vector3Value;
            var w  = GUILayout.Width(width > 0f ? width * 0.25f : 90f);
            GUILayout.BeginHorizontal();
            {
                SetLabelWidth(12f, true);
                if (gui == null) EditorGUILayout.LabelField(prop.name, w);
                else EditorGUILayout.LabelField(gui,                   w);
                v.x               = EditorGUILayout.FloatField("X", v.x, w);
                v.y               = EditorGUILayout.FloatField("Y", v.y, w);
                v.z               = EditorGUILayout.FloatField("Z", v.z, w);
                prop.vector3Value = v;
            }
            SetLabelWidth(lw, false);
            GUILayout.EndHorizontal();
        }

        public static void ShowVector3(SerializedProperty prop, float width = 0f) {
            ShowVector3(prop, null, width);
        }

        #endregion

        #region Mouse tools

        /// <summary>
        /// Invoke method after click (default: RMB)
        /// </summary>
        /// <param name="mouseButton">Mouse button</param>
        /// <returns>Is clicked</returns>
        public static bool CheckMouseClick(int mouseButton = 1) {
            var w = EditorWindow.mouseOverWindow;
            if (w == null) return false;
            var current = Event.current;
            if (!current.isMouse || current.button != mouseButton) return false;
            return GUILayoutUtility.GetLastRect().Contains(current.mousePosition);
        }

        /// <summary>
        /// Invoke method if mouse over element
        /// </summary>
        /// <returns>Is over element</returns>
        public static bool CheckMouseOver() {
            var w = EditorWindow.mouseOverWindow;
            if (w == null) return false;
            var current = Event.current;
            return GUILayoutUtility.GetLastRect().Contains(current.mousePosition);
        }

        #endregion

        /// <summary>
        ///     Отметка объекта как изменённого и отметка сцены как изменённой
        /// </summary>
        /// <param name="object"></param>
        public static void MarkAsDirty(SerializedObject @object = null) {
            if (@object != null)
                @object.ApplyModifiedProperties();
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
    }
}
#endif