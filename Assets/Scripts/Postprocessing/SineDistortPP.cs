using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(SineDistortPPRenderer), PostProcessEvent.AfterStack, "Custom/SineDistort")]
public sealed class SineDistortPP : PostProcessEffectSettings
{
    [Range(0, 0.007f)]
    public FloatParameter distort = new FloatParameter { value = 0f };
    public FloatParameter tiling = new FloatParameter { value = 0f };
    public FloatParameter scroll = new FloatParameter { value = 0f };
    public ColorParameter color = new ColorParameter { value = Color.white };
    
}

public sealed class SineDistortPPRenderer : PostProcessEffectRenderer<SineDistortPP>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/SineDistort"));
        sheet.properties.SetFloat("_Distort", settings.distort);
        sheet.properties.SetFloat("_Tiling", settings.tiling);
        sheet.properties.SetFloat("_Scroll", settings.scroll);
        sheet.properties.SetColor("_Color", settings.color);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}