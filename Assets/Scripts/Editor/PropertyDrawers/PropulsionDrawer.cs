using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomPropertyDrawer(typeof(Propulsion))]
public class PropulsionDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.isExpanded)
            return CustomEditorUtility.PropertyHeight(8);
        else
            return CustomEditorUtility.PropertyHeight(1);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Propulsion propulsion = fieldInfo.GetValue(property.serializedObject.targetObject) as Propulsion;
        property.isExpanded = EditorGUI.Foldout(CustomEditorUtility.GetPropertyRect(0, position), property.isExpanded, property.name);
        if (property.isExpanded)
        {
            CustomEditorUtility.QuickSerializeRelativeGUI("separateAxes", property, position, 1);
            float w = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 0.01f;
            if (propulsion.separateAxes)
            {
                CustomEditorUtility.QuickSerializeRelativeGUI("strengthHoriz", property, CustomEditorUtility.GetPropertyRect(2, position, 0, 0.5f, position.width));
                CustomEditorUtility.QuickSerializeRelativeGUI("curveHoriz", property, CustomEditorUtility.GetPropertyRect(2, position, 0.5f, 0.5f, position.width));

                CustomEditorUtility.QuickSerializeRelativeGUI("strengthVerti", property, CustomEditorUtility.GetPropertyRect(3, position, 0, 0.5f, position.width));
                CustomEditorUtility.QuickSerializeRelativeGUI("curveVerti", property, CustomEditorUtility.GetPropertyRect(3, position, 0.5f, 0.5f, position.width));
            }

            else
            {
                CustomEditorUtility.QuickSerializeRelativeGUI("strength", property, CustomEditorUtility.GetPropertyRect(2, position, 0, 0.5f, position.width));
                CustomEditorUtility.QuickSerializeRelativeGUI("curve", property, CustomEditorUtility.GetPropertyRect(2, position, 0.5f, 0.5f, position.width));
            }

            EditorGUIUtility.labelWidth = w;
            int start = propulsion.separateAxes ? 4 : 3;
            CustomEditorUtility.QuickSerializeRelativeGUI("duration", property, position, start, 0, 1, position.width);
            CustomEditorUtility.QuickSerializeRelativeGUI("airControl", property, position, start + 1, 0, 1, position.width);
            CustomEditorUtility.QuickSerializeRelativeGUI("direction", property, position, start + 2, 0, 1, position.width);
            CustomEditorUtility.QuickSerializeRelativeGUI("priority", property, position, start + 3, 0, 1, position.width);
        }

        property.serializedObject.ApplyModifiedProperties();
    }
}
