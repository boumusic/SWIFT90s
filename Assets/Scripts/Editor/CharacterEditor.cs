using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Character))]
public class CharacterEditor : Editor
{
	private Character t;

	private void OnEnable()
	{
		t = target as Character;
	}

	public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();

        Debug();

        serializedObject.ApplyModifiedProperties();
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(t);
        }
    }

    private void Debug()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("DEBUG", EditorStyles.boldLabel);

        Label("Current State", t.CurrentState.ToString());
        Label("Velocity", t.Velocity.ToString());
        Label("Body Velocity", t.body.velocity.ToString());
        Label("Horizontal Axis", t.HorizontalAxis.ToString());
        Label("Horizontal Acceleration", t.XAccel.ToString());

        EditorGUILayout.EndVertical();
    }

    private void Label(string name, string value)
    {
        EditorGUILayout.LabelField(name + ":" + value);
    }
}
