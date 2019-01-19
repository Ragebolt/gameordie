using UnityEngine;
using UnityEngine.AI;

public static class GizmoTool {
    public static Vector3 gizmoCubeBig        = new Vector3(0.4f, 0.4f, 0.4f);
    public static Vector3 gizmoCubeMed        = new Vector3(0.3f, 0.3f, 0.3f);
    public static Vector3 gizmoCubeSmall      = new Vector3(0.2f, 0.2f, 0.2f);
    public static Vector3 gizmoCubeSmallest   = new Vector3(0.1f, 0.1f, 0.1f);
    public static float   gizmoSphereBig      = 0.15f;
    public static float   gizmoSphereMed      = 0.11f;
    public static float   gizmoSphereSmall    = 0.07f;
    public static float   gizmoSphereSmallest = 0.035f;

    public static void SetHashColor(Object @object) {
        Random.InitState(@object.GetHashCode());
        var c = new Color(Random.Range(0, 3) / 2f, Random.Range(0, 3) / 2f, Random.Range(0, 3) / 2f, 1f);
        Gizmos.color = c;
    }

    public static void DrawLink(Vector3 first, Vector3 second, float arrowSize = 0.15f) {
        var dist = Vector3.Distance(first, second);
        var dir = second - first;
        DrawArrow(first, dir, Vector3.up, dist, arrowSize);
        DrawArrow(second, -dir, Vector3.up, dist, arrowSize);
    }

    public static void DrawSimpleLink(Vector3 first, Vector3 second, float size) {
        Gizmos.DrawSphere(first, size);
        Gizmos.DrawSphere(second, size);
        Gizmos.DrawLine(first, second);
    }

    public static void DrawArrow(Vector3 start, Quaternion direction, Vector3 up, float length = 1f, float arrowSize = 0.15f) {
        var v = direction * Vector3.forward;
        DrawArrow(start, v, up, length, arrowSize);
    }

    public static void DrawArrow(Vector3 start, Vector3 direction, Vector3 up, float length = 1f, float arrowSize = 0.15f) {
        var a = direction.normalized * length;
        var arrowEnd = start + a;
        var arrowUp = up.normalized * arrowSize;
        var arrowLeft = Vector3.Cross(direction, up).normalized * arrowSize;
        Gizmos.DrawSphere(start, gizmoSphereSmallest);
        Gizmos.DrawLine(start, arrowEnd);
        Gizmos.DrawLine(arrowEnd, arrowEnd + arrowUp);
        Gizmos.DrawLine(arrowEnd, arrowEnd - arrowUp);
        Gizmos.DrawLine(arrowEnd, arrowEnd + arrowLeft);
        Gizmos.DrawLine(arrowEnd, arrowEnd - arrowLeft);
    }
}