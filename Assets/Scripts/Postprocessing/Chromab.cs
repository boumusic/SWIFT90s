using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(ChromabRenderer), PostProcessEvent.AfterStack, "Custom/Chromab")]
public sealed class Chromab : PostProcessEffectSettings
{
    [Range(-0.8f, 0.8f)]
    public FloatParameter redOffset = new FloatParameter { value = 0f };
    [Range(-0.8f, 0.8f)]
    public FloatParameter greenOffset = new FloatParameter { value = 0f };
    [Range(-0.8f, 0.8f)]
    public FloatParameter blueOffset = new FloatParameter { value = 0f };
}

public sealed class ChromabRenderer : PostProcessEffectRenderer<Chromab>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Chromab"));
        sheet.properties.SetFloat("_ROffset", settings.redOffset);
        sheet.properties.SetFloat("_GOffset", settings.greenOffset);
        sheet.properties.SetFloat("_BOffset", settings.blueOffset);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}