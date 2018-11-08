using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerMovement))]
[CanEditMultipleObjects]
public class PlayerMovementAttributeEditor : Editor {
    private void OnEnable() {
        
    }

    public override void OnInspectorGUI() {
        //base.OnInspectorGUI();
        PlayerMovement playerM = target as PlayerMovement;
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
            playerM.fluidMoveSpeedX = EditorGUILayout.Toggle("fluidMoveSpeedX", playerM.fluidMoveSpeedX);
        GUILayout.EndHorizontal();
        if(playerM.fluidMoveSpeedX) {
            GUILayout.BeginHorizontal();
                playerM.minFluidMoveSpeedX = EditorGUILayout.FloatField("minFluidMoveSpeedX", playerM.minFluidMoveSpeedX);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
                playerM.maxFluidMoveSpeedX = EditorGUILayout.FloatField("maxFluidMoveSpeedX", playerM.maxFluidMoveSpeedX);
            GUILayout.EndHorizontal();
        }
        else {
            if(playerM.moveSpeedsX != null) {
                GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Move Speeds X", GUIStyle.none);
                GUILayout.EndHorizontal();
                EditorGUI.indentLevel++;
                GUILayout.BeginHorizontal();
                    int size = playerM.moveSpeedsX.Length;
                    size = EditorGUILayout.IntField("Size", size);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                    if(size != playerM.moveSpeedsX.Length) {
                        List<float> listMoveSpeeds = new List<float>();
                        for(int i = 0; i < size; i++) {
                            if(size <= playerM.moveSpeedsX.Length) {
                                listMoveSpeeds.Add(playerM.moveSpeedsX[i]);
                            }
                        }
                        playerM.moveSpeedsX = listMoveSpeeds.ToArray();
                    }
                    for(int i = 0; i < playerM.moveSpeedsX.Length; i++) {
                        GUILayout.BeginHorizontal();
                            playerM.moveSpeedsX[i] = EditorGUILayout.FloatField("Element " + i.ToString(), playerM.moveSpeedsX[i]);
                        GUILayout.EndHorizontal();
                    }
                    EditorGUI.indentLevel--;
                GUILayout.EndHorizontal();
            }
            else {
                GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Move Speeds X", GUIStyle.none);
                GUILayout.EndHorizontal();
                EditorGUI.indentLevel++;
                GUILayout.BeginHorizontal();
                    int size = 1;
                    size = EditorGUILayout.IntField("Size", size);
                GUILayout.EndHorizontal();
                playerM.moveSpeedsX = new float[size];
                for(int i = 0; i < playerM.moveSpeedsX.Length; i++) {
                    GUILayout.BeginHorizontal();
                        playerM.moveSpeedsX[i] = EditorGUILayout.FloatField("Element " + i.ToString(), playerM.moveSpeedsX[i]);
                    GUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;

            }
            if(playerM.accelorationsX != null) {
                GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Accelorations X", GUIStyle.none);
                GUILayout.EndHorizontal();
                EditorGUI.indentLevel++;
                GUILayout.BeginHorizontal();
                    int size = playerM.accelorationsX.Length;
                    size = EditorGUILayout.IntField("Size", size);  //think I need to apply changes somewhere around here...
                GUILayout.EndHorizontal();
                if(size != playerM.accelorationsX.Length) {
                    List<float> listAccelorations = new List<float>();
                    for(int i = 0; i < size; i++) {
                        if(size <= playerM.accelorationsX.Length) {
                            listAccelorations.Add(playerM.moveSpeedsX[i]);
                        }
                    }
                    playerM.accelorationsX = listAccelorations.ToArray();
                }
                for(int i = 0; i < playerM.moveSpeedsX.Length; i++) {
                    GUILayout.BeginHorizontal();
                        playerM.accelorationsX[i] = EditorGUILayout.FloatField("Element " + i.ToString(), playerM.accelorationsX[i]);
                    GUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
                
            }
            else {
                GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Accelorations X", GUIStyle.none);
                GUILayout.EndHorizontal();
                EditorGUI.indentLevel++;
                GUILayout.BeginHorizontal();
                    int size = 1;
                    size = EditorGUILayout.IntField("Size", size);
                GUILayout.EndHorizontal();
                playerM.accelorationsX = new float[size];
                for(int i = 0; i < playerM.accelorationsX.Length; i++) {
                    GUILayout.BeginHorizontal();
                        playerM.accelorationsX[i] = EditorGUILayout.FloatField("Element " + i.ToString(), playerM.accelorationsX[i]);
                    GUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;

            }
            //jump variables next

        }
        
        GUILayout.EndVertical();
    }

}
