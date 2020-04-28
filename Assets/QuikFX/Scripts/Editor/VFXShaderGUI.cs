using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Pataya.QuikFX
{
    public class VFXShaderGUI : ShaderGUI
    {
        #region Fields

        private MaterialEditor materialEditor;
        private MaterialProperty[] properties;

        #endregion

        #region Methods

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            //base.OnGUI(materialEditor, properties);

            this.materialEditor = materialEditor;
            this.properties = properties;

            DrawProperties();
        }

        private void DrawProperties()
        {
            Infos();

            EditorGUILayout.Space();

            MainTex();

            EditorGUILayout.Space();

            SecondaryTex();

            EditorGUILayout.Space();

            GradientTex();

            EditorGUILayout.Space();

            Alpha();

            EditorGUILayout.Space();

            Emissive();

            EditorGUILayout.Space();

            Distortion();

            EditorGUILayout.Space();

            Displacement();

            EditorGUILayout.Space();

            CustomData();

            EditorGUILayout.Space();

            Effects();

            materialEditor.RenderQueueField();
        }

        private void Infos()
        {
            EditorGUILayout.HelpBox("Make sure that your textures Wrap Mode are set to 'Repeat' in your Texture Import Settings if you're using the Scrolling Speed parameter.", MessageType.Info);
        }

        private void MainTex()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Main Texture", EditorStyles.boldLabel);
            TextureField("_MainTex", "The Main Texture of the material.", "_TilingSpeedA");
            ShaderProperty("_SpeedMultiplier", "Multiplier affecting all scrolling textures.");
            ShaderProperty("_FresnelColor", "Color of the fresnel.");
            ShaderProperty("_FresnelColorPower", "Power of the fresnel.");
            EditorGUILayout.EndVertical();
        }

        private void SecondaryTex()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Secondary Texture", EditorStyles.boldLabel);
            MaterialProperty useSecond = ShaderProperty("_UseSecondaryTex", "Use a secondary texture on top of the main one.");

            if (useSecond.floatValue == 1)
            {
                TextureField("_SecondaryTex", "Optional secondary texture that will be blended to the Main Texture", "_TilingSpeedB");
                ShaderProperty("_BlendFactor", "0 only displays MainTex, 1 only displays SecondaryTex, intermediate values interpolate both.");
            }

            EditorGUILayout.EndVertical();
        }

        private void Effects()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Effects", EditorStyles.boldLabel);
            ShaderProperty("_Posterize", "Clamp the maximum amount of shades, lower values mean less different shades. Leave at 0 to disable.");
            MaterialProperty pixel = ShaderProperty("_Pixellise", "Pixellise the textures. Lower values mean lower amount of pixels. Leave at 0 to disable.");
            if (pixel.floatValue != 0)
            {
                EditorGUILayout.HelpBox("For better pixellise results, set Advanced/Generate Mips Maps to false and Filter Mode to 'Point' in your Texture Import Settings.", MessageType.Info);
            }
            EditorGUILayout.EndVertical();
        }

        private void GradientTex()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Gradient Map", EditorStyles.boldLabel);
            MaterialProperty useGrad = ShaderProperty("_UseGradientMap", "Map the textures to a gradient instead of outputting their colors.");
            if (useGrad.floatValue == 1)
            {
                TextureField("_GradientTex", "Gradient Texture used for gradient mapping.", "_GradientRemap");
                ShaderProperty("_FlipGradientMap", "Invert the gradient.");
                MaterialProperty scroll = ShaderProperty("_ScrollingSpeedGrad", "Scrolls the gradient texture, resulting in a blink animation.");
                if(scroll.floatValue != 0)
                {
                    EditorGUILayout.HelpBox("Don’t forget to set the Wrap Mode of your gradient texture to ‘Repeat’ in the import settings when using scrolling speed.", MessageType.Info);
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void Alpha()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Alpha", EditorStyles.boldLabel);
            MaterialProperty useAlpha = ShaderProperty("_UseAlpha", "Use transparency effects.");

            if (useAlpha.floatValue == 1)
            {
                MaterialProperty useSecond = FindProperty("_UseSecondaryTex");
                ShaderProperty("_Erosion", "Increase this value to dissolve the texture.");
                if (useSecond.floatValue == 1)
                {
                    MaterialProperty alphaSource = ShaderProperty("_AlphaSource", "0 uses MainTex as alpha source, 1 uses SecondaryTex as alpha source, intermediate values interpolate both.");
                }
                ShaderProperty("_Sharpness", "Edge sharpness of the dissolved texture.");

                EditorGUILayout.Space();

                Color def = GUI.color;
                GUI.color = GUI.color * 0.8f;
                EditorGUILayout.BeginVertical("box");
                GUI.color = def;
                GUILayout.Label("Edge Fade", EditorStyles.boldLabel);
                MaterialProperty fadeU = ShaderProperty("_EdgeFadeU", "Fade the edges on the U axis of the UV coords.");
                ShaderProperty("_OffsetX", "Offset of the effect on the X axis.");
                EditorGUILayout.EndVertical();
                
                GUI.color = GUI.color * 0.8f;
                EditorGUILayout.BeginVertical("box");
                GUI.color = def;
                MaterialProperty fadeV = ShaderProperty("_EdgeFadeV", "Fade the edges on the V axis of the UV coords.");
                ShaderProperty("_OffsetY", "Offset of the effect on the Y axis.");
                EditorGUILayout.EndVertical();

                EditorGUILayout.Space();

                GUILayout.Label("Fresnel", EditorStyles.boldLabel);
                ShaderProperty("_FresnelAlphaPower", "Power of the alpha fresnel.");
            }

            EditorGUILayout.EndVertical();
        }

        private void Emissive()
        {
            EditorGUILayout.BeginVertical("box");

            GUILayout.Label("Emissive", EditorStyles.boldLabel);
            ShaderProperty("_UseEmissive", "Use emissive effects.");

            MaterialProperty useEmissive = FindProperty("_UseEmissive");
            if (useEmissive.floatValue == 1)
            {
                ShaderProperty("_Emissive", "Define the minimum and maximum emissive value. Remap the emissive to apply it only to a certain range of the Main/Secondary textures values.");
                GUILayout.Label("Edge Burn", EditorStyles.boldLabel);
                ShaderProperty("_UseEdgeBurn", "Use edge burn effect.");
                MaterialProperty useEdgeBurn = FindProperty("_UseEdgeBurn");

                if (useEdgeBurn.floatValue == 1)
                {
                    ShaderProperty("_FlipEdgeBurn", "Edge burn appears on lighter or darker values?");
                    ShaderProperty("_EdgeBurnThreshold", "Threshold for an edge burn effect. Higher values mean larger edge burn.");
                    ShaderProperty("_EdgeBurnEmissive", "Emissive value for the edge burn.");
                    ShaderProperty("_EdgeBurnSharpness", "Sharpness of the edge burn.");
                    ShaderProperty("_EdgeBurnColor", "Color applied to the edge burn.");
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void Distortion()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Distortion", EditorStyles.boldLabel);
            MaterialProperty useDistortion = ShaderProperty("_UseDistortion", "Distort the UV coordinates of the Main/Secondary textures.");

            if (useDistortion.floatValue == 1)
            {
                TextureField("_DistortionTex", "Distortion noise texture.", "_TilingSpeedDistortion");
                ShaderProperty("_DistortionStrengthX", "Strength of the distortion on the horizontal axis.");
                ShaderProperty("_DistortionStrengthY", "Strength of the distortion on the vertical axis.");
            }
            EditorGUILayout.EndVertical();
        }

        private void Displacement()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Displacement", EditorStyles.boldLabel);
            ;
            MaterialProperty useDisp = ShaderProperty("_UseDisplacement", "Displace the vertices of the mesh.");

            if (useDisp.floatValue == 1)
            {
                TextureField("_DisplacementTex", "Displacement noise texture.", "_TilingSpeedDisplacement");
                ShaderProperty("_DisplacementStrength", "Strength of the displacement.");
            }

            EditorGUILayout.EndVertical();
        }

        private void CustomData()
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label("Custom Datas", EditorStyles.boldLabel);
            ;
            MaterialProperty useCustom = ShaderProperty("_UseCustomData", "Stream custom vertex data from the particle system to the shader.");

            if (useCustom.floatValue == 1)
            {
                string customDataString = "You can control 3 fields with particle custom datas : " +
                    "\n-Erosion is stored in Custom.x," +
                    "\n-OffsetX is stored in Custom.y" +
                    "\n-OffsetY is stored in Custom.z" +
                    "\nYou can check the prefab VFXMesh ParticleSystem to better understand the setup.";
                EditorGUILayout.HelpBox(customDataString, MessageType.Info);
            }

            EditorGUILayout.EndVertical();
        }

        private void TextureField(string name, string tooltip, string extraProperty)
        {
            MaterialProperty texture = FindProperty(name);
            GUIContent labelTex = new GUIContent(texture.displayName, tooltip);
            materialEditor.TexturePropertySingleLine(labelTex, texture, FindProperty(extraProperty));
        }

        private void FloatField(string name)
        {
            MaterialProperty property = FindProperty(name);
            materialEditor.FloatProperty(property, property.displayName);
        }

        private void VectorField(string name)
        {
            MaterialProperty property = FindProperty(name);
            materialEditor.VectorProperty(property, property.displayName);
        }

        private MaterialProperty ShaderProperty(string name, string tooltip)
        {
            MaterialProperty property = FindProperty(name);
            GUIContent content = new GUIContent(property.displayName, tooltip);
            materialEditor.ShaderProperty(property, content);
            return property;
        }

        private MaterialProperty FindProperty(string name)
        {
            return FindProperty(name, properties);
        }

        #endregion
    }
}