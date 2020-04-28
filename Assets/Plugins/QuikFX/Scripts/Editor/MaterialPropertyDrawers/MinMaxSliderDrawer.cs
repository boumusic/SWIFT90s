using UnityEngine;
using UnityEditor;

namespace Pataya.QuikFX
{
    public class MinMaxSliderDrawer : MaterialPropertyDrawer
    {
        protected float xLimit = -1;
        protected float yLimit = 1;
        protected float zLimit = -1;
        protected float wLimit = 1;
        protected float minMaxXY = 1;
        protected float minMaxZW = 1;
        protected bool isVector4 = false;
        protected string nameXY = "";
        protected string nameZW = "";
        protected string labelName;

        public MinMaxSliderDrawer()
        {
            isVector4 = false;
            nameXY = "";
            nameZW = "";
        }

        public MinMaxSliderDrawer(float xLimit, float yLimit, string labelName)
        {
            this.xLimit = xLimit;
            this.yLimit = yLimit;
            isVector4 = false;
            nameXY = "";
            nameZW = "";
            this.labelName = labelName;
        }

        public MinMaxSliderDrawer(float xLimit, float yLimit, float minMaxXY, string nameXY, float zLimit, float wLimit, float minMaxZW, string nameZW)
        {
            this.xLimit = xLimit;
            this.yLimit = yLimit;
            this.zLimit = zLimit;
            this.wLimit = wLimit;
            this.minMaxXY = minMaxXY;
            this.minMaxZW = minMaxZW;
            isVector4 = true;
            this.nameXY = nameXY;
            this.nameZW = nameZW;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, string label, MaterialEditor editor)
        {
            Undo.RecordObject(editor, "Edited material");
            EditorGUI.BeginChangeCheck();

            string labelXY = nameXY == "" ? labelName : nameXY;
            string labelZW = nameZW == "" ? label : nameZW;

            Vector2 vectorFieldXY = PackedVectorDrawer.DrawVectorPacked(prop, true, labelXY);
            if (minMaxXY == 1) DrawMinMaxSlider(ref vectorFieldXY, xLimit, yLimit, labelName != "" ? labelName : labelXY);

            Vector2 vectorFieldZW = new Vector2(0, 0);
            if (isVector4)
            {
                vectorFieldZW = PackedVectorDrawer.DrawVectorPacked(prop, false, labelZW);
                if(minMaxZW == 1) DrawMinMaxSlider(ref vectorFieldZW, zLimit, wLimit, labelZW);
            }

            prop.vectorValue = new Vector4(vectorFieldXY.x, vectorFieldXY.y, vectorFieldZW.x, vectorFieldZW.y);
            Apply(prop);
        }

        public static void DrawMinMaxSlider(ref Vector2 vectorField, float minLimit, float maxLimit, string label)
        {
            if (vectorField.x < minLimit) vectorField = new Vector2(minLimit, vectorField.y);
            if (vectorField.y > maxLimit) vectorField = new Vector2(vectorField.x, maxLimit);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.MaxWidth(100));
            GUILayout.FlexibleSpace();
            EditorGUILayout.MinMaxSlider(ref vectorField.x, ref vectorField.y, minLimit, maxLimit);
            EditorGUILayout.EndHorizontal();

            if (vectorField.y < vectorField.x)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("Warning : " + label + " X is greater than Y. \nClick 'Fix' to invert values.", MessageType.Warning);
                if (GUILayout.Button("Fix"))
                {
                    float fixY = vectorField.x;
                    float fixX = vectorField.y;
                    vectorField = new Vector2(fixX, fixY);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
