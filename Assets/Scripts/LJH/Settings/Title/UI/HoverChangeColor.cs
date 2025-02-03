using UnityEngine;
using UnityEngine.EventSystems;

public class HoverChangeColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private SpriteRenderer spriteRenderer; // 2D SpriteRenderer

    [Header("인스펙터에서 색상 지정")]
    public Color originalColor = Color.white; // 원래 색상 (기본값: 흰색)
    public Color hoverColor = Color.green; // 호버 시 색상 (기본값: 초록색)

    void Start()
    {
        // SpriteRenderer 컴포넌트 가져오기
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor; // 🎯 인스펙터에서 지정한 기본 색상 적용
        }
        else
        {
            Debug.LogError("🚨 SpriteRenderer가 없습니다! 인스펙터에서 색상을 수동으로 지정하세요.");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = hoverColor; // 🎯 호버 시 인스펙터에서 지정한 색상 적용
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor; // 🎯 호버 해제 시 원래 색상 복구
        }
    }
}
