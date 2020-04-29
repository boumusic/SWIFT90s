using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Pataya.Utility;
using System.IO;

namespace Pataya.QuikFX
{
    public class GradientToTextureWindow : EditorWindow
    {
        #region Fields

        public Editor editorPreviewTex;

        [Header("Preview")]
        [Tooltip("Enable the preview of your texture in the Editor.")]
        public bool showPreviewTexture = true;

        [Header("Gradients")]
        [Tooltip("Enable a two-dimensions gradient :  the top gradient will interpolate towards the bottom one based on the Y coordinate.")]
        public bool twoDimensional = false;

        [Tooltip("The top gradient.")]
        public Gradient gradientTop = new Gradient();

        [Tooltip("The bottom gradient.")]
        public Gradient gradientBottom = new Gradient();

        public List<Color> colors = new List<Color>();
        public Texture2D tex;
        public Texture2D previewTex;

        [Tooltip("The current path of the texture.")]
        public string currentPath = "Assets";

        [Header("Dimensions")]

        [Tooltip("The width (X axis) of the output texture.")]
        [Range(1, 1024)] public int width = 128;

        [Tooltip("The height (Y axis) of the output texture.")]
        [Range(1, 1024)] public int height = 128;

        #endregion

        [MenuItem("Tools/GradientToTexture")]
        static void Init()
        {
            GradientToTextureWindow window = (GradientToTextureWindow)EditorWindow.GetWindow(typeof(GradientToTextureWindow), true, "Gradient to Texture");
            window.Show();
        }

        public void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            UpdatePreviewTexture();
            DrawSettings();

            EditorGUILayout.Space();

            DrawPreview();

            EditorGUILayout.Space();

            DrawGenerateButton();
        }

        private void DrawSettings()
        {
            //Undo.RecordObject(g, "Change Settings");

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField("Dimensions", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("W:", GUILayout.MaxWidth(20));
            width = EditorGUILayout.IntField(width, GUILayout.MaxWidth(50));
            EditorGUILayout.LabelField("H:", GUILayout.MaxWidth(20));
            height = EditorGUILayout.IntField(height, GUILayout.MaxWidth(50));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();
            if(twoDimensional)
            {
                EditorGUILayout.LabelField("Two dimensional Gradient:", EditorStyles.boldLabel);
            }

            else
            {
                EditorGUILayout.LabelField("Two dimensional Gradient:");
            }
            twoDimensional = EditorGUILayout.Toggle(twoDimensional);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            
            gradientTop = EditorGUILayout.GradientField(gradientTop);

            if(GUILayout.Button("Flip gradient <->", EditorStyles.miniButton))
            {
                FlipGradient(gradientTop);
            }

            EditorGUILayout.EndHorizontal();

            if (twoDimensional)
            {
                EditorGUILayout.BeginHorizontal();
                gradientBottom = EditorGUILayout.GradientField(gradientBottom);

                if (GUILayout.Button("Flip gradient <->"))
                {
                    FlipGradient(gradientBottom);
                }

                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button("Invert top and bottom"))
                {
                    Gradient top = gradientTop;
                    gradientTop = gradientBottom;
                    gradientBottom = top;
                    ForceSave();
                }
            }
           
            EditorGUILayout.EndVertical();
        }

        private void FlipGradient(Gradient gradient)
        {
            List<GradientColorKey> flippedCols = new List<GradientColorKey>();
            List<GradientAlphaKey> flippedAlphas = new List<GradientAlphaKey>();

            for (int i = 0; i < gradient.colorKeys.Length; i++)
            {
                GradientColorKey newCol = gradient.colorKeys[i];
                newCol.time = 1 - gradient.colorKeys[i].time;                
                flippedCols.Add(newCol);
            }

            for (int i = 0; i < gradient.alphaKeys.Length; i++)
            {
                GradientAlphaKey newAlpha = gradient.alphaKeys[i];
                newAlpha.time = 1 - gradient.alphaKeys[i].time;
                flippedAlphas.Add(newAlpha);
            }

            gradient.SetKeys(flippedCols.ToArray(), flippedAlphas.ToArray());
            ForceSave();
        }

