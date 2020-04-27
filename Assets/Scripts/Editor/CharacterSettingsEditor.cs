using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CharacterSettings))]
public class CharacterSettingsEditor : Editor
{
	private CharacterSettings t;

	private void OnEnable()
	{
		t = target as CharacterSettings;
	}

	public override void OnInspectorGUI()
	{
		EditorGUI.BeginChangeCheck();
		base.OnInspectorGUI();

        EditorGUILayout.BeginVertical("box");

        float height = 0;
        for (int i = 0; i < 100; i++)
        {
            float normalizedI = (float)i / 100;
            height += ((t.jumpCurve.Evaluate(normalizedI) * t.jumpStrength)) * (t.jumpDuration / 100);
        }

        EditorGUILayout.LabelField("Jump Height : " + height.ToString("F2") + " meters");

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
		if (EditorGUI.EndChangeCheck())
		{
			EditorUtility.SetDirty(t);
		}
	}
}
