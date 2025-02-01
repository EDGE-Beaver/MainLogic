using UnityEngine;
using UnityEngine.EventSystems;

public class HoverChangeColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private SpriteRenderer spriteRenderer; // 2D SpriteRenderer
    private Color originalColor;          // 원래 색상
    public Color hoverColor = Color.green; // 호버 시 색상 (초록색이 일단 포인트 컬러라고 가정하고 했음)

    void Start()
    {
        // SpriteRenderer 컴포넌트 가져오기
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color; // 원래 색상 저장
        }
        else
        {
            Debug.LogError("이거 안되면 인스펙터 창 지정 가능으로 수정하셔");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = hoverColor; // 호버 시 초록색으로 변경
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor; // 호버 해제 시 원래 색상으로 복구
        }
    }
}
