using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(RandomGlitchPPRenderer), PostProcessEvent.AfterStack, "Custom/RandomGlitch")]
public sealed class RandomGlitchPP : PostProcessEffectSettings
{
    [Range(0, 0.05f)]
    public FloatParameter distort = new FloatParameter { value = 0f };
    public FloatParameter tiling = new FloatParameter { value = 0f };
    public FloatParameter scroll = new FloatParameter { value = 0f };
    public FloatParameter speed = new FloatParameter { value = 0f };
    public TextureParameter texture = new TextureParameter { value = null };

}

public sealed class RandomGlitchPPRenderer : PostProcessEffectRenderer<RandomGlitchPP>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/RandomGlitch"));
        sheet.properties.SetFloat("_Distort", settings.distort);
        sheet.properties.SetFloat("_Tiling", settings.tiling);
        sheet.properties.SetFloat("_Scroll", settings.scroll);
        sheet.properties.SetFloat("_Speed", settings.speed);
        if (settings.texture != null) sheet.properties.SetTexture("_NoiseTex", settings.texture);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}