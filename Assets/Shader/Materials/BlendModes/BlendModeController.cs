using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum BlendingMode
{
    Normal,
    Darken,
    Multiply,
    Lighten,
    Additive,
    Soft_Additive,
    Linear_Burn,
    Overlay,
}

public class BlendModeController : MonoBehaviour
{
    public BlendingMode blendMode;
    public Material material;

    void Start()
    {
        ApplyBlendMode();
    }

    void OnValidate()
    {
        ApplyBlendMode();
    }

    void ApplyBlendMode()
    {
        if (material == null)
        {
            var renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                material = renderer.material; // 실행 중에도 반영되도록 변경
            }
            if (material == null) return;
        }

        BlendMode srcMode = BlendMode.SrcAlpha, dstMode = BlendMode.OneMinusSrcAlpha;
        BlendOp op = BlendOp.Add;

        switch (blendMode)
        {
            case BlendingMode.Darken:
                srcMode = BlendMode.One;
                dstMode = BlendMode.One;
                op = BlendOp.Min;
                break;
            case BlendingMode.Multiply:
                srcMode = BlendMode.DstColor;
                dstMode = BlendMode.Zero;
                break;
            case BlendingMode.Lighten:
                srcMode = BlendMode.One;
                dstMode = BlendMode.One;
                op = BlendOp.Max;
                break;
            case BlendingMode.Additive:
                srcMode = BlendMode.One;
                dstMode = BlendMode.One;
                break;
            case BlendingMode.Soft_Additive:
                srcMode = BlendMode.OneMinusDstColor;
                dstMode = BlendMode.One;
                break;
            case BlendingMode.Linear_Burn:
                srcMode = BlendMode.One;
                dstMode = BlendMode.One;
                op = BlendOp.ReverseSubtract;
                break;
            case BlendingMode.Overlay:
                srcMode = BlendMode.One;
                dstMode = BlendMode.SrcColor;
                break;
            default:
                break;
        }

        material.SetInt("_SrcMode", (int)srcMode);
        material.SetInt("_DstMode", (int)dstMode);
        material.SetInt("_BlendOp", (int)op);

        // 디버깅용 로그 출력
        Debug.Log($"Blending Mode Applied: {blendMode}");
        Debug.Log($"_SrcMode: {srcMode}, _DstMode: {dstMode}, _BlendOp: {op}");
    }
}
