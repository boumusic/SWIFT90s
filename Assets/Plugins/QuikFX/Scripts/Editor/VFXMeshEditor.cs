using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Pataya.QuikFX
{
    [CustomEditor(typeof(VFXMesh))]
    public class VFXMeshEditor : Editor
    {
        public VFXMesh m;

        private void OnEnable()
        {
            m = (VFXMesh)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            InitializeIfNeeded();
            DrawSettings();
            SaveMeshButton();

            if(EditorGUI.EndChangeCheck())
            {
                m.GenerateMesh();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void InitializeIfNeeded()
        {
            if (!m.initialized)
            {
                m.initialized = true;
                m.GenerateMesh();
            }
        }

        private void DrawSettings()
        {
            RegenerateMesh();

            EditorGUILayout.Space();

            Ring();

            EditorGUILayout.Space();

            Loop();

            EditorGUILayout.Space();

            Radius();

            EditorGUILayout.Space();

            Depth();

            EditorGUILayout.Space();

            RingOffset();

            EditorGUILayout.Space();

            Pivot();

            EditorGUILayout.Space();

            UVs();

            EditorGUILayout.Space();

            Normals();            

            EditorGUILayout.Space();

            VertexDebug();
        }

        private void RegenerateMesh()
        {
            EditorGUILayout.BeginVertical("box");
            QuickSerialize("updateInPlayMode");
            EditorGUILayout.EndVertical();
        }

        private void VertexDebug()
        {
            EditorGUILayout.BeginVertical("box");
            QuickSerialize("drawVertexDebug");
            if(m.drawVertexDebug)
            {
                QuickSerialize("vertexSizeDebug");
                QuickSerialize("vertexColorDebug");
            }
            EditorGUILayout.EndVertical();
        }

        private void Normals()
        {
            EditorGUILayout.BeginVertical("box");
            QuickSerialize("flipNormals");
            QuickSerialize("drawNormalDebug");
            if(m.drawNormalDebug) QuickSerialize("normalSizeDebug");
            EditorGUILayout.EndVertical();
        }

        private void UVs()
        {
            EditorGUILayout.BeginVertical("box");
            QuickSerialize("invertUAndV");
            QuickSerialize("displayUVs");
            EditorGUILayout.EndVertical();
        }

        private void Pivot()
        {
            EditorGUILayout.BeginVertical("box");
            QuickSerialize("pivot");
            EditorGUILayout.EndVertical();
        }

        private void RingOffset()
        {
            EditorGUILayout.BeginVertical("box");
            QuickSerialize("arc");
            QuickSerialize("twist");
            EditorGUILayout.EndVertical();
        }

        private void Depth()
        {
            EditorGUILayout.BeginVertical("box");
            QuickSerialize("outerLoopDepth");
            if (m.outerLoopDepth != 0) QuickSerializeCurve("loopHeightPositionRemap", m.loopHeightPositionRemap, 0, 1, true);
            else
            {
                if (ErrorCurve(m.loopHeightPositionRemap, 0, 1) != "")
                {
                    ResetCurve(m.loopHeightPositionRemap, 0, 1);
                }

                else
                {
                    Keyframe zero = m.loopHeightPositionRemap.keys[0];
                    Keyframe one = m.loopHeightPositionRemap.keys[1];
                    if (zero.outTangent != 1 || one.inTangent != 1)
                    {
                        ResetCurve(m.loopHeightPositionRemap, 0, 1);
                    }
                }
            }

            QuickSerialize("maxLoopDepth");

            if (m.maxLoopDepth != 0) QuickSerializeCurve("loopDepthPositionRemap", m.loopDepthPositionRemap, 0, 0, true);

            QuickSerialize("depthByU");
            if (m.depthByU != 0) QuickSerialize("spiralAmount");
            else m.spiralAmount = 1;

            EditorGUILayout.EndVertical();
        }

        private void Radius()
        {
            EditorGUILayout.BeginVertical("box");
            QuickSerialize("minRadius");
            QuickSerialize("maxRadius");
            QuickSerializeCurve("widthRemap", m.widthRemap, 1, 1, false);
            QuickSerialize("widthRemapInfluence");
            EditorGUILayout.EndVertical();
        }

        private void Loop()
        {
            EditorGUILayout.BeginVertical("box");
            QuickSerialize("loopAmount");
            QuickSerializeCurve("loopPositionRemap", m.loopPositionRemap, 0, 1, true);
            EditorGUILayout.EndVertical();
        }

        private void Ring()
        {
            EditorGUILayout.BeginVertical("box");
            QuickSerialize("ringAmount");
            QuickSerializeCurve("ringPositionRemap", m.ringPositionRemap, 0, 1, true);
            EditorGUILayout.EndVertical();
        }

        private void QuickSerialize(string property)
        {
            SerializedProperty p = serializedObject.FindProperty(property);
            EditorGUILayout.PropertyField(p);
        }

        private void QuickSerializeCurve(string name, AnimationCurve curve, float zeroValue, float oneValue, bool checkError)
        {
            QuickSerialize(name);

            if(checkError)
            {
                EditorGUILayout.BeginHorizontal();
                string errorString = ErrorCurve(curve, zeroValue, oneValue);                

                if (errorString != "")
                {
                    EditorGUILayout.HelpBox("Error : " + errorString, MessageType.Error);
                    EditorGUILayout.BeginVertical();

                    Color defaultColor = GUI.color;
                    GUI.color = new Color(0.38f, 1f, 0.56f, 1f);

                    if (GUILayout.Button("Fix", EditorStyles.miniButton))
                    {
                        FixCurve(curve, zeroValue, oneValue);
                    }

                    GUI.color = new Color(1f, 0.38f, 0.54f, 1f);

                    if (GUILayout.Button("Reset", EditorStyles.miniButton))
                    {
                        ResetCurve(curve, zeroValue, oneValue);
                    }

                    GUI.color = defaultColor;
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.EndHorizontal();
            }           
        }
        
        private string ErrorCurve(AnimationCurve curve, float zeroValue, float oneValue)
        {
            string error = "";

            if (curve.keys.Length <= 1) error += "- Curve doesn't have at least two keys.";

            else
            {
                int lastIndex = curve.keys.Length - 1;
                if (curve.keys[0].time != 0 || curve.keys[0].value != zeroValue) error += "\n - First key isnt equal to (0," + zeroValue + ").";
                if (curve.keys[lastIndex].time != 1 || curve.keys[lastIndex].value != oneValue) error += "\n - Last key isnt equal to (0," + oneValue + ").";
            }

            return error;
        }

        private void FixCurve(AnimationCurve curve, float zeroValue, float oneValue)
        {
            Undo.RecordObject(m, "Fix curve.");
            if (curve.keys.Length <= 1)
            {
                ResetCurve(curve, zeroValue, oneValue);
            }

            else
            {
                int lastIndex = curve.keys.Length - 1;
                curve.MoveKey(0, new Keyframe(0, zeroValue, curve.keys[0].inTangent, curve.keys[0].outTangent));
                curve.MoveKey(lastIndex, new Keyframe(1, oneValue, curve.keys[lastIndex].inTangent, curve.keys[lastIndex].outTangent));
            }

            m.GenerateMesh();
            ForceSave();
        }

        private void ResetCurve(AnimationCurve curve, float zeroValue, float oneValue)
        {
            Undo.RecordObject(m, "Reset Curve");
            List<Keyframe> newKeys = new List<Keyframe>();
            newKeys.Add(new Keyframe(0, zeroValue, 0, oneValue - zeroValue));
            newKeys.Add(new Keyframe(1, oneValue, oneValue - zeroValue, 0));
            curve.keys = newKeys.ToArray();
            m.GenerateMesh();
            ForceSave();
        }

        private void SaveMeshButton()
        {
            Color defaultColor = GUI.color;
            GUI.color = new Color(0.35f, 0.89f, 1f, 1f);

            if (m.mesh)
            {
                if (GUILayout.Button("Save Mesh"))
                {
                    SaveMesh(m.mesh, m.name);
                }
            }

            GUI.color = defaultColor;
        }

        public static void SaveMesh(Mesh mesh, string name)
        {
            string path = EditorUtility.SaveFilePanel("Save Mesh Asset", "Assets/", name, "asset");
            if (string.IsNullOrEmpty(path)) return;

            path = FileUtil.GetProjectRelativePath(path);

            Mesh meshToSave = Instantiate(mesh) as Mesh;
            MeshUtility.Optimize(meshToSave);

            AssetDatabase.CreateAsset(meshToSave, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("GameObject/3D Object/VFX Mesh", false, 10)]
        public static void CreateVFXMesh()
        {
            GameObject go = new GameObject("VFX Mesh");
            VFXMesh newVfx = go.AddComponent<VFXMesh>();
            EditorGUIUtility.PingObject(newVfx);
            Selection.activeTransform = newVfx.transform;
        }

        private void ForceSave()
        {
            EditorUtility.SetDirty(m);
            EditorApplication.ExecuteMenuItem("File/Save");
        }        
    }
}
