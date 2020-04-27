using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//https://486boy.blogspot.kr/2014/05/unity.html

[CustomEditor(typeof(EnviornmentManager))]
public class EnviornmentEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EnviornmentManager myScript = (EnviornmentManager)target;
        if (GUILayout.Button("Change selctedEnviornment"))
        {
            myScript.ChangeEnviornment();
        }
    }
}