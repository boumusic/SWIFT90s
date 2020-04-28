using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pataya.Utility;

namespace Pataya.QuikFX
{
    public class GradientToTexture : MonoBehaviour
    {
        #region Fields

        [Header("Preview")]
        [Tooltip("Enable the preview of your texture in the Editor.")]
        public bool showPreviewTexture = true;

        [Header("Gradients")]
        [Tooltip("Enable a two-dimensions gradient :  the top gradient will interpolate towards the bottom one based on the Y coordinate.")]
        public bool twoDimensional = false;

        [Tooltip("The top gradient.")]
        public Gradient gradientTop;

        [Tooltip("The bottom gradient.")]
        public Gradient gradientBottom;

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

        #region MonoBehaviour Callbacks

        private void OnValidate()
        {
            UpdatePreviewTexture();
        }

        #endregion

        #region Void Methods

        public void UpdatePreviewTexture()
        {           
            if(showPreviewTexture)
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

                    if(gradientTop != null && gradientBottom != null)
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
