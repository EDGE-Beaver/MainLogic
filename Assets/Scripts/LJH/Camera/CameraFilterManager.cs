using UnityEngine;
using System.Collections;

public class CameraFilterManager : MonoBehaviour
{
    [Header("카메라 필터 효과")]
    public Material blurMaterial;  // 흐릿한 효과
    public Material sharpenMaterial; // 선명한 효과
    public Material grayscaleMaterial; // 흑백 효과
    public Material defaultMaterial; // 기본 필터

    private Camera mainCamera;
    private Material currentFilter;

    void Start()
    {
        mainCamera = Camera.main;
        currentFilter = defaultMaterial;
        ApplyFilter(defaultMaterial);
    }

    public void ApplyFilter(Material filterMaterial)
    {
        if (mainCamera == null || filterMaterial == null) return;
        mainCamera.GetComponent<Camera>().GetComponent<Renderer>().material = filterMaterial;
        currentFilter = filterMaterial;
    }

    public void ApplyFilterByName(string filterName)
    {
        switch (filterName)
        {
            case "흐릿":
                ApplyFilter(blurMaterial);
                break;
            case "선명":
                ApplyFilter(sharpenMaterial);
                break;
            case "흑백":
                ApplyFilter(grayscaleMaterial);
                break;
            default:
                ApplyFilter(defaultMaterial);
                break;
        }
    }
}