        private void ForceSave()
        {            
            EditorUtility.SetDirty(this);
            EditorApplication.ExecuteMenuItem("File/Save");            
        }

        private void DrawPreview()
        {
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();
            if(showPreviewTexture) EditorGUILayout.LabelField("Draw Preview Texture", EditorStyles.boldLabel);
            else EditorGUILayout.LabelField("Draw Preview Texture");
            showPreviewTexture = EditorGUILayout.Toggle(showPreviewTexture);
            EditorGUILayout.EndHorizontal();

            if (showPreviewTexture)
            {
                if (editorPreviewTex == null)
                {
                    if (previewTex != null)
                    {
                        editorPreviewTex = Editor.CreateEditor(previewTex);
                    }
                }

                GUIStyle style = new GUIStyle();
                //style.normal.background = new Texture2D(50, 50);
                if (editorPreviewTex != null)
                {
                    editorPreviewTex.OnPreviewGUI(GUILayoutUtility.GetRect(50, 50), style);
                    DestroyImmediate(editorPreviewTex);
                }
            }

            else
            {
                if (editorPreviewTex != null) DestroyImmediate(editorPreviewTex);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawGenerateButton()
        {
            Color defaultCol = GUI.color;
            EditorGUILayout.BeginVertical("box");

            GUI.enabled = false;
            EditorGUILayout.LabelField(currentPath);
            GUI.enabled = true;

            GUI.color = new Color(0.35f, 0.89f, 1f, 1f);
            if (GUILayout.Button("Save Texture"))
            {
                GenerateTexture();
            }
            GUI.color = defaultCol;
            EditorGUILayout.HelpBox("Don't forget to set the Wrap Mode to 'Clamp', and the Alpha is Transparency setting to 'true' in your Texture Import Settings.", MessageType.Info);
            EditorGUILayout.EndVertical();
        }

        private void GenerateTexture()
        {
            UpdateColors(width, height);
            tex = new Texture2D(width, height);
            tex.SetPixels(colors.ToArray());
            tex.Apply();

            string defaultPath = currentPath == "" ? "Assets" : currentPath;

            string savePath = EditorUtility.SaveFilePanel("Save As", defaultPath, "New Gradient Texture", "png");
            if (savePath != "")
            {
                SaveTextureToFile(tex, savePath);
            }
        }

        private void SaveTextureToFile(Texture2D texture, string fileName)
        {
            byte[] bytes = texture.EncodeToPNG();
            FileStream file = File.Open(fileName, FileMode.Create);
            BinaryWriter binary = new BinaryWriter(file);
            binary.Write(bytes);
            file.Close();
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            currentPath = fileName;
        }       
        
        #region Void Methods

        public void UpdatePreviewTexture()
        {
            if (showPreviewTexture)
            {
                //Debug.Log("Updated preview texture");
                UpdateColors(50, 50);

                if (colors.Count > 0)
                {
                    previewTex = new Texture2D(50, 50);
                    previewTex.SetPixels(colors.ToArray());
                    previewTex.Apply();
                }
            }
        }

        public void UpdateColors(int width, int height)
        {
            colors = new List<Color>();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float normalizedX = CustomMethod.Interpolate(0f, 1f, 0f, width - 1, x);
                    float normalizedY = CustomMethod.Interpolate(1f, 0f, 0f, height - 1, y);

                    if (gradientTop != null && gradientBottom != null)
                    {
                        Color colX = gradientTop.Evaluate(normalizedX);
                        Color colY = gradientBottom.Evaluate(normalizedX);

                        Color mixed = Color.Lerp(colX, colY, normalizedY);
                        if (twoDimensional)
                            colors.Add(mixed);
                        else
                        {
                            colors.Add(colX);
                        }
                    }

                }
            }
        }

        #endregion
    }
}