using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(PixelPorridgeRenderer), PostProcessEvent.AfterStack, "Custom/PixelPorridge")]
public sealed class PixelPorridge : PostProcessEffectSettings
{
    [Range(0f, 1920f), Tooltip("Resolution")]
    public FloatParameter resolution = new FloatParameter { value = 1f };
}

public sealed class PixelPorridgeRenderer : PostProcessEffectRenderer<PixelPorridge>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/PixelPorridge"));
        sheet.properties.SetFloat("_Res", settings.resolution);
        sheet.properties.SetVector("_ScreenSize", new Vector4(Screen.width, Screen.height, 0, 0));
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}