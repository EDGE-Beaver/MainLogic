using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class HoverTextEffect : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUI; // 🎯 UI용 TextMeshPro
    public TextMeshPro textMeshPro3D; // 🎯 3D용 TextMeshPro

    public Color hoverColor = Color.red; // 🎯 호버 시 변경할 색상
    public float hoverSizeMultiplier = 1.5f; // 🎯 호버 시 크기 변경 비율

    private int lastHoveredCharIndex = -1; // 마지막으로 호버한 문자 인덱스
    private Camera uiCamera; // UI 렌더링을 위한 카메라

    void Start()
    {
        uiCamera = Camera.main; // 🎯 기본 카메라 설정

        // 🎯 Collider 자동 추가 (3D TextMeshPro 사용 시 필요)
        if (textMeshPro3D != null && textMeshPro3D.GetComponent<Collider>() == null)
        {
            BoxCollider collider = textMeshPro3D.gameObject.AddComponent<BoxCollider>();
            collider.size = new Vector3(textMeshPro3D.bounds.size.x, textMeshPro3D.bounds.size.y, 0.1f); // 🎯 크기 조정
            Debug.LogWarning($"🚨 {textMeshPro3D.gameObject.name}에 BoxCollider 자동 추가됨.");
        }
    }

    void Update()
    {
        if (textMeshProUI != null)
        {
            DetectHoverUI();
        }
        else if (textMeshPro3D != null)
        {
            DetectHover3D();
        }
    }

    // 🎯 UI용 TextMeshPro 호버 감지
    void DetectHoverUI()
    {
        if (uiCamera == null) return;

        Vector3 mousePosition = Input.mousePosition;

        int charIndex = TMP_TextUtilities.FindIntersectingCharacter(textMeshProUI, mousePosition, uiCamera, true);

        if (charIndex == lastHoveredCharIndex) return;

        if (charIndex != -1)
        {
            Debug.Log($"✅ UI에서 감지된 문자 인덱스: {charIndex}");
            ApplyHoverEffect(textMeshProUI, charIndex);
            lastHoveredCharIndex = charIndex;
        }
        else
        {
            Debug.Log("❌ UI에서 마우스가 글자를 감지하지 못했습니다.");
            lastHoveredCharIndex = -1;
        }
    }

    // 🎯 3D TextMeshPro 호버 감지
    void DetectHover3D()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log($"🖱️ 3D 오브젝트 감지됨: {hit.collider.gameObject.name}");

            if (hit.collider.gameObject == textMeshPro3D.gameObject)
            {
                int charIndex = TMP_TextUtilities.FindIntersectingCharacter(textMeshPro3D, Input.mousePosition, Camera.main, true);

                if (charIndex != -1)
                {
                    Debug.Log($"✅ 3D에서 감지된 문자 인덱스: {charIndex}");
                    ApplyHoverEffect(textMeshPro3D, charIndex);
                    lastHoveredCharIndex = charIndex;
                }
                else
                {
                    Debug.Log("❌ 3D에서 마우스가 글자를 감지하지 못했습니다.");
                    lastHoveredCharIndex = -1;
                }
            }
        }
        else
        {
            Debug.Log("❌ 3D에서 Raycast 충돌 없음.");
        }
    }

    void ApplyHoverEffect(TMP_Text textMesh, int charIndex)
    {
        TMP_TextInfo textInfo = textMesh.textInfo;

        if (charIndex >= 0 && charIndex < textInfo.characterCount && textInfo.characterInfo[charIndex].isVisible)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[charIndex];

            Vector3 offset = (charInfo.topRight - charInfo.bottomLeft) * (hoverSizeMultiplier - 1) / 2;
            ModifyCharacterVertices(textMesh, charIndex, offset);
            ModifyCharacterColor(textMesh, charIndex, hoverColor);
        }
    }

    void ModifyCharacterVertices(TMP_Text textMesh, int charIndex, Vector3 offset)
    {
        TMP_TextInfo textInfo = textMesh.textInfo;
        TMP_CharacterInfo charInfo = textInfo.characterInfo[charIndex];

        Vector3[] vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
        int vertexIndex = charInfo.vertexIndex;

        vertices[vertexIndex + 0] += offset;
        vertices[vertexIndex + 1] += offset;
        vertices[vertexIndex + 2] += offset;
        vertices[vertexIndex + 3] += offset;

        textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }

    void ModifyCharacterColor(TMP_Text textMesh, int charIndex, Color color)
    {
        TMP_TextInfo textInfo = textMesh.textInfo;
        TMP_CharacterInfo charInfo = textInfo.characterInfo[charIndex];

        Color32[] vertexColors = textInfo.meshInfo[charInfo.materialReferenceIndex].colors32;
        int vertexIndex = charInfo.vertexIndex;

        for (int i = 0; i < 4; i++)
        {
            vertexColors[vertexIndex + i] = color;
        }

        textMesh.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }
}
