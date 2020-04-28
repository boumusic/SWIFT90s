using UnityEngine;
using UnityEditor;

namespace Pataya.QuikFX
{
    public class PackedVectorDrawer : MaterialPropertyDrawer
    {
        protected string nameA;
        protected string nameB;
        protected bool twoRows;

        public PackedVectorDrawer()
        {
            nameA = "Vector A";
            nameB = "Vector B";
            twoRows = true;
        }

        public PackedVectorDrawer(string nameA, string nameB)
        {
            this.nameA = nameA;
            this.nameB = nameB;
            twoRows = true;
        }

        public PackedVectorDrawer(string nameA)
        {
            this.nameA = nameA;
            twoRows = false;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            EditorGUI.BeginChangeCheck();

            Vector2 XY = DrawVectorPacked(prop, true, nameA);

            if(twoRows)
            {
                Vector2 ZW = DrawVectorPacked(prop, false, nameB);
                prop.vectorValue = new Vector4(XY.x, XY.y, ZW.x, ZW.y);
            }

            else
            {
                prop.vectorValue = new Vector4(XY.x, XY.y, 0, 0);
            }            

            Apply(prop);
        }

        public static Vector2 DrawVectorPacked(MaterialProperty prop, bool xy, string name)
        {
            float currentLabelWidth = EditorGUIUtility.labelWidth;

            float mainLabelWidth = 150;
            float floatFieldWidth = 200;
            float floatLabelWidth = 10;

            float x = xy ? prop.vectorValue.x : prop.vectorValue.z;
            float y = xy ? prop.vectorValue.y : prop.vectorValue.w;

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(name, GUILayout.MaxWidth(mainLabelWidth));

            EditorGUIUtility.labelWidth = floatLabelWidth;
            float returnedX = EditorGUILayout.FloatField("X", x, GUILayout.MaxWidth(floatFieldWidth));
            float returnedY = EditorGUILayout.FloatField("Y", y, GUILayout.MaxWidth(floatFieldWidth));

            EditorGUIUtility.labelWidth = currentLabelWidth;

            EditorGUILayout.EndHorizontal();

            if (GUI.changed)
            {
                SceneView.RepaintAll();
            }

            return new Vector2(returnedX, returnedY);
        }
    }
}
