using EasyEditorGUI;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using Generation;

[CustomEditor(typeof(SpawnPattern))]
public class SpawnPatternEditor : Editor {
    private       SpawnPattern       spawnPattern;
    private       SerializedProperty objectsArray;
    private       ReorderableList    reorderableList;
    private       float              elementHeight;
    public static Generator          generator;
    private       SerializedObject   thisSO;
    private List<float> listElementsHeight;   

    private static float lengthAngleRays    = 1f;
    private static int   howMany            = 10;
    private static bool  showEveryDirection = true;

    private SerializedProperty dataP;

    private const int       GIZMO_HANDLE_OFFSET = 10000;
    private       Vector3   scale;
    private       Vector3   pos;
    private       float[]   rotV    = new float[3];
    private       Vector3[] rot     = new Vector3[3];
    private       int       current = -1;
    private       Vector3   sizeWorld;

    private int                   focused = -1;
    private SceneView.OnSceneFunc onSceneFunc;


    private void OnEnable()
    {
        thisSO                                =  new SerializedObject(this);
        spawnPattern                          =  (SpawnPattern) target;
        objectsArray                          =  serializedObject.FindProperty("objects");
        reorderableList                       =  new ReorderableList(serializedObject, objectsArray);
        reorderableList.drawElementCallback   =  DrawElement;
        reorderableList.elementHeightCallback =  (int  index) => { return listElementsHeight[index]; };
        reorderableList.drawHeaderCallback    += (Rect rect) => { EditorGUI.LabelField(rect, "Objects"); };
        onSceneFunc                           =  sceneView => OnSceneGUI();
        SceneView.onSceneGUIDelegate          += onSceneFunc;
    }

    private void OnDisable() {
        SceneView.onSceneGUIDelegate -= onSceneFunc;
    }

