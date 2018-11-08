using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
/*
[CustomEditor(typeof(SplineComponent))]
public class SplineComponentEditor : Editor {
    public override void OnInspectorGUI() {
        EditorGUILayout.HelpBox("Hold Shift and click to append and insert curve points. Backspace to delete points.", MessageType.Info);
        SplineComponent spline = target as SplineComponent;
        GUILayout.BeginHorizontal();
        /*bool closed = GUILayout.Toggle(spline.closed, "Closed", "button");
        if(spline.closed != closed) {
            spline.closed = closed;
            spline.ResetIndex();
        }/
        if(GUILayout.Button("Flatten Y Axis")) {
            Undo.RecordObject(target, "Flatten Y Axis");
            //TODO: Flatten(spline.points);
            spline.ResetIndex();
        }
        if(GUILayout.Button("Center around Origin")) {
            Undo.RecordObject(target, "Center around Origin");
            //TODO: CenterAroundOrigin(spline.points);
            spline.ResetIndex();
        }
        GUILayout.EndHorizontal();
    }

    [DrawGizmo(GizmoType.NonSelected)]
    static void DrawGizmosLoRes(SplineComponent spline, GizmoType gizmoType) {
        Gizmos.color = Color.white;
        DrawGizmo(spline, 64);
    }

    [DrawGizmo(GizmoType.Selected)]
    static void DrawGizmosHiRes(SplineComponent spline, GizmoType gizmoType) {
        Gizmos.color = Color.white;
        DrawGizmo(spline, 1024);
    }

    static void DrawGizmo(SplineComponent spline, int stepCount) {
        if(spline.points.Count > 0) {
            float P = 0f;
            Vector3 start = spline.GetNonUniformPoint(0);
            float step = 1f / stepCount;
            do {
                P += step;
                Vector3 here = spline.GetNonUniformPoint(P);
                Gizmos.DrawLine(start, here);
                start = here;
            } while(P + step <= 1);
        }
    }

    //Not sure what these are
    int hotIndex = -1;
    int removeIndex = -1;

    void OnSceneGUI() {
        SplineComponent spline = target as SplineComponent;
        Event e = Event.current;
        GUIUtility.GetControlID(FocusType.Passive);


        Vector2 mousePos = Event.current.mousePosition;
        Vector3 view = SceneView.currentDrawingSceneView.camera.ScreenToViewportPoint(Event.current.mousePosition);
        bool mouseIsOutside = view.x < 0 || view.x > 1 || view.y < 0 || view.y > 1;
        if(mouseIsOutside){
            return;
        }

        SerializedProperty points = serializedObject.FindProperty("points");
        if(Event.current.shift) {
           /* if(spline.closed) {
                ShowClosestPointOnClosedSpline(points);
            }
            else {/
                ShowClosestPointOnOpenSpline(points);
           // }
        }

        for(int i = 0; i < spline.points.Count; i++) {
            SerializedProperty prop = points.GetArrayElementAtIndex(i);
            Vector3 point = prop.vector3Value;
            Vector3 wp = spline.transform.TransformPoint(point);

            if(hotIndex == i) {
                Vector3 newWp = Handles.PositionHandle(wp, Tools.pivotRotation == PivotRotation.Global ? Quaternion.identity : spline.transform.rotation);
                Vector3 delta = spline.transform.InverseTransformDirection(newWp - wp);
                if(delta.sqrMagnitude > 0) {
                    prop.vector3Value = point + delta;
                    spline.ResetIndex();
                }
                HandleCommands(wp);
            }

            Handles.color = i == 0 | i == spline.points.Count - 1 ? Color.red : Color.white;
            float buttonSize = HandleUtility.GetHandleSize(wp) * 0.1f;
            if(Handles.Button(wp, Quaternion.identity, buttonSize, buttonSize, Handles.SphereHandleCap)) {
                hotIndex = i;
            }

            Vector3 v = SceneView.currentDrawingSceneView.camera.transform.InverseTransformPoint(wp);
            bool labelIsOutside = v.z < 0;
            if(!labelIsOutside) {
                Handles.Label(wp, i.ToString());
            }

            if(removeIndex >= 0 && points.arraySize > 4) {
                points.DeleteArrayElementAtIndex(removeIndex);
                spline.ResetIndex();
            }
        }
        removeIndex = -1;
        serializedObject.ApplyModifiedProperties();
    }

    void HandleCommands(Vector3 wp) {
        if(Event.current.type == EventType.ExecuteCommand) {
            if(Event.current.commandName == "FrameSelected") {
                SceneView.currentDrawingSceneView.Frame(new Bounds(wp, Vector3.one * 10), false);
                Event.current.Use();
            }
        }
        if(Event.current.type == EventType.KeyDown) {
            if(Event.current.keyCode == KeyCode.Backspace) {
                removeIndex = hotIndex;
                Event.current.Use();
            }
        }
    }

    void ShowClosestPointOnClosedSpline(SerializedProperty points) {
        SplineComponent spline = target as SplineComponent;
        Plane plane = new Plane(spline.transform.up, spline.transform.position);
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        float center;
        if(plane.Raycast(ray, out center)) {
            Vector3 hit = ray.origin + ray.direction * center;
            Handles.DrawWireDisc(hit, spline.transform.up, 5);
            float p = SearchForClosestPoint(Event.current.mousePosition);
            Vector3 sp = spline.GetNonUniformPoint(p);
            Handles.DrawLine(hit, sp);


            if(Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.shift) {
                int i = (Mathf.FloorToInt(p * spline.points.Count) + 2) % spline.points.Count;
                points.InsertArrayElementAtIndex(i);
                points.GetArrayElementAtIndex(i).vector3Value = spline.transform.InverseTransformPoint(sp);
                serializedObject.ApplyModifiedProperties();
                hotIndex = i;
            }
        }
    }


    void ShowClosestPointOnOpenSpline(SerializedProperty points) {
        SplineComponent spline = target as SplineComponent;
        Plane plane = new Plane(spline.transform.up, spline.transform.position);
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        float center;
        if(plane.Raycast(ray, out center)) {
            Vector3 hit = ray.origin + ray.direction * center;
            float discSize = HandleUtility.GetHandleSize(hit);
            Handles.DrawWireDisc(hit, spline.transform.up, discSize);
            float p = SearchForClosestPoint(Event.current.mousePosition);


            if((hit - spline.GetNonUniformPoint(0)).sqrMagnitude < 25)
                p = 0;
            if((hit - spline.GetNonUniformPoint(1)).sqrMagnitude < 25)
                p = 1;


            Vector3 sp = spline.GetNonUniformPoint(p);


            bool extend = Mathf.Approximately(p, 0) || Mathf.Approximately(p, 1);


            Handles.color = extend ? Color.red : Color.white;
            Handles.DrawLine(hit, sp);
            Handles.color = Color.white;


            int i = 1 + Mathf.FloorToInt(p * (spline.points.Count - 3));


            if(Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.shift) {
                if(extend) {
                    if(i == spline.points.Count - 2)
                        i++;
                    points.InsertArrayElementAtIndex(i);
                    points.GetArrayElementAtIndex(i).vector3Value = spline.transform.InverseTransformPoint(hit);
                    hotIndex = i;
                }
                else {
                    i++;
                    points.InsertArrayElementAtIndex(i);
                    points.GetArrayElementAtIndex(i).vector3Value = spline.transform.InverseTransformPoint(sp);
                    hotIndex = i;
                }
                serializedObject.ApplyModifiedProperties();
            }
        }
    }


    float SearchForClosestPoint(Vector2 screenPoint, float A = 0f, float B = 1f, float steps = 1000) {
        SplineComponent spline = target as SplineComponent;
        float smallestDelta = float.MaxValue;
        float step = (B - A) / steps;
        float closestI = A;
        for(var i = 0; i <= steps; i++) {
            Vector3 p = spline.GetNonUniformPoint(i * step);
            Vector2 gp = HandleUtility.WorldToGUIPoint(p);
            float delta = (screenPoint - gp).sqrMagnitude;
            if(delta < smallestDelta) {
                closestI = i;
                smallestDelta = delta;
            }
        }
        return closestI * step;
    }

    //Utility Methods, I hope this is where they go...
    void Flatten(List<Vector3> points) {
        for(int i = 0; i < points.Count; i++) {
            points[i] = Vector3.Scale(points[i], new Vector3(1, 0, 1));
        }
    }


    void CenterAroundOrigin(List<Vector3> points) {
        Vector3 center = Vector3.zero;
        for(int i = 0; i < points.Count; i++) {
            center += points[i];
        }
        center /= points.Count;
        for(int i = 0; i < points.Count; i++) {
            points[i] -= center;
        }
    }

}*/
