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
    private       SerializedObject   thisSO;
    private List<float> listElementsHeight;   

    private static float lengthAngleRays    = 1f;
    private static int   howMany            = 5;
    private static bool  showEveryDirection = false;
    private static Vector2 editorNormalisedRoomSize = new Vector2(10f, 10f);
    private static bool  showEditorSettings = false;
    private static bool  isNormalisedMode   = true;

    private SerializedProperty dataP;

    private const int       GIZMO_HANDLE_OFFSET = 10000;
    private       Vector2   scale;
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

        //generator = (Generator) EditorGUILayout.ObjectField(generator, typeof(Generator), true);

        string buttonText = showEditorSettings ? "Hide editor settings" : "Show editor settings";
        if (GUILayout.Button(buttonText)) showEditorSettings = !showEditorSettings;

        if (showEditorSettings)
        {
            lengthAngleRays = EditorGUILayout.Slider("Length", lengthAngleRays, 0.5f, 3f);
            howMany = EditorGUILayout.IntSlider("###", howMany, 2, 15);
            showEveryDirection = EditorGUILayout.Toggle("Show every direction", showEveryDirection);
            //editorNormalisedRoomSize = EditorGUILayout.Vector2Field("Editor Normalised Room Size", editorNormalisedRoomSize);
        }

        GUILayout.BeginHorizontal();
        var leftButtonStyle = new GUIStyle(EditorStyles.miniButtonLeft); if (isNormalisedMode) leftButtonStyle.normal = leftButtonStyle.active;
        if (GUILayout.Button("Normalised Mode", leftButtonStyle)) isNormalisedMode = true;

        var rightButtonStyle = new GUIStyle(EditorStyles.miniButtonRight); if (!isNormalisedMode) rightButtonStyle.normal = rightButtonStyle.active;
        if (GUILayout.Button("Integer Mode", rightButtonStyle)) isNormalisedMode = false;
        GUILayout.EndHorizontal();

        thisSO.ApplyModifiedProperties();


        serializedObject.Update();

        //if (!isNormalisedMode)
        EditorGUILayout.PropertyField(serializedObject.FindProperty("editorRoomSize"), new GUIContent("Editor room size"));

        sizeWorld = (Vector2)spawnPattern.editorRoomSize;

        var sizes = serializedObject.FindProperty("sizes");
        EditorGUILayout.PropertyField(sizes, new GUIContent("Generator room sizes"), true);

        if (listElementsHeight == null) listElementsHeight = new List<float>();
        for (int i = listElementsHeight.Count; i < objectsArray.arraySize; i++)
        {
            listElementsHeight.Add(EditorGUIUtility.singleLineHeight * 10f);
        }
        reorderableList.DoLayoutList(); 
        foreach (var obj in spawnPattern.objects)
        {
            obj.spawnRect.position = new Vector2(Mathf.Clamp01(obj.spawnRect.position.x), Mathf.Clamp01(obj.spawnRect.position.y));
            obj.spawnRect.size = new Vector2(Mathf.Clamp(obj.spawnRect.size.x, 0f, 1f - obj.spawnRect.position.x),
                                                Mathf.Clamp(obj.spawnRect.size.y, 0f, 1f - obj.spawnRect.position.y));
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
    {
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


        Rect spawnRectValue;
        if (isNormalisedMode)
        {
            var spawnRect = item.FindPropertyRelative("spawnRect");
            EditorGUI.PropertyField(rect, spawnRect);
            spawnRectValue = spawnRect.rectValue;
        }
        else
        {
            var obj = spawnPattern.objects[index];
            var spawnRectInt = new RectInt(Vector2Int.RoundToInt(obj.spawnRect.position * sizeWorld), Vector2Int.RoundToInt(obj.spawnRect.size * sizeWorld));
            spawnRectInt = EditorGUI.RectIntField(rect, new GUIContent("Spawn Rect"), spawnRectInt);
            obj.spawnRect = new Rect((Vector2)spawnRectInt.VectorPos() / sizeWorld, (Vector2)spawnRectInt.VectorSize() / sizeWorld);
            spawnRectValue = obj.spawnRect;
        }

        rect.height = EditorGUIUtility.singleLineHeight;
        rect.y += EditorGUIUtility.singleLineHeight * 2f;
        var snapToGrid = item.FindPropertyRelative("snapToGrid");
        EditorGUI.PropertyField(rect, snapToGrid);
        rect.y -= EditorGUIUtility.singleLineHeight * 2f;
        rect.height = EditorGUIUtility.singleLineHeight * 3f;

        var textureRect = new Rect(eGUI.indentWidth - rect.height - 5f, rect.y, rect.height, rect.height);
        EditorGUI.DrawTextureTransparent(textureRect, SpawnPatternEditor.DrawRectangle(spawnRectValue));

        rect.y += rect.height + 10f;
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

        listElementsHeight[index] = elementHeight;
    }

    public static Texture2D DrawRectangle(Rect rect) {
        var texture = new Texture2D(100, 100);
        for (var x = (int) ((float) rect.xMin * 100);
             x < (float) rect.xMax * 100;
             x++)
            for (var y = (int) ((float) rect.yMin * 100);
                 y < (float) rect.yMax * 100;
                 y++)
                texture.SetPixel(x, y, Color.black);
        var centerDotSize = 5f;
        for (var x = Mathf.Clamp((int) ((float) rect.x * 100 - centerDotSize), 0, 100);
             x < Mathf.Clamp((float) rect.x * 100 + centerDotSize, 0, 100);
             x++)
            for (var y = Mathf.Clamp((int) ((float) rect.y * 100 - centerDotSize), 0, 100);
                 y < Mathf.Clamp((float) rect.y * 100 + centerDotSize, 0, 100);
                 y++)
                texture.SetPixel(x, y, Color.red);
        texture.Apply();
        return texture;
    }

    private void OnSceneGUI() {
        //if (generator == null) return;
        DrawRoomBorder();
        var v = Vector3.zero;
        for (var i = 0; i < objectsArray.arraySize; i++) {
            dataP = objectsArray.GetArrayElementAtIndex(i);
            var data2 = dataP.FindPropertyRelative("spawnRect").rectValue;
            scale   = data2.size * sizeWorld;
            pos     = (Vector2)(data2.position - Vector2.one * 0.5f) * sizeWorld;
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
        Handles.DrawWireCube(Vector3.zero, sizeWorld);
    }

    private void DrawSelected(int index, Color colorArea, Color colorDirection, bool showControl) {
        var data = new RoomObject() {
                                                     spawnRect   = dataP.FindPropertyRelative("spawnRect").rectValue,
                                                     minRotation = dataP.FindPropertyRelative("minRotation").floatValue,
                                                     maxRotation = dataP.FindPropertyRelative("maxRotation").floatValue
                                                 };
        Vector3 sizeArea  = data.spawnRect.size;
        var     shiftVert = new Vector3(0f,    0.25f, 0f);
        var     shitHorz  = new Vector3(0.25f, 0.0f,  0f);
        var     centerArc = pos + (Vector3) (sizeArea * 0.5f);

        DrawArea(data.spawnRect, colorArea);
        if (showEveryDirection)
            DrawRotationMulti(pos + shiftVert * 2f + shitHorz * 2f, rotV[0], rotV[2], colorDirection, data.spawnRect.size * sizeWorld);
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

            Debug.Log("1 " + pos);
            Handles.color = Color.green;
            var posT = pos + shiftVert * 0f + shitHorz * 0f;
            Handles.FreeMoveHandle(GIZMO_HANDLE_OFFSET + 4, posT, Quaternion.identity, HandleUtility.GetHandleSize(pos) * 0.2f, Vector3.zero, Handles.CubeHandleCap);
            if (current == 4) {
                pos                     = Handles.PositionHandle(posT, Quaternion.identity);
                pos -= shiftVert * 0f + shitHorz * 0f;
                pos.x                   = Mathf.Clamp(pos.x, -sizeWorld.x * 0.5f, sizeWorld.x * 0.5f - sizeArea.x);
                pos.y                   = Mathf.Clamp(pos.y, -sizeWorld.y * 0.5f, sizeWorld.y * 0.5f - sizeArea.y);
                //pos                    += Vector3.one * 0.5f;
                data.spawnRect.position = ((Vector2)pos / sizeWorld) + Vector2.one * 0.5f;
                Debug.Log("2 " + pos);
            } 


            #endregion

            #region Size

            Handles.color = Color.blue;
            posT          = pos + shiftVert * 2f + shitHorz * 2f;
            Handles.FreeMoveHandle(GIZMO_HANDLE_OFFSET + 5, posT, Quaternion.identity, HandleUtility.GetHandleSize(pos) * 0.2f, Vector3.zero, Handles.SphereHandleCap);
            if (current == 5) {
                scale                 = Handles.ScaleHandle(scale, posT, Quaternion.identity, HandleUtility.GetHandleSize(pos) * 1f);
                scale.x               = Mathf.Clamp(scale.x, 0f, (1f - data.spawnRect.position.x) * sizeWorld.x);
                scale.y               = Mathf.Clamp(scale.y, 0f, (1f - data.spawnRect.position.y) * sizeWorld.y);
                data.spawnRect.size   = scale / sizeWorld;
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

            dataP.FindPropertyRelative("spawnRect").rectValue = data.spawnRect;
            dataP.FindPropertyRelative("minRotation").floatValue = rotV[0];
            dataP.FindPropertyRelative("maxRotation").floatValue = rotV[2];
            serializedObject.ApplyModifiedProperties();

            #endregion
        }
    }

    private void DrawArea(Rect rect, Color color) {
        Handles.color = color;
        Handles.DrawWireCube((rect.center - Vector2.one * 0.5f) * sizeWorld, rect.size * sizeWorld);
    }

    private void DrawRotationMulti(Vector3 centerArc, float min, float max, Color color, Vector2 size) {
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