    public override void OnInspectorGUI()
    {
        GUI.enabled = true;

        thisSO.Update();

        generator = (Generator) EditorGUILayout.ObjectField(generator, typeof(Generator), true);

        lengthAngleRays = EditorGUILayout.Slider("Length", lengthAngleRays, 0.5f, 3f);
        howMany         = EditorGUILayout.IntSlider("###", howMany, 2, 15);
        showEveryDirection = EditorGUILayout.Toggle("Show every direction", showEveryDirection);

        thisSO.ApplyModifiedProperties();

        if (generator == null) {
            focused = -1;
            return;
        }
        sizeWorld = generator.roomSize.GetVector3();

        serializedObject.Update();
        if (listElementsHeight == null) listElementsHeight = new List<float>();
        for (int i = listElementsHeight.Count; i < objectsArray.arraySize; i++)
        {
            listElementsHeight.Add(EditorGUIUtility.singleLineHeight * 10f);
        }
        reorderableList.DoLayoutList(); 
        foreach (var obj in spawnPattern.objects) {
            obj.spawnRect.position = new Vector2Int(Mathf.Clamp(obj.spawnRect.position.x, 0, generator.roomSize.x), Mathf.Clamp(obj.spawnRect.position.y, 0, generator.roomSize.y));
            obj.spawnRect.size = new Vector2Int(Mathf.Clamp(obj.spawnRect.size.x, 0, generator.roomSize.x - obj.spawnRect.position.x),
                                                Mathf.Clamp(obj.spawnRect.size.y, 0, generator.roomSize.y - obj.spawnRect.position.y));
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawElement(Rect rect, int index, bool isActive, bool isFocused) {
        if (isFocused) {
            if (index != focused) current = -1;
            this.focused = index;
        }
        const float fieldsOffset = 2f;
        Rect startRect = rect;
        float elementHeight = EditorGUIUtility.singleLineHeight * 10f;
        rect.height = EditorGUIUtility.singleLineHeight;



       
        SerializedProperty item = objectsArray.GetArrayElementAtIndex(index);

        var prefabData = item.FindPropertyRelative("prefabData");
        EditorGUI.PropertyField(rect, prefabData);


        rect.y      += rect.height * 2 + fieldsOffset;
        rect.height =  EditorGUIUtility.singleLineHeight * 3f;
        rect.width  -= rect.height;



        var spawnRect = item.FindPropertyRelative("spawnRect");
        EditorGUI.PropertyField(rect, spawnRect);

        var textureRect = new Rect(eGUI.indentWidth - rect.height - 5f, rect.y, rect.height, rect.height);
        EditorGUI.DrawTextureTransparent(textureRect, SpawnPatternEditor.DrawRectangle(spawnRect.rectIntValue));
        rect.y += rect.height;
        rect.width += rect.height;

        rect.height = EditorGUIUtility.singleLineHeight;
        var spMin = item.FindPropertyRelative("minRotation");
        var spMax = item.FindPropertyRelative("maxRotation");

        GUILayout.BeginHorizontal();
        {
            rect.width *= 0.5f;
            EditorGUI.PropertyField(rect, spMin);
            rect.x += rect.width;
            EditorGUI.PropertyField(rect, spMax);
            rect.x -= rect.width;
            rect.width *= 2f;
        }
        GUILayout.EndHorizontal();
        rect.y += rect.height;

        float min = spMin.floatValue;
        float max = spMax.floatValue;
        EditorGUI.MinMaxSlider(rect, "Rotation", ref min, ref max, -360f, 360f);
        var f = max - min;
        if (f > 360f)
        {
            f -= 360f;
            max -= f;
        }
        spMin.floatValue = min;
        spMax.floatValue = max;
        rect.y += rect.height;


        var spawnChance = item.FindPropertyRelative("spawnChance");
        EditorGUI.Slider(rect, spawnChance, 0f, 1f);


        rect.y += EditorGUIUtility.singleLineHeight;
        rect.height = EditorGUIUtility.singleLineHeight;
        rect.width = 100f;

        EditorGUI.PrefixLabel(rect, new GUIContent("Custom Config"));
        rect.x += 100f;
        rect.width = 50f;
        var useCustomConfig = item.FindPropertyRelative("useCustomConfig");
        useCustomConfig.boolValue = EditorGUI.Toggle(rect, useCustomConfig.boolValue);


        IConfig configurable = Selection.activeGameObject?.GetComponent<IConfig>();

        if (!useCustomConfig.boolValue || configurable == null) GUI.enabled = false;


        rect.x += 50f;
        rect.width = 200f;

        if (GUI.Button(rect, "Take config from selected"))
        {
            spawnPattern.objects[index].config = configurable.GetConfig();
        }

        rect.x = startRect.x;
        rect.width = startRect.width;
        rect.y += EditorGUIUtility.singleLineHeight;

        if (configurable != null)
        {
            EditorGUI.PropertyField(rect, configurable.GetProperty(), true);
            configurable.ApplyProperty();

            elementHeight += EditorGUI.GetPropertyHeight(configurable.GetProperty(), true);
        }

        GUI.enabled = true;

        listElementsHeight[index] = elementHeight;

        //var spawnRect = item.FindPropertyRelative("spawnRect");
        //EditorGUI.PropertyField(rect, spawnRect);

        //var textureRect = new Rect(eGUI.indentWidth - rect.height - 5f, rect.y, rect.height, rect.height);
        //EditorGUI.DrawTextureTransparent(textureRect, DrawRectangle(spawnPattern.objects[index].spawnRect));
        //rect.y     += rect.height + fieldsOffset;
        //rect.width += rect.height;

        //rect.height = EditorGUIUtility.singleLineHeight;
        //var spMin = item.FindPropertyRelative("minRotation");
        //var spMax = item.FindPropertyRelative("maxRotation");

        //GUILayout.BeginHorizontal();
        //{
        //    rect.width *= 0.5f;
        //    EditorGUI.PropertyField(rect, spMin);
        //    rect.x += rect.width; 
        //    EditorGUI.PropertyField(rect, spMax);
        //    rect.x -= rect.width;
        //    rect.width *= 2f;
        //}
        //GUILayout.EndHorizontal();
        //rect.y += rect.height + fieldsOffset;

        //EditorGUI.MinMaxSlider(rect, "Rotation", ref spawnPattern.objects[index].minRotation, ref spawnPattern.objects[index].maxRotation, -360f, 360f);
        //var f = spawnPattern.objects[index].maxRotation - spawnPattern.objects[index].minRotation;
        //if (f > 360f) {
        //    f                                       -= 360f;
        //    spawnPattern.objects[index].maxRotation -= f;
        //}
        //rect.y += rect.height + fieldsOffset;

        //var spawnChance = item.FindPropertyRelative("spawnChance");
        //EditorGUI.Slider(rect, spawnChance, 0f, 1f);
    }

    public static Texture2D DrawRectangle(RectInt rect) {
        var texture = new Texture2D(100, 100);
        for (var x = (int) ((float) rect.xMin / generator.roomSize.x * 100);
             x < (float) rect.xMax / generator.roomSize.x * 100;
             x++)
            for (var y = (int) ((float) rect.yMin / generator.roomSize.y * 100);
                 y < (float) rect.yMax / generator.roomSize.y * 100;
                 y++)
                texture.SetPixel(x, y, Color.black);
        var centerDotSize = 5f;
        for (var x = Mathf.Clamp((int) ((float) rect.x / generator.roomSize.x * 100 - centerDotSize), 0, 100);
             x < Mathf.Clamp((float) rect.x / generator.roomSize.x * 100 + centerDotSize, 0, 100);
             x++)
            for (var y = Mathf.Clamp((int) ((float) rect.y / generator.roomSize.y * 100 - centerDotSize), 0, 100);
                 y < Mathf.Clamp((float) rect.y * 100 / generator.roomSize.y + centerDotSize, 0, 100);
                 y++)
                texture.SetPixel(x, y, Color.red);
        texture.Apply();
        return texture;
    }

    private void OnSceneGUI() {
        if (generator == null) return;
        DrawRoomBorder();
        var v = Vector3.zero;
        for (var i = 0; i < objectsArray.arraySize; i++) {
            dataP = objectsArray.GetArrayElementAtIndex(i);
            var data2 = dataP.FindPropertyRelative("spawnRect").rectIntValue;
            scale   = data2.VectorSize();
            pos     = data2.position.GetVector3() - generator.roomSize.GetVector3() * 0.5f;
            rotV[0] = dataP.FindPropertyRelative("minRotation").floatValue;
            rotV[2] = dataP.FindPropertyRelative("maxRotation").floatValue;
            rotV[1] = Mathf.Lerp(rotV[0], rotV[2], 0.5f);
            var c1 = Color.gray;
            var c2 = Color.gray;
            if (i == focused && focused >= 0) {
                c1 = Color.white;
                c2 = new Color(0f, 1f, 1f, 0.25f);
                DrawSelected(i, c1, c2, true);
                continue;
            }
            DrawSelected(i, c1, c2, false);
        }
    }

    private void DrawRoomBorder() {
        Handles.color = Color.red;
        Handles.DrawWireCube(Vector3.zero, generator.roomSize.GetVector3());
    }

    private void DrawSelected(int index, Color colorArea, Color colorDirection, bool showControl) {
        var data = new RoomObject() {
                                                     spawnRect   = dataP.FindPropertyRelative("spawnRect").rectIntValue,
                                                     minRotation = dataP.FindPropertyRelative("minRotation").floatValue,
                                                     maxRotation = dataP.FindPropertyRelative("maxRotation").floatValue
                                                 };
        Vector3 sizeArea  = data.spawnRect.size.GetVector3();
        var     shiftVert = new Vector3(0f,    0.25f, 0f);
        var     shitHorz  = new Vector3(0.25f, 0.0f,  0f);
        var     centerArc = pos + (Vector3) (sizeArea * 0.5f);

        DrawArea(data.spawnRect, colorArea);
        if (showEveryDirection)
            DrawRotationMulti(pos + shiftVert * 2f + shitHorz * 2f, rotV[0], rotV[2], colorDirection, data.spawnRect.size);
        else 
            DrawRotation(centerArc, rotV[0], rotV[2], colorDirection);

        if (showControl) {
            var ev = Event.current;
            if (ev.button == 0 && ev.type == EventType.MouseDown
                               && ev.control == false && ev.shift == false && ev.alt == false) { }

            var id = GUIUtility.hotControl - GIZMO_HANDLE_OFFSET;
            if (id >= 0 && id <= 5) {
                current = id;
            }

            #region Position 

            Handles.color = Color.green;
            var posT = pos + shiftVert * 1f + shitHorz * 1f;
            Handles.FreeMoveHandle(GIZMO_HANDLE_OFFSET + 4, posT, Quaternion.identity, HandleUtility.GetHandleSize(pos) * 0.2f, Vector3.zero, Handles.CubeHandleCap);
            if (current == 4) {
                pos                     = Handles.PositionHandle(posT, Quaternion.identity);
                pos.x                   = Mathf.Clamp(pos.x, -sizeWorld.x * 0.5f, sizeWorld.x * 0.5f - sizeArea.x);
                pos.y                   = Mathf.Clamp(pos.y, -sizeWorld.y * 0.5f, sizeWorld.y * 0.5f - sizeArea.y);
                data.spawnRect.position = (pos + sizeWorld * 0.5f).GetVector2Int();
            }

            #endregion

            #region Size

            Handles.color = Color.blue;
            posT          = pos + shiftVert * 2f + shitHorz * 2f;
            Handles.FreeMoveHandle(GIZMO_HANDLE_OFFSET + 5, posT, Quaternion.identity, HandleUtility.GetHandleSize(pos) * 0.2f, Vector3.zero, Handles.SphereHandleCap);
            if (current == 5) {
                scale                 = Handles.ScaleHandle(scale, posT, Quaternion.identity, HandleUtility.GetHandleSize(pos) * 1f);
                scale.x               = Mathf.Clamp(scale.x, 0f, generator.roomSize.x - data.spawnRect.position.x);
                scale.y               = Mathf.Clamp(scale.y, 0f, generator.roomSize.y - data.spawnRect.position.y);
                data.spawnRect.width  = (int) scale.x;
                data.spawnRect.height = (int) scale.y;
            }

            #endregion

            #region Rotation

            colorDirection.a = 1f;
            DrawRotation(centerArc, rotV[0], rotV[2], colorDirection);

            Handles.color = Color.white;
            for (var i = 0; i < 3; i++) {
                rot[i] = centerArc + pos.GetAngleVector3(rotV[i]) * lengthAngleRays;
                Handles.FreeMoveHandle(GIZMO_HANDLE_OFFSET + i, rot[i], Quaternion.identity, HandleUtility.GetHandleSize(rot[i]) * 0.1f, Vector3.zero, Handles.SphereHandleCap);
                if (current == i) {
                    rot[i]   = Handles.PositionHandle(rot[i], Quaternion.identity);
                    rot[i].z = centerArc.z;
                    rot[i]   = centerArc + (rot[i] - centerArc).normalized * lengthAngleRays;
                    rot[i]   = rot[i] - centerArc;
                    rotV[i]  = -Vector3.SignedAngle(Vector3.up, rot[i], Vector3.forward);
                    if (i == 2)
                        if (rotV[i] < 0f)
                            rotV[i] += 360f;
                    if (i == 1) {
                        var d = rotV[2] - rotV[0];
                        rotV[0] = rotV[1] - d / 2f;
                        rotV[2] = rotV[1] + d / 2f;
                    }
                }
            }

            #endregion

            #region Apply changes

            dataP.FindPropertyRelative("spawnRect").rectIntValue = data.spawnRect;
            dataP.FindPropertyRelative("minRotation").floatValue = rotV[0];
            dataP.FindPropertyRelative("maxRotation").floatValue = rotV[2];
            serializedObject.ApplyModifiedProperties();

            #endregion
        }
    }

    private void DrawArea(RectInt rect, Color color) {
        Handles.color = color;
        Handles.DrawWireCube((Vector3) rect.VectorCenter() - sizeWorld * 0.5f, new Vector3(rect.width, rect.height, 0f));
    }

    private void DrawRotationMulti(Vector3 centerArc, float min, float max, Color color, Vector2Int size) {
        Handles.color = color;
        var v = Vector3.zero;
        for (int y = 0; y < size.y; y++)
            for (int x = 0; x < size.x; x++) {
                var dev = new Vector3(x, y, 0f);
                for (var i = 0.0f; i < lengthAngleRays; i += lengthAngleRays / (float) howMany) {
                    Handles.DrawWireArc(centerArc + dev, Vector3.back, v.GetAngleVector3(rotV[0]), rotV[2] - rotV[0], i);
                }
                Handles.DrawWireArc(centerArc + dev, Vector3.back, v.GetAngleVector3(rotV[0]), rotV[2] - rotV[0], lengthAngleRays);
                Handles.DrawDottedLine(centerArc + dev + v.GetAngleVector3(rotV[0]) * lengthAngleRays * 1.5f, centerArc + dev, HandleUtility.GetHandleSize(centerArc) * 2f);
                Handles.DrawDottedLine(centerArc + dev + v.GetAngleVector3(rotV[2]) * lengthAngleRays * 1.5f, centerArc + dev, HandleUtility.GetHandleSize(centerArc) * 2f);
            }
    }

    private void DrawRotation(Vector3 centerArc, float min, float max, Color color) {
        Handles.color = color;
        var v = Vector3.zero;
        for (var i = 0.0f; i < lengthAngleRays; i += lengthAngleRays / (float) howMany) {
            Handles.DrawWireArc(centerArc, Vector3.back, v.GetAngleVector3(rotV[0]), rotV[2] - rotV[0], i);
        }
        Handles.DrawWireArc(centerArc, Vector3.back, v.GetAngleVector3(rotV[0]), rotV[2] - rotV[0], lengthAngleRays);
        Handles.DrawDottedLine(centerArc + v.GetAngleVector3(rotV[0]) * lengthAngleRays * 1.5f, centerArc, HandleUtility.GetHandleSize(centerArc) * 2f);
        Handles.DrawDottedLine(centerArc + v.GetAngleVector3(rotV[2]) * lengthAngleRays * 1.5f, centerArc, HandleUtility.GetHandleSize(centerArc) * 2f);
    }
}