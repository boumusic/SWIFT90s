using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
public class EditorCreator : Editor
{
    public static string objectName;

    [MenuItem("Assets/Create Editor Script", false)]
    public static void CreateEditor()
    {
        Object obj = Selection.activeObject;
        objectName = obj.name;

        //string path = AssetDatabase.GetAssetPath(obj).Replace(objectName, EditorName());
        string path = Application.dataPath + "/Scripts/Editor/" + EditorName() + ".cs";

        string text = "";
        text =  "using System.Collections;\n" +
                "using System.Collections.Generic;\n" +
                "using UnityEngine;\n" +
                "using UnityEditor;\n" +
                "\n" +

                "[CustomEditor(typeof("+ objectName + "))]\n" +
                "public class " + EditorName() + " : Editor\n" +
                "{\n" +
                    "\tprivate " + objectName + " t;\n\n" +

                "\tprivate void OnEnable()\n" +
                "\t{\n" +
                    "\t\tt = target as " + objectName + ";\n" +
                "\t}\n\n" +

                "\tpublic override void OnInspectorGUI()\n" +
                "\t{\n" +
                    "\t\tEditorGUI.BeginChangeCheck();\n" +
                    "\t\tbase.OnInspectorGUI();\n" +
                    "\t\tserializedObject.ApplyModifiedProperties();\n" +

                    "\t\tif (EditorGUI.EndChangeCheck())\n" +
                    "\t\t{\n" +
                        "\t\t\tEditorUtility.SetDirty(t);\n" +
                    "\t\t}\n" +
                "\t}\n" +
            "}\n";

        File.WriteAllText(path, text);

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    // Note that we pass the same path, and also pass "true" to the second argument.
    [MenuItem("Assets/Create Editor Script", true)]
    private static bool Validation()
    {
        return Selection.activeObject is MonoScript;
    }

    public static string EditorName()
    {
        return objectName + "Editor";
    }
}
